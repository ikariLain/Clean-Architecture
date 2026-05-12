# Clean Architecture - Order Management Feature

## 📐 Arkitektur-Översikt

Denna implementering visar hur Clean Architecture principerna tillämpas på en modern .NET 10-applikation.

```
┌─────────────────────────────────────────────────────────┐
│                    API Layer (Thin)                     │
│  - Minimal APIs (OrderEndpoints)                         │
│  - HTTP ↔ MediatR mapping                               │
│  - Dependency Injection setup                           │
│  - Beroende på: Application + Infrastructure            │
└─────────────────────────────────────────────────────────┘
                          ↓
┌─────────────────────────────────────────────────────────┐
│              Application Layer (Orchestration)          │
│  - CQRS Commands/Queries (MediatR)                      │
│  - Handlers (CreateOrderCommandHandler, etc.)           │
│  - DTOs (OrderDto, AddressDto)                          │
│  - Mapping Logic (OrderMapper)                          │
│  - Beroende på: Domain                                  │
└─────────────────────────────────────────────────────────┘
                          ↓
┌─────────────────────────────────────────────────────────┐
│             Domain Layer (Core Business Logic)          │
│  - Entities (Order, OrderItem)                          │
│  - Value Objects (Address)                              │
│  - Enums (OrderStatus)                                  │
│  - Repository Interfaces (IOrderRepository)             │
│  - NO External Dependencies                             │
└─────────────────────────────────────────────────────────┘
                          ↓
┌─────────────────────────────────────────────────────────┐
│             Infrastructure Layer (Data Access)          │
│  - EF Core DbContext (ApplicationDbContext)             │
│  - Repository Implementation (OrderRepository)          │
│  - Entity Configurations (Fluent API)                   │
│  - Beroende på: Domain + Application                    │
└─────────────────────────────────────────────────────────┘

VIKTIGT: Alla pilar pekar INÅT mot Domain!
```

## 🎯 Tre Order-Operationer

### 1. CreateOrder - Visar affärsregler och validering

**Request:**
```json
POST /api/orders
{
  "orderNumber": "ORD-2026-001",
  "shippingAddress": {
    "street": "Main Street 123",
    "city": "Stockholm",
    "state": "Stockholm",
    "postalCode": "10100",
    "country": "Sweden"
  },
  "billingAddress": {
    "street": "Main Street 123",
    "city": "Stockholm",
    "state": "Stockholm",
    "postalCode": "10100",
    "country": "Sweden"
  },
  "items": [
    {
      "productId": "PROD-001",
      "productName": "Widget",
      "price": 99.99,
      "quantity": 2
    }
  ]
}
```

**Affärsregler som tillämpas:**
- Ordernummer kan inte vara tomt
- Adresser måste vara giltiga
- Varor kan inte ha negativ pris eller kvantitet
- Ordern skapas i `Pending` status
- Ordern bekräftas automatiskt → `Confirmed` status

**Flöde i koden:**
1. **API** (`OrderEndpoints.cs`) - Mottager POST-request
2. **MediatR** - Skickar `CreateOrderCommand` till handler
3. **Application** (`CreateOrderCommandHandler`) - Validering och orkestrering
4. **Domain** (`Order.Create()`) - Skapa entitet med affärsregler
5. **Infrastructure** (`OrderRepository`) - Spara i SQL Server
6. **DTO** (`OrderDto`) - Mappa tillbaka för response

### 2. GetOrder - Visar användningen av DTOs

**Request:**
```http
GET /api/orders/{id}
```

**Response:**
```json
{
  "id": "f47ac10b-58cc-4372-a567-0e02b2c3d479",
  "orderNumber": "ORD-2026-001",
  "status": "Confirmed",
  "createdAt": "2026-05-11T10:30:00Z",
  "updatedAt": "2026-05-11T10:30:15Z",
  "totalPrice": 199.98,
  "totalItems": 2,
  "shippingAddress": { ... },
  "billingAddress": { ... },
  "items": [
    {
      "id": "guid...",
      "productId": "PROD-001",
      "productName": "Widget",
      "unitPrice": 99.99,
      "quantity": 2,
      "total": 199.98
    }
  ]
}
```

**Varför DTOs?**
- Entiteter (Order, OrderItem) läckar INTE ut till API:et
- DTOs ger oss kontroll över vad som exponeras
- Möjliggör breaking changes i Domain utan att påverka API-klienter
- Säkerhet: Känslig data kan filtreras bort

### 3. DeleteOrder - "Ceremony Count" demonstration

**Request:**
```http
DELETE /api/orders/{id}
```

**Affärsregel:** Kan bara radera ordrar som INTE är skickade (inte `Shipped` status)

## 📊 Ceremony Count - Filer för DeleteOrder

I Clean Architecture är radering väl isolerad. Här är alla filer som är **involverade** när man implementerar DeleteOrder:

### Domain Layer (1 fil)
- `IOrderRepository.cs` - DeleteAsync-metod redan definierad ✓

### Application Layer (2 filer)
- `DeleteOrderCommand.cs` - Definitionen av kommandot
- `DeleteOrderCommandHandler.cs` - Logiken för radering

### Infrastructure Layer (1 fil)
- `OrderRepository.cs` - Implementationen av DeleteAsync

### API Layer (1 fil)
- `OrderEndpoints.cs` - Mapping av DELETE-endpoint

### Tests (minimal påverkan)
- Befintliga Domain-tests täcker redan scenariot

**Totalt: ~5 filer för komplett radera-funktionalitet**

### Jämfört med traditionell VSA (Vertical Slice Architecture):
- VSA:ande tillsammans alla lager för DeleteOrder i SAMMA fil/mapp
- CA: Logiken är fördelad men väl organiserad och testbar
- Det spelar mindre roll vilken arkitektur du väljer;
  **Viktigt är att konsekvent tillämpa den valda arkitekturen**

## ✅ Affärsregler (Business Rules)

Alla affärsregler implementeras i **Domain-lagret** (Order-entiteten):

```csharp
// Affärsregel 1: Kan inte lägga till vara till skickad order
if (Status == OrderStatus.Shipped || Status == OrderStatus.Cancelled)
    throw new InvalidOperationException(...);

// Affärsregel 2: Kvantitet måste vara positiv
if (quantity <= 0)
    throw new ArgumentException(...);

// Affärsregel 3: Kan bara bekräfta order med varor
if (_items.Count == 0)
    throw new InvalidOperationException(...);

// Affärsregel 4: Order-status följer en strängt definierad progression
// Pending → Confirmed → Shipped
// eller Pending/Confirmed → Cancelled
```

**Varför är detta viktigt?**
- Affärsreglerna ligger på ett ställe (DRY - Don't Repeat Yourself)
- Enkelt att testa affärsreglerna oberoende av databas/API
- Om règeln behöver ändras, ändrar du den på ett ställe

## 🔄 Mappning: Databas → Entitet → DTO → API Response

```
1. SQL Server                    Order-tabell
                                   ↓
2. EF Core                      Order-entitet (med affärslogik)
                                   ↓
3. OrderMapper.ToDto()          OrderDto (säker, exponerbar)
                                   ↓
4. MediatR Handler              JSON-response till klient
```

**Mappningskostnad:**
- Mer kod (OrderMapper)
- Men: Större säkerhet och flexibilitet
- Lätt att lägga till beräknade egenskaper (TotalPrice, TotalItems)
- Enkelt att filtrera känslig data

## 🧪 Testning - Unit Tests utan databas

Alla tests i `MyApp.Domain.Tests`:
- ✓ Kör på **millisekunder** (ingen databas)
- ✓ Är **deterministiska** (samma input = samma output alltid)
- ✓ Kan köras **parallellt**
- ✓ Testar **affärslogik direkt**

Exempel på test:
```csharp
[Fact]
public void Confirm_WithoutItems_ShouldThrow()
{
    // Arrange
    var order = Order.Create("ORD-001", address, address);

    // Act & Assert
    Assert.Throws<InvalidOperationException>(() => order.Confirm());
}
```

Denna test:
- Skapar en Order utan att röra databasen
- Verifiera att affärsregeln tillämpas
- Körs på < 1ms

## 🎁 Fördelar med denna arkitektur

### 1. **Löst kopplad (Loosely Coupled)**
- Domain vet INGET om SQL Server, HTTP, eller EF Core
- Enkelt att byta databas (mongo → PostgreSQL → cosmosDB)
- Bara ändra OrderRepository-implementationen

### 2. **Högt sammanhängande (Highly Cohesive)**
- Alla Domain-klasser fokuserar på affärslogik
- Application-lagret fokuserar på orchestration
- Infrastructure fokuserar på teknik

### 3. **Testbar (Testable)**
- Domain-tests utan mocks
- Snabba, deterministiska tests
- Fokus på affärslogik inte teknik

### 4. **Underhållbar (Maintainable)**
- Klart vad varje lager är ansvarigt för
- Nykomlingar förstår arkitekturen
- Ändringar i ett lager påverkar inte andra

### 5. **Skalbar (Scalable)**
- Lätt att lägga till nya features
- Lager-strukturen förhindrar kaos
- CQRS möjliggör senare split mellan read/write

## 📈 Nästa steg (för presentationen)

1. **Köra Unit Tests**
   ```bash
   dotnet test MyApp.Domain.Tests
   ```

2. **Köra Applikationen**
   ```bash
   dotnet run --project MyApp.API
   ```

3. **Testa Endpoints**
   ```bash
   # Swagger UI på: https://localhost:7001/openapi/v1.json
   POST /api/orders
   GET /api/orders/{id}
   DELETE /api/orders/{id}
   ```

4. **Visa Arkitektur-diagram**
   - Visa pilar pekar inåt
   - Förklara varje lager

5. **Jämför med VSA**
   - Samma funktionalitet
   - Olika struktur
   - Diskutera trade-offs

## 🔗 Projektberoenden

```
MyApp.API
├── MyApp.Application (MediatR, DTOs, Handlers)
└── MyApp.Infrastructure (EF Core, Repositories)

MyApp.Application
└── MyApp.Domain (Entities, Value Objects, Interfaces)

MyApp.Infrastructure
├── MyApp.Application
└── MyApp.Domain

MyApp.Domain (ZERO external dependencies!)
```

**Regel: Inget projekt i nedre lager får bero på projekt i övre lager!**

## 📝 Notering om NuGet-paket

- **Domain**: Inga externa beroenden (ren och fokuserad)
- **Application**: MediatR (för CQRS-mönstret)
- **Infrastructure**: EF Core, SQL Server provider
- **API**: MediatR (för middleware), OpenAPI

Detta gör projektet **AOT-kompatibelt** för .NET 10 och senare.

---

**Skapad: 2026-05-11**
**Clean Architecture Implementation för Order Management**
