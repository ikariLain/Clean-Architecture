# File Structure - Clean Architecture Order Management

## 📁 Mapstruktur

```
MyApp.slnx (Solution file)
│
├── MyApp.Domain/                          [CORE - NO EXTERNAL DEPS]
│   ├── Entities/
│   │   ├── Order.cs                      [Rich Entity med affärslogik]
│   │   └── OrderItem.cs                  [Child Entity]
│   ├── Enums/
│   │   └── OrderStatus.cs                [Enum för Order status]
│   ├── ValueObjects/
│   │   └── Address.cs                    [Value Object - immutable]
│   └── Repositories/
│       └── IOrderRepository.cs           [Interface - Contract för data access]
│
├── MyApp.Application/                    [ORCHESTRATION - CQRS & MediatR]
│   ├── DTOs/
│   │   ├── OrderDto.cs                   [Data Transfer Object]
│   │   ├── OrderItemDto.cs
│   │   └── AddressDto.cs
│   ├── Mapping/
│   │   └── OrderMapper.cs                [Manual mapping - Entity ↔ DTO]
│   └── Orders/
│       ├── Commands/
│       │   ├── CreateOrderCommand.cs     [Command - Skapar order]
│       │   ├── CreateOrderCommandHandler.cs
│       │   ├── DeleteOrderCommand.cs     [Command - Raderar order]
│       │   └── DeleteOrderCommandHandler.cs
│       └── Queries/
│           ├── GetOrderQuery.cs          [Query - Hämtar order]
│           └── GetOrderQueryHandler.cs
│
├── MyApp.Infrastructure/                 [DATA ACCESS - EF Core]
│   ├── Persistence/
│   │   ├── ApplicationDbContext.cs       [EF Core DbContext]
│   │   ├── Configuration/
│   │   │   ├── OrderConfiguration.cs     [Fluent API mapping]
│   │   │   └── OrderItemConfiguration.cs
│   │   └── Repositories/
│   │       └── OrderRepository.cs        [Implementation av IOrderRepository]
│   └── InfrastructureServiceExtensions.cs [DI-registrering]
│
├── MyApp.API/                            [HTTP LAYER - Minimal APIs]
│   ├── Endpoints/
│   │   └── OrderEndpoints.cs             [HTTP-endpoints: POST, GET, DELETE]
│   ├── Extensions/
│   │   └── ApplicationServiceExtensions.cs [DI-setup för Application]
│   ├── Program.cs                        [Composition Root - Main setup]
│   ├── appsettings.json
│   └── appsettings.Development.json
│
└── MyApp.Domain.Tests/                   [UNIT TESTS - NO DATABASE]
    ├── OrderTests.cs                     [14 Unit Tests för Order]
    ├── AddressTests.cs                   [5 Unit Tests för Address]
    └── MyApp.Domain.Tests.csproj
```

## 📋 Antal filer per operation

### CreateOrder Implementation

Filer som SKAPADES eller ÄNDRADES:

**Domain Layer:**
- ✅ `Domain/Entities/Order.cs` (CREATE)
- ✅ `Domain/Entities/OrderItem.cs` (CREATE)
- ✅ `Domain/Enums/OrderStatus.cs` (CREATE)
- ✅ `Domain/ValueObjects/Address.cs` (CREATE)
- ✅ `Domain/Repositories/IOrderRepository.cs` (CREATE)

**Application Layer:**
- ✅ `Application/DTOs/OrderDto.cs` (CREATE)
- ✅ `Application/DTOs/OrderItemDto.cs` (CREATE)
- ✅ `Application/DTOs/AddressDto.cs` (CREATE)
- ✅ `Application/Mapping/OrderMapper.cs` (CREATE)
- ✅ `Application/Orders/Commands/CreateOrderCommand.cs` (CREATE)
- ✅ `Application/Orders/Commands/CreateOrderCommandHandler.cs` (CREATE)

**Infrastructure Layer:**
- ✅ `Infrastructure/Persistence/ApplicationDbContext.cs` (CREATE)
- ✅ `Infrastructure/Persistence/Configuration/OrderConfiguration.cs` (CREATE)
- ✅ `Infrastructure/Persistence/Configuration/OrderItemConfiguration.cs` (CREATE)
- ✅ `Infrastructure/Persistence/Repositories/OrderRepository.cs` (CREATE)
- ✅ `Infrastructure/InfrastructureServiceExtensions.cs` (CREATE)
- ✅ `Infrastructure/MyApp.Infrastructure.csproj` (MODIFY - add NuGet packages)

**API Layer:**
- ✅ `API/Endpoints/OrderEndpoints.cs` (CREATE)
- ✅ `API/Extensions/ApplicationServiceExtensions.cs` (CREATE)
- ✅ `API/Program.cs` (MODIFY - setup DI and MediatR)
- ✅ `API/appsettings.json` (MODIFY - add connection string)
- ✅ `API/MyApp.API.csproj` (MODIFY - add NuGet packages)

**Application Project:**
- ✅ `Application/MyApp.Application.csproj` (MODIFY - add MediatR)

**Tests:**
- ✅ `Domain.Tests/OrderTests.cs` (CREATE)
- ✅ `Domain.Tests/AddressTests.cs` (CREATE)
- ✅ `Domain.Tests/MyApp.Domain.Tests.csproj` (CREATE)

**Totalt: 27 filer för CreateOrder funktionalitet** ✓

### GetOrder Implementation

Filer som REDAN FANNS + NY:

- ✅ `Application/Orders/Queries/GetOrderQuery.cs` (CREATE - 1 ny fil)
- ✅ `Application/Orders/Queries/GetOrderQueryHandler.cs` (CREATE - 1 ny fil)
- ✅ `API/Endpoints/OrderEndpoints.cs` (MODIFY - lägg till MapGet)

**Totalt: 3 filer för GetOrder** (2 nya, 1 modifierad)

### DeleteOrder Implementation

Filer som REDAN FANNS + NYA:

- ✅ `Application/Orders/Commands/DeleteOrderCommand.cs` (CREATE - 1 ny fil)
- ✅ `Application/Orders/Commands/DeleteOrderCommandHandler.cs` (CREATE - 1 ny fil)
- ✅ `API/Endpoints/OrderEndpoints.cs` (MODIFY - lägg till MapDelete)
- ✅ `Infrastructure/Persistence/Repositories/OrderRepository.cs` (MODIFY - redan har DeleteAsync)

**Totalt: 3-4 filer för DeleteOrder** (2 nya, 1-2 modifierade)

## 🔍 Varför fler filer än VSA?

I VSA (Vertical Slice Architecture) skulle allt för DeleteOrder ligga tillsammans:
- `Features/DeleteOrder/DeleteOrderCommand.cs`
- `Features/DeleteOrder/DeleteOrderHandler.cs`
- `Features/DeleteOrder/DeleteOrderDbAccess.cs`
- `Features/DeleteOrder/DeleteOrderTests.cs`

**≈ 4 filer samlat**

I Clean Architecture är det bredare:
- **Domain**: IOrderRepository.DeleteAsync interface
- **Application**: DeleteOrderCommand + Handler
- **Infrastructure**: OrderRepository.DeleteAsync implementation
- **API**: OrderEndpoints.MapDelete mapping

**≈ 6-7 filer fördelat**

### Trade-offs:

| Aspekt | CA | VSA |
|--------|----|----|
| Filöverblick | Bredare | Tätare |
| Separation of Concerns | Strikt | Flexibel |
| Testning av Domain | Lätt | Svårare |
| Databaskompetens | Lög | Högt |
| Börja utveckla | Längre setup | Snabbare start |

## 📊 Sammanfattning - "Ceremony Count"

För order-hantering TOTALT:

| Operation | Nya Filer | Modifierade Filer | Total |
|-----------|-----------|-------------------|-------|
| CreateOrder | 19 | 8 | 27 |
| GetOrder | 2 | 1 | 3 |
| DeleteOrder | 2 | 1-2 | 3-4 |
| **Totalt** | **23** | **10-11** | **33-34** |

**Men:** Efter initial setup (CreateOrder), varje ny operation kräver bara **2-4 filer**!

## 🎓 Lektionen

Clean Architecture är inte om **antal filer**, det är om:

1. ✅ **Affärslogik isolerad** från teknik (Domain)
2. ✅ **Testning enkelt** utan infrastructure
3. ✅ **Ändringar isolerade** till relevanta lager
4. ✅ **Beroenden pekar inåt** - "The Dependency Rule"
5. ✅ **Framtidsbeständig** - enkelt att byta databas/UI/framework

Välj arkitektur baserat på:
- Projektets storlek
- Team-storlek
- Komplexitet i affärslogik
- Förutsatta ändringar

---

**Skapad: 2026-05-11**
**File Structure och Ceremony Count för Clean Architecture Order Management**
