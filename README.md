# OrderService
This .NET 9 microservice handles order processing following DDD principles. It exposes two REST API
endpoints (/orders POST and GET) and logs via Serilog to file. PostgreSQL is hosted in
Docker.
## Architecture Decisions
### Domain Layer:
- **Order (Aggregate Root):** The Order entity acts as the aggregate root, responsible for maintaining
the consistency of the order as a whole. It manages a collection of OrderItems, enforces business
rules such as ensuring all items are in stock and ensures no external
component can modify child entities directly. It provides behavior for creating orders and adding/
removing items while ensuring invariants are preserved.
- **OrderItem (Entity):** References Value Objects for ProductId, ProductName, ProductAmount,
ProductPrice.
### Application Layer:
- **Commands & Queries:** MediatR is used for handling create and retrieve operations.
- **Validation:** FluentValidation used to ensure command/query correctness (e.g., email format, order number format).
- **DTOs:** Data Transfer Objects for API contracts.
### Infrastructure Layer:
- **ORM:** Entity Framework Core is used to map entities with DB.
- **Repository Pattern:** Repository pattern used to abstract data access logic from the application.
- **Database:** PostgreSQL is used as database, configured via Docker and connection string in appsettings.
### API Layer:
- **OrdersController:** Exposes POST /orders and GET /orders/{id} .
- **Exception Handling:** Middleware to translate exceptions to HTTP responses.
- **Logging:** Serilog configured to write logs to file.
## Testing Strategy:
- **Unit Tests:**  Domain invariants, handlers, repository logic.
- **Integration Tests:** API endpoints against TestServer and in-memory DB.
## Getting Started
1. Ensure Docker is running.
2. Run following to start PostgreSQL in docker:
   ```
   docker run -d \
    --name my-postgres \
    -e POSTGRES_USER=${DB_USER} \
    -e POSTGRES_PASSWORD=${DB_PASSWORD} \
    -e POSTGRES_DB=ordersdb\
    -p 5432:5432 \
    -v pgdata:/var/lib/postgresql/data \
    postgres:15
4. Update `appsettings.Development.json` with your DB connection string.
5. Run EF migration:
   ```
   dotnet ef migrations add InitialCreate --context OrderDbContext --project OrderService --output-dir Infrastructure/Persistence/Migrations
   dotnet ef database update --context OrderDbContext --project OrderService
7. `dotnet run --project OrderService`.
8. Access `swagger` at `https://localhost:7124/swagger`.
