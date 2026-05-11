# Pagarte Backend Design

## Runtime Shape

Pagarte is split into a public API, an internal services project, and an async engine.

```text
Pagarte.API  ---- gRPC ---->  Pagarte.Services  ---- SQL Server: PagarteDb
                                      |
                                      | publishes payment messages
                                      v
                                 RabbitMQ
                                      |
                                      v
                              Pagarte.Engine
```

`AppHost` is local development orchestration only. Production should run each service independently and should provide production infrastructure such as RabbitMQ, SQL Server, logs, metrics, and secrets outside Aspire.

## Projects

### `Pagarte.API`

Public HTTP entry point for Pagarte. It validates OpenIddict access tokens, extracts the authenticated client/user identity, and calls `Pagarte.Services` over gRPC.

It does not own payment persistence or call payment operators directly.

### `Pagarte.Services`

Internal gRPC service and owner of `PagarteDb`.

Responsibilities:

- Credit card registration and card persistence.
- Service catalog access.
- Payment quote/preview creation.
- Payment confirmation.
- Payment and quote persistence.
- Payment operator resolution for card registration and payment confirmation.
- Payment operator charge call during confirmation.
- Publishing payment messages to RabbitMQ after a card charge succeeds.

This project was previously named `Pagarte.Worker`. It is not only a background worker, so `Pagarte.Services` is the clearer name.

### `Pagarte.Engine`

Async payment orchestration after the payment operator charge has succeeded.

Responsibilities:

- Consume `payment.request`.
- Send payment to the company.
- Mark payment as completed when the company accepts it.
- Mark payment as `CompanyPaymentFailed` and trigger refund when the company rejects it.
- Consume refund requests and retry failed refunds.
- Publish alert messages when refund fails after maximum retries.

Engine updates payment status through SQL and does not reference `Pagarte.Services`, avoiding a circular dependency.

### `Pagarte.Messaging`

Shared Pagarte payment message contracts and RabbitMQ topology.

This project owns Pagarte-specific exchanges, queues, routing keys, and messages such as:

- `PaymentRequestMessage`
- `RefundRequestMessage`
- `AlertMessage`

Notification messaging should become a separate generic area later, for example:

```text
Notifications.Messaging
Notifications.Worker
```

That notification system should own its own topology and support multiple channels such as email, SMS, WhatsApp, push, and admin alerts.

### `Pagarte.Connections`

External adapters for payment operators and company integrations.

Payment operator selection is not hardcoded in the payment flow. `Pagarte.Services` resolves the active operator from the database, then uses `Pagarte.Connections` to obtain the correct adapter.

`PaymentOperator:Provider` in configuration is used for initial/local setup and seeding, not as a permanent business rule. No default provider should be silently assumed.

Current payment operator adapter shape:

```text
IPaymentOperatorAdapter
  RegisterCard
  Charge
  Refund

IPaymentOperatorAdapterFactory
  provider code -> adapter
```

Cards and payments store the operator provider that was used. This matters because card tokens and payment ids belong to the operator that created them and cannot safely be reused by another operator.

### `RabbitMQ`

Shared RabbitMQ connection, publisher, and base consumer infrastructure.

## Payment Quote Flow

Payment is now a two-step flow.

### 1. Quote / Preview

The client asks for a quote before paying.

```text
POST /api/payment/quote
```

Input:

```text
service id
currency
```

`Pagarte.Services` calculates and stores:

- service amount
- payment operator fee
- company fee
- Pagarte fee
- taxes, if any
- discounts, if any
- total amount
- currency
- expiration time
- quote id
- status: `Unpaid`

Quotes expire after 60 minutes. A quote is not a payment yet. It is persisted for traceability, statistics, and to ensure the confirmed payment uses the same values shown to the client.

### 2. Confirm / Pay

The client confirms a quote and chooses a card.

```text
POST /api/payment/confirm
```

Input:

```text
quote id
credit card id
```

`Pagarte.Services` then:

1. Validates the quote belongs to the client.
2. Validates the quote is still `Unpaid`.
3. Validates the quote is not expired.
4. Validates the card belongs to the client.
5. Creates a payment with status `Confirmed`.
6. Copies the quote details into payment details.
7. Updates payment to `ChargingCard`.
8. Uses the card's stored operator provider.
9. Calls the payment operator synchronously for the quote total.
10. If charge fails, marks payment `Failed`.
11. If charge succeeds, marks quote `Paid`.
12. Saves `OperatorProvider` and `OperatorPaymentId`.
13. Updates payment to `CardCharged`.
14. Publishes `PaymentRequestMessage` to RabbitMQ.

The credit card charge is intentionally synchronous. The client should know immediately whether the card was charged or rejected. The Engine only continues the asynchronous company/payment-delivery flow after a successful card charge.

## Payment Operators

Operators are card/payment processors, not companies.

Current model:

```text
PaymentOperator
  Code
  Name
  Scope: International | Local
  Priority
  IsActive
```

For now the system resolves the active `International` operator. This supports the current flow while leaving room for future routing by local/international transactions, country, currency, or priority.

When a card is registered:

```text
active international operator -> register card -> save OperatorProvider + OperatorCardToken
```

When a quote is confirmed:

```text
credit card -> OperatorProvider -> adapter factory -> charge synchronously
```

When a refund is needed:

```text
PaymentRequestMessage.OperatorProvider -> RefundRequestMessage.OperatorProvider -> adapter factory -> refund
```

This allows future operators such as `DLocal` or another processor without changing the payment confirmation flow.

## Companies

Companies are different from operators. A company is the recipient/provider behind the service being paid.

```text
Service -> Company -> Company connection strategy
```

The Engine should eventually resolve company connection details from `ServiceId`/`CompanyId`, not from hardcoded endpoint strings.

Expected company connection types:

```text
External
  Direct API
  Middleware API
  Web service

Internal
  Manual/physical information
  No external Pagarte.Connections call
```

External company integrations belong in `Pagarte.Connections`. Internal/manual payments should be resolved inside the application/engine workflow without pretending they are external API calls.

## Engine Flow

After the card is charged, Engine continues the async work.

```text
payment.request
    -> SendingPaymentToCompany
    -> call company
        -> Completed
        -> CompanyPaymentFailed
            -> Refunding
            -> refund.request
                -> Refunded
                -> RefundFailed
```

If the company accepts the payment, status becomes `Completed`.

If the company rejects the payment, status becomes `CompanyPaymentFailed`, then `Refunding`, and Engine publishes a refund request.

Refunds are retried up to three times. If all attempts fail, the payment becomes `RefundFailed` and an alert message is published.

Refund requests include the operator provider so the refund uses the same processor that charged the card.

## Payment Statuses

Payment statuses are:

```text
Confirmed
ChargingCard
CardCharged
SendingPaymentToCompany
Completed
CompanyPaymentFailed
Failed
Refunding
Refunded
RefundFailed
```

`Quoted` is intentionally not a payment status. Quote state belongs to `PaymentQuote`.

Quote statuses are:

```text
Unpaid
Paid
```

## RabbitMQ Topology

Current Pagarte queues:

```text
pagarte.payments exchange
  payment.request
  refund.request

pagarte.notifications exchange
  email.send
  alert.create
```

Current dead-letter queues:

```text
payment.request.dlq
refund.request.dlq
email.send.dlq
```

Both `Pagarte.Services` and `Pagarte.Engine` declare the Pagarte topology on startup, making startup idempotent.

## Local Development

AppHost wires local services:

- Identity
- Clients API
- Pagarte API
- Pagarte Services
- Pagarte Engine
- RabbitMQ

For local development, AppHost injects:

```text
AuthSettings__Authority
PagarteServices__GrpcUrl
```

Production should configure these values through environment variables, secrets, or cloud service configuration.
