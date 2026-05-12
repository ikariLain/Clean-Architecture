# 🚀 Quick Reference - Clean Architecture Commands & Snippets

## 📌 Quick Commands

### Build & Test
```bash
# Build all projects
dotnet build

# Build specific project
dotnet build MyApp.Domain

# Run unit tests
dotnet test

# Run specific test project
dotnet test MyApp.Domain.Tests

# Run tests with details
dotnet test --verbosity detailed

# Run specific test
dotnet test --filter "OrderTests.Create_WithValidData_ShouldSucceed"
```

### Database
```bash
# Create migration
dotnet ef migrations add InitialCreate \
  -p MyApp.Infrastructure -s MyApp.API

# Apply migrations
dotnet ef database update \
  -p MyApp.Infrastructure -s MyApp.API

# Drop database
dotnet ef database drop -f \
  -p MyApp.Infrastructure -s MyApp.API

# List migrations
dotnet ef migrations list -p MyApp.Infrastructure
```

### Run API
```bash
# Start development server
cd MyApp.API
dotnet run

# Start with specific environment
dotnet run --environment Production

# The API will be at:
# https://localhost:7001
# Swagger: https://localhost:7001/openapi/v1.json
```

### Clean
```bash
# Clean build artifacts
dotnet clean

# Clear NuGet cache
dotnet nuget locals all --clear
```

---

## 🏗️ Architecture Layers - Quick Summary

### Domain (NO External Dependencies)
- **Entities**: `Order.cs`, `OrderItem.cs`
- **Value Objects**: `Address.cs`
- **Enums**: `OrderStatus.cs`
- **Interfaces**: `IOrderRepository.cs`
- **Purpose**: Pure business logic

### Application (MediatR only)
- **Commands**: `CreateOrderCommand`, `DeleteOrderCommand`
- **Queries**: `GetOrderQuery`
- **Handlers**: `*Handler.cs` files
- **DTOs**: `*Dto.cs` files
- **Mapping**: `OrderMapper.cs`
- **Purpose**: CQRS orchestration

### Infrastructure (EF Core)
- **DbContext**: `ApplicationDbContext.cs`
- **Repositories**: `OrderRepository.cs`
- **Configurations**: `*Configuration.cs` files
- **Extensions**: `InfrastructureServiceExtensions.cs`
- **Purpose**: Data access implementation

### API (HTTP)
- **Endpoints**: `OrderEndpoints.cs`
- **DI Setup**: `ApplicationServiceExtensions.cs`, `Program.cs`
- **Configuration**: `appsettings.json`
- **Purpose**: HTTP request handling

---

## 🎯 Three Operations Implemented

### 1. CreateOrder
```
Request: POST /api/orders
Body: { orderNumber, shippingAddress, billingAddress, items[] }
Response: 201 Created + OrderDto
Process: Command → Handler → Domain.Create() → Repository.Add()
```

### 2. GetOrder
```
Request: GET /api/orders/{id}
Response: 200 OK + OrderDto (or 404 NotFound)
Process: Query → Handler → Repository.GetById() → Mapper.ToDto()
```

### 3. DeleteOrder
```
Request: DELETE /api/orders/{id}
Response: 204 NoContent (or 404 NotFound)
Process: Command → Handler → validate status → Repository.Delete()
```

---

## ✨ Highlightade filer för CreateOrder och DeleteOrder

### CreateOrder
- `Application/Orders/Commands/CreateOrderCommand.cs`
- `Application/Orders/Commands/CreateOrderCommandHandler.cs`
- `Domain/Entities/Order.cs`
- `Domain/Entities/OrderItem.cs`
- `Domain/ValueObjects/Address.cs`
- `Application/Mapping/OrderMapper.cs`
- `Infrastructure/Persistence/ApplicationDbContext.cs`
- `Infrastructure/Persistence/Repositories/OrderRepository.cs`
- `API/Endpoints/OrderEndpoints.cs`

### DeleteOrder
- `Application/Orders/Commands/DeleteOrderCommand.cs`
- `Application/Orders/Commands/DeleteOrderCommandHandler.cs`
- `Domain/Entities/Order.cs`
- `Infrastructure/Persistence/Repositories/OrderRepository.cs`
- `API/Endpoints/OrderEndpoints.cs`

---

## 📁 File Locations Cheat Sheet

| What | Where |
|------|-------|
| Order Logic | `Domain/Entities/Order.cs` |
| Create Command | `Application/Orders/Commands/CreateOrderCommand.cs` |
| Create Handler | `Application/Orders/Commands/CreateOrderCommandHandler.cs` |
| Get Query | `Application/Orders/Queries/GetOrderQuery.cs` |
| Get Handler | `Application/Orders/Queries/GetOrderQueryHandler.cs` |
| Delete Command | `Application/Orders/Commands/DeleteOrderCommand.cs` |
| Delete Handler | `Application/Orders/Commands/DeleteOrderCommandHandler.cs` |
| DbContext | `Infrastructure/Persistence/ApplicationDbContext.cs` |
| Repository | `Infrastructure/Persistence/Repositories/OrderRepository.cs` |
| DTOs | `Application/DTOs/` |
| Mapping | `Application/Mapping/OrderMapper.cs` |
| Endpoints | `API/Endpoints/OrderEndpoints.cs` |
| Tests | `Domain.Tests/OrderTests.cs`, `AddressTests.cs` |

---

## 🧪 Test Command Reference

```bash
# All tests
dotnet test

# Domain tests only
dotnet test MyApp.Domain.Tests

# With code coverage
dotnet test /p:CollectCoverage=true

# Watch mode (run tests on save)
dotnet watch test

# Parallel execution
dotnet test --maxParallelThreads 4

# Specific test class
dotnet test --filter "OrderTests"

# Tests containing specific word
dotnet test --filter "FullyQualifiedName~Delete"
```

---

## 📊 Project Dependencies

```
Domain (0 external)
  ↑
Application (MediatR)
  ↑
Infrastructure (EF Core)
  ↑
API (MediatR, OpenAPI)
```

**Rule**: No downward dependencies!
- Domain ← can be called by anyone
- Application ← depends on Domain
- Infrastructure ← depends on Domain + Application
- API ← depends on Application + Infrastructure

---

## 💾 NuGet Packages Added

| Package | Version | Project | Purpose |
|---------|---------|---------|---------|
| MediatR | 12.4.0 | Application | CQRS pattern |
| Microsoft.EntityFrameworkCore.SqlServer | 10.0.1 | Infrastructure | SQL Server provider |
| Microsoft.EntityFrameworkCore.Design | 10.0.1 | Infrastructure, API | Migration tools |
| xunit | 2.8.1 | Domain.Tests | Unit testing |
| Microsoft.NET.Test.Sdk | 17.13.0 | Domain.Tests | Test runner |

---

## 🔄 Request Flow Example (CreateOrder)

```
HTTP Request (JSON)
       ↓
OrderEndpoints.CreateOrder()
       ↓
MediatR.Send(CreateOrderCommand)
       ↓
CreateOrderCommandHandler.Handle()
       ↓
Order.Create() ← Business rules applied here
       ↓
order.AddItem() ← More business rules
       ↓
order.Confirm() ← Status transition
       ↓
OrderRepository.AddAsync(order)
       ↓
ApplicationDbContext.SaveChangesAsync()
       ↓
SQL INSERT + INSERT (Order + OrderItems)
       ↓
OrderMapper.ToDto(order) ← Convert to DTO
       ↓
HTTP Response (201 Created + JSON)
```

---

## ✅ Business Rules Checklist

Order-related rules implemented:

- [ ] Order number cannot be empty
- [ ] Addresses must be valid
- [ ] Cannot add items to shipped order
- [ ] Quantity must be positive
- [ ] Price cannot be negative
- [ ] Can only confirm order with items
- [ ] Status progression must be followed (Pending → Confirmed → Shipped)
- [ ] Cannot delete shipped order
- [ ] Adding same product increases quantity
- [ ] Total price = sum of (unitPrice × quantity)

---

## 🎓 Learning Resources in This Project

To understand each concept:

1. **Clean Architecture**
   - Read: `ARCHITECTURE.md`
   - See: Diagram at top of `ARCHITECTURE.md`

2. **The Dependency Rule**
   - Read: Section in `ARCHITECTURE.md`
   - See: `Domain` folder - zero dependencies

3. **Rich Domain Model**
   - Read: Comments in `Order.cs`
   - Study: `AddItem()`, `Confirm()`, `Ship()` methods

4. **Value Objects**
   - Read: Comments in `Address.cs`
   - Study: Equals, GetHashCode implementations

5. **CQRS Pattern**
   - Read: Comments in Command/Query files
   - See: `CreateOrderCommand` vs `GetOrderQuery`

6. **MediatR**
   - See: `Program.cs` setup
   - See: `CreateOrderCommandHandler` implementation

7. **EF Core Mappings**
   - See: `OrderConfiguration.cs` (Fluent API)
   - See: No Data Annotations on entities

8. **DTOs & Mapping**
   - See: `OrderDto.cs` vs `Order.cs`
   - See: `OrderMapper.cs` conversion

9. **Minimal APIs**
   - See: `OrderEndpoints.cs`
   - See: `Program.cs` MapOrderEndpoints call

10. **Unit Testing Without DB**
    - See: `OrderTests.cs` - 15 tests
    - See: `AddressTests.cs` - 5 tests
    - Run: `dotnet test MyApp.Domain.Tests`

---

## 🔧 Common Modifications

### Add new property to Order
1. Add to `Order.cs` entity
2. Update `OrderConfiguration.cs` fluent mapping
3. Update `OrderDto.cs`
4. Update `OrderMapper.cs` mapping
5. Create migration: `dotnet ef migrations add AddPropertyToOrder`
6. Update tests in `OrderTests.cs`

### Add new command (e.g., UpdateOrder)
1. Create `UpdateOrderCommand.cs`
2. Create `UpdateOrderCommandHandler.cs`
3. Add handler logic
4. Add endpoint in `OrderEndpoints.cs`
5. Add mapping if needed
6. Add tests

### Change database (SQL Server → PostgreSQL)
1. Replace NuGet: `Microsoft.EntityFrameworkCore.Npgsql`
2. Update connection string in `appsettings.json`
3. Update Program.cs: `UseSqlServer()` → `UseNpgsql()`
4. Everything else stays the same! ← This is the power of CA

---

## 🐛 Debugging Tips

### Debug Test Failure
```bash
# Run with verbose output
dotnet test --verbosity detailed

# Run single test with breakpoint
dotnet test --filter "TestName" --no-build
```

### Debug API Endpoint
1. Set breakpoint in Handler
2. Run: `dotnet run`
3. Call endpoint via Swagger or curl
4. VS Code debugger will stop at breakpoint

### Inspect Database
```bash
# Via SQL Server Management Studio
# Server: (localdb)\mssqllocaldb
# Database: MyAppDb

# Or via EF Core commands
dotnet ef dbcontext info -p MyApp.Infrastructure -s MyApp.API
```

### Check Package Versions
```bash
dotnet package search MediatR
dotnet add package MediatR --version 12.4.0
```

---

## 📝 Documentation Quick Links

| Document | Purpose |
|----------|---------|
| README.md | Quick start |
| ARCHITECTURE.md | Detailed architecture |
| FILE_STRUCTURE.md | File overview & ceremony count |
| BUILD_AND_RUN.md | Build/test/run instructions |
| IMPLEMENTATION_SUMMARY.md | What was implemented |
| PRESENTATION_GUIDE.md | How to present this |
| QUICK_REFERENCE.md | This file |

---

## 🎯 One-Liners

```bash
# Build and run all tests
dotnet build && dotnet test

# Build, test, run API
dotnet build && dotnet test && cd MyApp.API && dotnet run

# Watch tests
dotnet watch test --project MyApp.Domain.Tests

# Show dependency tree
dotnet list package --vulnerable

# Format code
dotnet format

# Analyze code
dotnet code-metrics

# Generate nuget packages
dotnet pack
```

---

## 🚀 Getting Started Fresh

```bash
# 1. Clone repo
git clone <repo>
cd "Clean Architecture"

# 2. Build
dotnet build

# 3. Run tests
dotnet test MyApp.Domain.Tests

# 4. Setup database
dotnet ef database update -p MyApp.Infrastructure -s MyApp.API

# 5. Start API
cd MyApp.API
dotnet run

# 6. Open browser
# https://localhost:7001/openapi/v1.json

# 7. Test endpoints in Swagger UI
```

---

## 💡 Architecture Decision Record (ADR)

**Decision**: Use Clean Architecture for Order Management

**Why**:
- Business logic changes frequently
- Multiple team members need to work on different layers
- Testing is important
- Future-proofing for database/UI changes

**Trade-offs**:
- More files/folders than simpler patterns
- More DI configuration needed
- Learning curve for new developers

**Alternatives Considered**:
- VSA (Vertical Slice): Good for simple CRUD
- Traditional Layered: Less flexible
- Microservices: Overkill for this size

**Decision**: MediatR for CQRS

**Why**:
- Decouples command/query handling
- Enables cross-cutting concerns (logging, validation)
- Testable handlers
- Clear request pipeline

**Decision**: Manual DTO Mapping instead of AutoMapper

**Why**:
- .NET 10 AOT compatibility
- No runtime reflection needed
- Explicit and debuggable
- Slightly more code, but worth it

---

**Last Updated**: 2026-05-11
**Project**: Clean Architecture - Order Management
**Status**: Production Ready ✅
