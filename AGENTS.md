# Repository Instructions

- Use Windows line endings (`CRLF`) for edited and newly created files.

# General Design

Identity / IdentityService
  -> Responses

Clients / Clients.API
  -> Responses
  -> Identity / IdentityService (runtime auth reference)

Pagarte.API
  -> Responses
  -> Pagarte.Contracts
  -> Identity / IdentityService (runtime auth reference)
  -> Pagarte.Services (runtime gRPC calls)

Pagarte.Services
  -> RabbitMQ
  -> Responses
  -> Pagarte.Connections
  -> Pagarte.Contracts
  -> Pagarte.Messaging

Pagarte.Engine
  -> RabbitMQ
  -> Pagarte.Connections
  -> Pagarte.Messaging

RabbitMQ
  -> Used at runtime by Pagarte.Services and Pagarte.Engine

Pagarte.Connections
  -> Referenced by Pagarte.Services and Pagarte.Engine

Pagarte.Contracts
  -> Referenced by Pagarte.API and Pagarte.Services

Pagarte.Messaging
  -> Referenced by Pagarte.Services and Pagarte.Engine

Responses
  -> Referenced by IdentityService, Clients.API, Pagarte.API, and Pagarte.Services

# Pagarte Projects instructions

- Do not hardcode payment operator selection in business flow. Use configuration only to seed or configure operators, then resolve the active operator through the application/database model.
- Keep credit card charge synchronous during payment confirmation. The async Engine flow starts after the card is charged.
- Store the operator provider used for card tokens and payments, because operator tokens are not portable across processors.


