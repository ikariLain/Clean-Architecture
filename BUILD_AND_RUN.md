# 🚀 Guide - Build, Test & Run

## 1️⃣ Förutsättningar

- .NET 10 SDK installerat
- SQL Server (LocalDB eller expresss)
- Visual Studio Code eller Visual Studio

## 2️⃣ Bygg projekten

### Build allt
```bash
cd "c:\Users\Matheus\source\vc\.NET\Clean Architecture"
dotnet build
```

### Build specifikt projekt
```bash
dotnet build MyApp.API
dotnet build MyApp.Domain
```

### Rebuildbuild (rensa först)
```bash
dotnet clean
dotnet build
```

## 3️⃣ Kör Unit Tests

### Kör alla tests
```bash
dotnet test
```

### Kör specifika test-projekt
```bash
dotnet test MyApp.Domain.Tests
```

### Kör med verbose output
```bash
dotnet test --verbosity detailed
```

### Kör ett specifikt test
```bash
dotnet test --filter "OrderTests.Create_WithValidData_ShouldSucceed"
```

### Exempel på test-utmatning:
```
✓ OrderTests.Create_WithValidData_ShouldSucceed (1.2ms)
✓ OrderTests.Create_WithEmptyOrderNumber_ShouldThrow (0.8ms)
✓ OrderTests.AddItem_WithValidData_ShouldAddItem (0.9ms)
✓ OrderTests.AddItem_ToShippedOrder_ShouldThrow (0.7ms)
✓ OrderTests.TotalPrice_WithMultipleItems_ShouldCalculateCorrectly (1.1ms)
...
Totalt: 19 tests, 15.8ms
```

## 4️⃣ Database-migration (Entity Framework Core)

### Skapa initial migration
```bash
cd MyApp.Infrastructure
dotnet ef migrations add InitialCreate -p ../MyApp.Infrastructure -s ../MyApp.API
```

### Tillämpa migration på databas
```bash
dotnet ef database update -p MyApp.Infrastructure -s MyApp.API
```

### Kolla status
```bash
dotnet ef migrations list -p MyApp.Infrastructure
```

### Radera senaste migration
```bash
dotnet ef migrations remove -p MyApp.Infrastructure
```

## 5️⃣ Kör API:et

### Start i Development mode
```bash
cd MyApp.API
dotnet run --configuration Development
```

### Output:
```
Building...
...
info: Microsoft.Hosting.Lifetime[14]
      Now listening on: https://localhost:7001
      Now listening on: http://localhost:5000
```

### Åtkomst:
- API: `https://localhost:7001`
- Swagger/OpenAPI: `https://localhost:7001/openapi/v1.json`

## 6️⃣ Testa Endpoints

### Via curl

#### 1. Create Order
```bash
curl -X POST "https://localhost:7001/api/orders" \
  -H "Content-Type: application/json" \
  -d '{
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
      },
      {
        "productId": "PROD-002",
        "productName": "Gadget",
        "price": 49.99,
        "quantity": 1
      }
    ]
  }'
```

#### 2. Get Order (ersätt {id} med verkligt ID från Create response)
```bash
curl -X GET "https://localhost:7001/api/orders/{id}" \
  -H "Accept: application/json"
```

#### 3. Delete Order
```bash
curl -X DELETE "https://localhost:7001/api/orders/{id}"
```

### Via REST Client Extension i VS Code

Skapa fil: `test.http`

```http
@host=https://localhost:7001
@orderNumber = ORD-2026-001

### Create Order
POST {{host}}/api/orders
Content-Type: application/json

{
  "orderNumber": "{{orderNumber}}",
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

### Get Order
@orderId = <paste id from create response>
GET {{host}}/api/orders/{{orderId}}

### Delete Order
DELETE {{host}}/api/orders/{{orderId}}
```

### Via Swagger UI

1. Starta API: `dotnet run` i MyApp.API
2. Öppna i webbläsare: `https://localhost:7001/openapi/ui`
3. Testa endpoints direkt i UI

## 7️⃣ Troubleshooting

### Problem: "Could not find a valid certificate..."
**Lösning:** Acceptera localhost-certifikat
```bash
dotnet dev-certs https --trust
```

### Problem: "Connection string not found"
**Lösning:** Säkerställ `appsettings.json` har `ConnectionStrings`:
```json
{
  "ConnectionStrings": {
    "DefaultConnection": "Server=(localdb)\\mssqllocaldb;Database=MyAppDb;Trusted_Connection=true;"
  }
}
```

### Problem: "Migration failed"
**Lösning:** Radera database och kör igen
```bash
dotnet ef database drop -f -p MyApp.Infrastructure -s MyApp.API
dotnet ef database update -p MyApp.Infrastructure -s MyApp.API
```

### Problem: "MediatR not finding handlers"
**Lösning:** Säkerställ `Program.cs` registrerar rätt assembly:
```csharp
services.AddMediatR(cfg =>
    cfg.RegisterServicesFromAssemblies(
        typeof(CreateOrderCommand).Assembly)); // Application-assembly
```

## 8️⃣ Profiling & Performance

### Mät test-performance
```bash
dotnet test --collect:"XPlat Code Coverage"
```

### Mät API-responstid
```bash
# Use Stopwatch i handler
// Använd Stopwatch eller Application Insights
```

### Kolla Entity Framework SQL
```csharp
// I Program.cs:
builder.Services.AddLogging(config => 
    config.AddConsole().SetMinimumLevel(LogLevel.Debug));
```

## 9️⃣ Clean Up

### Radera build-artifacts
```bash
dotnet clean
```

### Radera NuGet packages
```bash
dotnet nuget locals all --clear
```

### Radera databas
```bash
# SQL Server Management Studio:
DROP DATABASE MyAppDb;

# Eller via EF Core:
dotnet ef database drop -f -p MyApp.Infrastructure -s MyApp.API
```

## 📚 Presentation Checklist

För din presentation, gör detta:

- [ ] Visa arkitektur-diagram (pilar pekar inåt)
- [ ] Kör Unit Tests: `dotnet test MyApp.Domain.Tests`
- [ ] Start API: `dotnet run` i MyApp.API
- [ ] Skapa Order via Swagger eller curl
- [ ] Hämta Order och visa DTO-mappningen
- [ ] Radera Order
- [ ] Visa källkod för affärsregler i Order.cs
- [ ] Visa hur DeleteOrder är isolerat till få filer
- [ ] Diskutera trade-offs: CA vs VSA

## 🎯 Performance Baselines (ungefärliga)

- Unit test: < 2ms
- HTTP POST CreateOrder: 50-100ms (inkl. databas)
- HTTP GET GetOrder: 30-50ms (inkl. databas)
- HTTP DELETE: 20-40ms (inkl. databas)

---

**Skapad: 2026-05-11**
**Build, Test & Run Guide för Clean Architecture Order Management**
