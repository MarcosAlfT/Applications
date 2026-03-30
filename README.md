# Applications Solution

The **Applications Solution** is a cloud-ready, microservices-based platform built on **.NET 9** and orchestrated with **.NET Aspire**. It provides identity management, client profile management, and payment processing using the **dLocal** gateway. The platform is designed for **multi-tenant** use, allowing organizations to manage clients and process payments for services like water, electricity, gas, and internet.

---

## 🛠️ Technical Stack

| Component | Technology |
| :--- | :--- |
| **Platform** | .NET 9 |
| **Orchestration** | .NET Aspire 9.5 |
| **Architecture** | Microservices + Event-Driven |
| **Message Broker** | RabbitMQ 3 |
| **Database** | SQL Server (Database-per-service) |
| **Authentication** | OpenIddict (OAuth2 / OpenID Connect) |
| **Payment Gateway** | dLocal |
| **Real-time** | SignalR |

---

## 🏗️ Solution Architecture

The solution follows a **monorepo** approach where each microservice owns its domain, business logic, and database. Services never share databases directly.

### Core Microservices
* **IdentityService**: Handles authentication and authorization using OAuth2 Password and Refresh Token flows.
* **Clients.API**: Manages profiles for individuals (Person) and companies (Organization), linking them to users via JWT claims.
* **Pagarte.API**: The payment HTTP layer that manages credit card registration and initiates payment requests.
* **Pagarte.Worker**: A background service that acts as a payment switch, processing RabbitMQ messages for utility company delivery and refunds.

---

## 🚀 Key Features & Design Patterns

* **Secure Payments**: Credit card data is tokenized via dLocal and never stored in the platform's database.
* **Asynchronous Processing**: Payment delivery to utility companies is handled via RabbitMQ to ensure system responsiveness.
* **Resiliency**: Includes a retry policy for refunds (3 attempts every 5 minutes) and uses **Dead Letter Queues (DLQ)** for failed messages.
* **Real-time Updates**: Payment status changes are pushed to client applications immediately via **SignalR**.
* **Data Integrity**: Implements **Soft Delete** and **Global Query Filters** across all entities to ensure data is never permanently removed by mistake.

---

## 📂 Project Structure

* **AppHost**: The Aspire orchestrator that manages service discovery and health monitoring.
* **Shared.Messaging**: Contains RabbitMQ message contracts (e.g., PaymentRequestMessage).
* **Infrastructure.Messaging**: Provides base logic for RabbitMQ publishers and consumers.
* **Applications.ServiceDefaults**: Shared configuration for OpenTelemetry, health checks, and service discovery.

---

## 🛣️ Future Roadmap

* **RBAC**: Implementation of SystemRoles (Admin, Support) and AppRoles.
* **Advanced Logic**: Multi-currency support and international tax rules.
* **DevOps**: Migration to separate repositories, Docker containerization, and deployment to **Azure Container Apps** or **AKS** via GitHub Actions.
* **Enhanced Security**: Replacing application secrets with **Azure Key Vault**.
