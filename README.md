# Clean Architecture - Order Management System

## 🎯 Projekt-beskrivning

Detta är en moderna .NET 10-implementering av **Clean Architecture**-principerna med en komplett Order Management-feature. Projektet är utformat för att demonstrera:

- Affärslogik isolerad från infrastruktur (Domain-driven Design)
- CQRS-mönstret med MediatR
- Testbar arkitektur (unit tests utan databas)
- Beroenden som pekar inåt mot Domain
- Praktiska trade-offs mellan arkitektur-mönster

## GitHub

Repository: [https://github.com/ikariLain/Clean-Architecture](https://github.com/ikariLain/Clean-Architecture)

## 📚 Dokumentation

Denna mapp innehåller:

1. **[ARCHITECTURE.md](ARCHITECTURE.md)** - Detaljerad arkitektur-förklaring
   - Diagram över alla lager
   - Förklaring av Order-operationerna
   - Affärsregler och validering
   - Fördelar med arkitekturen

2. **[FILE_STRUCTURE.md](FILE_STRUCTURE.md)** - Filöversikt och Ceremony Count
   - Mapstruktur för alla projekt
   - Antal filer för varje operation
   - Jämföring med VSA (Vertical Slice Architecture)

3. **[BUILD_AND_RUN.md](BUILD_AND_RUN.md)** - Praktisk guide
   - Hur man bygger projekten
   - Hur man kör unit tests
   - Hur man startar API:et
   - Hur man testar endpoints
   - Troubleshooting

## 🏗️ Projekt-struktur

```
MyApp
├── MyApp.Domain/                    [KÄRNA - Affärslogik]
├── MyApp.Application/               [ORCHESTRATION - CQRS]
├── MyApp.Infrastructure/            [TEKNIK - EF Core, SQL]
├── MyApp.API/                       [HTTP LAYER - Endpoints]
├── MyApp.Domain.Tests/              [UNIT TESTS]
└── Solution File: MyApp.slnx
```

## 🎯 Tre Order-operationer

### 1. ✅ CreateOrder
- Skapar en ny order med varor
- Validerar affärsregler
- Bekräftar ordern automatiskt
- **Filer:** 19 nya + 8 modifierade = **27 totalt**

### 2. 🔍 GetOrder
- Hämtar en order efter ID
- Returnerar OrderDto (ej entiteten)
- Visar DTO-mappningen
- **Filer:** 2 nya + 1 modifierad = **3 totalt**

### 3. 🗑️ DeleteOrder
- Raderar en order
- Validerar att order inte är skickad
- Demonstrerar isolerad implementering
- **Filer:** 2 nya + 1-2 modifierad = **3-4 totalt**

## 🚀 Kom igång

### 1. Bygg projekten
```bash
dotnet build
```

### 2. Kör unit tests
```bash
dotnet test MyApp.Domain.Tests
```

### 3. Skapa databas (migrations)
```bash
dotnet ef database update -p MyApp.Infrastructure -s MyApp.API
```

### 4. Starta API
```bash
cd MyApp.API
dotnet run
```

### 5. Öppna Swagger
```
https://localhost:7001/openapi/v1.json
```

## 📊 Arkitektur-överblick

```
┌──────────────────────────────┐
│     API Layer (HTTP)         │  🌐 Minimal APIs
├──────────────────────────────┤
│  Application (CQRS)          │  📝 Commands, Queries, Handlers, DTOs
├──────────────────────────────┤
│     Domain (Business)        │  💼 Entities, Value Objects, Rules
├──────────────────────────────┤
│   Infrastructure (Data)      │  💾 EF Core, Repositories
└──────────────────────────────┘

Regel: Pilar pekar INÅT ➡️ Domain
```

## 🧪 Unit Tests

Alla tests finns i `MyApp.Domain.Tests`:

```bash
# Kör alla tests
dotnet test MyApp.Domain.Tests

# Eller specifika test-klasser
dotnet test MyApp.Domain.Tests --filter "OrderTests"
```

**19 tests**, körs på < 20ms totalt, **utan databas**! ✨

### Test-exempel:
```csharp
[Fact]
public void AddItem_ToShippedOrder_ShouldThrow()
{
    // Arrange
    var order = Order.Create("ORD-001", address, address);
    order.AddItem("PROD-001", "Widget", 99.99m, 1);
    order.Confirm();
    order.Ship();

    // Act & Assert
    Assert.Throws<InvalidOperationException>(() =>
        order.AddItem("PROD-002", "Gadget", 49.99m, 1));
}
```

## 🔑 Nyckel-koncept

### 1. **Rich Domain Model**
```csharp
// Affärslogik ligger i entiteten
order.AddItem(productId, name, price, quantity);
order.Confirm();
order.Ship();
order.Cancel();
```

### 2. **Value Objects (DDD)**
```csharp
// Address är immutable och identitetslös
var address = Address.Create(street, city, state, postalCode, country);
```

### 3. **CQRS Pattern**
```csharp
// Commands ändrar data
await mediator.Send(new CreateOrderCommand(...));

// Queries läser data
var order = await mediator.Send(new GetOrderQuery(id));
```

### 4. **Dependency Injection**
```csharp
// Services registreras i Program.cs
services.AddApplication();
services.AddInfrastructure(connectionString);
```

### 5. **DTO Mapping**
```csharp
// Entities → DTOs → JSON Response
var orderDto = OrderMapper.ToDto(order);
```

## ✅ Affärsregler

Alla affärsregler är implementerade i `Order.cs`:

- ✔️ Ordernummer kan inte vara tomt
- ✔️ Kan inte lägga till vara till skickad order
- ✔️ Kvantitet måste vara positiv
- ✔️ Pris kan inte vara negativt
- ✔️ Kan bara bekräfta order med varor
- ✔️ Order-status följer progression: Pending → Confirmed → Shipped
- ✔️ Kan inte radera skickad order

## 📈 Nästa steg

1. **Läs arkitektur-dokumentationen:** [ARCHITECTURE.md](ARCHITECTURE.md)
2. **Förstå filstrukturen:** [FILE_STRUCTURE.md](FILE_STRUCTURE.md)
3. **Bygg och testa:** [BUILD_AND_RUN.md](BUILD_AND_RUN.md)
4. **Experimentera** - lägg till nya operationer (UpdateOrder, GetAllOrders, etc.)

## 🎓 Lärdom

Clean Architecture är **inte om antal filer**, det är om:

- **Separation of Concerns**: Varje lager har ett ansvar
- **Testability**: Affärslogik kan testas isolerat
- **Maintainability**: Lätt att förstå och ändra
- **Flexibility**: Enkelt att byta databas/UI/framework
- **The Dependency Rule**: Beroenden pekar alltid inåt mot Domain

## 🔗 Projekt-beroenden

```
Domain (0 external)
  ↑
Application (MediatR)
  ↑
Infrastructure (EF Core)
API (MediatR, OpenAPI)
  ↓
  Infrastructure
    ↓
    Application
      ↓
      Domain
```

**Viktigt:** Låg till höger kan bero på allt, höger till vänster kan INTE bero på det!

## 💡 Tips för presentationen

1. **Visa unit tests** - kör `dotnet test` och visa snabbheten
2. **Visa arkitektur-diagram** - förklara pilar inåt
3. **Visa DeleteOrder-implementering** - visa hur isolerad den är
4. **Kör API:et** - demo av endpoints i Swagger
5. **Visa DTO-mappningen** - förklara två-stegs konvertering
6. **Jämför med VSA** - diskutera trade-offs

## 📝 Notering

- Denna implementering följer .NET 10 best practices
- AOT-kompatibel (ingen reflection i kritiska vägar)
- Använder Minimal APIs (IKKE Controllers)
- MediatR för CQRS-mönstret
- EF Core 10 för data access

## ❓ FAQ

**F: Varför så många lager?**
A: För att visa separation of concerns. För ett litet projekt kan du slå ihop Application och Infrastructure.

**F: Varför DTOs?**
A: För att skydda entiteterna från exponering och för att möjliggöra oberoende evolution av API.

**F: Varför MediatR?**
A: För att decentraslisera request-hantering och möjliggöra CQRS-mönstret enkelt.

**F: Kan jag använda AutoMapper istället för manuell mappning?**
A: Ja, men då mister du AOT-kompatibilitet. Manuell mappning är bättre för .NET 10.

## 👨‍💻 Skapad för

Denna implementering är skapad för att demonstrera Clean Architecture i en modern .NET-miljö, perfekt för:
- Università-presentations
- Code reviews
- Arkitektur-diskussioner
- Learning purposes

---

**Skapad: 2026-05-11**
**Clean Architecture - Order Management System**
**Framework: .NET 10 | Pattern: Clean Architecture + CQRS | Database: SQL Server**
