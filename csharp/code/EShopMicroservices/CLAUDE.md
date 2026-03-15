# CLAUDE.md

This file provides guidance to Claude Code (claude.ai/code) when working with code in this repository.

## Project Overview

This is a microservices-based e-commerce solution built with .NET 8. The system consists of multiple backend services, an API gateway, and a frontend web application. Services are containerized using Docker and orchestrated with Docker Compose.

## Architecture

### Service Structure

- **BuildingBlocks**: Shared libraries containing CQRS abstractions, validation behaviors, logging, exception handling, and messaging (MassTransit).
- **Services**:
  - `Catalog.API`: Product catalog service using Marten (event sourcing) and PostgreSQL.
  - `Basket.API`: Shopping cart service with Marten, Redis caching, and gRPC communication to Discount service.
  - `Discount.Grpc`: Discount service using gRPC, Entity Framework Core, and SQLite.
  - `Ordering.API`: Order processing service with clean architecture (Domain, Application, Infrastructure layers), SQL Server, and RabbitMQ integration.
  - `Identity.API`: Authentication and authorization service using ASP.NET Core Identity, JWT, and PostgreSQL.
- **ApiGateways**: `YarpApiGateway` reverse proxy using YARP.
- **WebApps**: `Shopping.Web` Razor Pages frontend using Refit HTTP clients to communicate through the API gateway.

### Key Technologies

- **CQRS/MediatR**: Command and query separation with MediatR pipeline behaviors for validation and logging.
- **Carter**: Minimal API framework for endpoint definitions.
- **Marten**: Event sourcing and document database for Catalog and Basket services.
- **MassTransit**: Message broker abstraction over RabbitMQ for asynchronous communication.
- **gRPC**: Used for synchronous service-to-service communication between Basket and Discount services.
- **YARP**: Reverse proxy for API gateway.
- **Refit**: Type-safe HTTP client for frontend API calls.
- **JWT/Identity**: JSON Web Tokens for authentication with ASP.NET Core Identity for user management.

### Data Stores

- **PostgreSQL**: CatalogDb (port 5432), BasketDb (port 5433), IdentityDb (port 5434)
- **Redis**: Distributed cache (port 6379)
- **SQL Server**: OrderDb (port 1433)
- **SQLite**: DiscountDb (file-based)
- **RabbitMQ**: Message broker (AMQP port 5672, management UI port 15672)

## Development Workflow

### Running the System

The easiest way to run the entire system is using Docker Compose:

```bash
docker-compose up -d
```

This will start all services, databases, and the frontend. Services are accessible on the following ports:

- **Catalog.API**: http://localhost:6000
- **Basket.API**: http://localhost:6001
- **Discount.Grpc**: http://localhost:6002
- **Ordering.API**: http://localhost:6003
- **YarpApiGateway**: http://localhost:6004
- **Shopping.Web**: http://localhost:6005
- **RabbitMQ Management**: http://localhost:15672 (guest/guest)

### Building and Debugging

#### Solution Build

```bash
dotnet build
```

#### Individual Service Development

Each service can be run independently using its project launch settings. For example, to run Catalog.API locally:

```bash
dotnet run --project Services/Catalog/Catalog.API
```

Most services have launch profiles for HTTP, HTTPS, and Docker. The solution also includes a "Docker Compose" launch profile that starts debugging for selected services (catalog.api, basket.api, discount.grpc, yarpapigateway, shopping.web).

#### Database Initialization

Some services include database seeders that run automatically in development environment:
- Catalog.API: `CatalogInitialData` seeds product data
- Ordering.API: `app.InitialiseDatabaseAsync()` creates and seeds the OrderDb

## Service Dependencies

- **Basket.API** depends on Discount.Grpc (gRPC) and RabbitMQ
- **Ordering.API** depends on RabbitMQ
- **Shopping.Web** depends on YarpApiGateway
- **YarpApiGateway** routes requests to backend services

## API Gateway Routing

The YARP gateway routes requests based on path prefixes:

- `/catalog-service/**` → Catalog.API (http://catalog.api:8080)
- `/basket-service/**` → Basket.API (http://basket.api:8080)
- `/ordering-service/**` → Ordering.API (http://ordering.api:8080) with rate limiting
- `/identity-service/**` → Identity.API (http://identity.api:8080)

The gateway strips the service prefix before forwarding (e.g., `/catalog-service/products` becomes `/products`). This allows the frontend to call a single gateway address while maintaining logical service separation.

**Authentication**: The gateway validates JWT tokens for protected routes and passes user information to backend services via request headers (`X-User-Id`, `X-User-Name`).

## Configuration

Service configurations are managed through:
1. `appsettings.json` in each project
2. Environment variables in `docker-compose.override.yml`
3. Docker Compose environment variables for connection strings

Key configuration sections:
- `ConnectionStrings:Database`: Database connection string
- `ConnectionStrings:Redis`: Redis connection string
- `ConnectionStrings:IdentityDatabase`: Identity service database connection string
- `GrpcSettings:DiscountUrl`: Discount gRPC service URL
- `MessageBroker:Host`: RabbitMQ connection URL
- `ApiSettings:GatewayAddress`: API gateway address (used by frontend)
- `JwtSettings:Secret`: JWT signing key (keep secure!)
- `JwtSettings:Issuer`: JWT issuer
- `JwtSettings:Audience`: JWT audience
- `JwtSettings:ExpiryMinutes`: JWT expiration time in minutes

## Common Tasks

### Adding a New Endpoint

1. Create a command/query in the appropriate service's `Features` folder
2. Implement the handler using MediatR
3. Create a Carter module to expose the endpoint
4. Register the module in the service's dependency injection (if needed)

### Adding a New Service

1. Create a new project under `Services/`
2. Follow patterns from existing services:
   - Use MediatR with validation and logging behaviors
   - Use Carter for endpoints
   - Add health checks for dependencies
   - Configure exception handling via `CustomExceptionHandler`
3. Update `docker-compose.yml` and `docker-compose.override.yml`
4. Add service to YARP gateway configuration if needed

### Testing Async Messaging

Use the RabbitMQ management console (http://localhost:15672) to monitor queues and messages.

### Adding Authentication to Existing Services

1. **Protect endpoints**: Add `[Authorize]` attribute to endpoints requiring authentication
2. **Extract user context**: In gateway, extract user claims and add to request headers (`X-User-Id`, `X-User-Name`)
3. **Access user info**: In services, read user information from request headers or `HttpContext.User`
4. **Update entities**: Add `UserId` and `UserName` fields to entities that need user association (e.g., Order, ShoppingCart)

## Notes

- The solution does not currently include unit or integration test projects.
- The Ordering service uses a clean architecture pattern with separate Domain, Application, and Infrastructure projects.
- Basket service implements a caching decorator pattern (`CachedBasketRepository` wraps `BasketRepository`).
- Catalog and Basket services use Marten's lightweight sessions for database operations.
- Health check endpoints are available at `/health` for each service.
- Basket.API's gRPC client is configured to accept any server certificate (for development only).
- Ordering.API includes a feature flag `FeatureManagement__OrderFullfilment` to toggle order fulfillment functionality.
- **Authentication**: The system uses JWT tokens with ASP.NET Core Identity. Tokens are validated at the API gateway, and user information is passed to backend services via request headers.
- **Protected routes**: Ordering endpoints require authentication, while catalog browsing may be public. Adjust authorization policies as needed.