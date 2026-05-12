# 🎤 Presentationsguide - Clean Architecture Order Management

## 📋 Agenda (30-45 minuter)

### 1. Introduktion (5 min)
   - Vad är Clean Architecture?
   - Varför är det viktigt?
   - Vad ska vi demonstrera?

### 2. Arkitektur & Design (10 min)
   - Visa lager-diagrammet
   - Förklara "The Dependency Rule"
   - Visa projektstruturen

### 3. Domain-logik (8 min)
   - Visa Order-entiteten
   - Förklara Value Objects (Address)
   - Visa affärsreglerna

### 4. CQRS & Endpoints (5 min)
   - Visa Commands och Queries
   - Förklara MediatR
   - Visa GetOrder-mappningen

### 5. Testing & Demo (7 min)
   - Köra Unit Tests (0.6s)
   - Live-demo av API
   - Testa CreateOrder endpoint

### 6. Slutsats & Discussion (5 min)
   - Key Takeaways
   - Trade-offs
   - Frågor

---

## 🖥️ Demonstration Checklist

### Före presentationen:
- [ ] Öppna VS Code med projektet
- [ ] Öppna terminal
- [ ] Öppna README.md i preview
- [ ] Öppna ARCHITECTURE.md ready
- [ ] Öppna Order.cs för visa affärsreglerna

### Under presentationen:

#### 1. Show Code Structure
```bash
# Visa mapstrukturen
ls -la MyApp.*
# Eller i VS Code: Explorer-vyn
```

#### 2. Show Domain Logic
```
Öppna: MyApp.Domain/Entities/Order.cs
- Visa affärsreglerna (lines 35-55)
- Visa AddItem-metoden (lines 73-105)
- Förklara att detta är rent C#, INGEN databaskod
```

#### 3. Run Unit Tests
```bash
dotnet test MyApp.Domain.Tests
```
**Resultat ska visa:** 20 tests, 0.6s runtime

#### 4. Show API Endpoints
```
Öppna: MyApp.API/Endpoints/OrderEndpoints.cs
- Visa GetOrderQuery-flow (line 48)
```

#### 5. Build Project
```bash
dotnet build
```
**Resultat:** Build succeeded

#### 6. Start API Server
```bash
cd MyApp.API
dotnet run
```
**Output:**
```
Now listening on: https://localhost:7001
```

#### 7. Test API Endpoints
**Option A: Via cURL**
```bash
# Create Order
curl -X POST "https://localhost:7001/api/orders" \
  -H "Content-Type: application/json" \
  -d '{"orderNumber":"ORD-001",...}'

# Get Order
curl -X GET "https://localhost:7001/api/orders/{id}"

# Delete Order
curl -X DELETE "https://localhost:7001/api/orders/{id}"
```

**Option B: Via Swagger UI**
1. Öppna: `https://localhost:7001/openapi/v1.json`
2. Interact with Swagger UI
3. Test CreateOrder
4. Test GetOrder
5. Test DeleteOrder

---

## 📊 Key Slides/Talking Points

### Slide 1: "Vad är Clean Architecture?"
```
Definition:
- Arkitektur-mönster för att organisera kod
- Centrum = affärslogik (Domain)
- Utanrunt = teknik (databas, HTTP, UI)
- Beroenden pekar INÅT

Fördelar:
✓ Lätt att testa
✓ Lätt att underhålla
✓ Lätt att byta teknologi
✓ Fokus på affärsregler
```

### Slide 2: "The Dependency Rule"
```
           ┌───────────────┐
           │   API Layer   │ (HTTP)
           ├───────────────┤
           │  Application  │ (CQRS)
           ├───────────────┤
           │    Domain     │ (Business)
           ├───────────────┤
           │Infrastructure │ (Data)
           └───────────────┘

Regel: Beroenden pekar alltid INÅT → Domain
Domain kan ALDRIG bero på något annat!
```

### Slide 3: "Order Entity - Rich Domain Model"
```csharp
// INLÄGG: Affärslogik ligger i entiteten
public void AddItem(string productId, string name,
    decimal price, int quantity)
{
    // Affärsregel: Kan inte lägga till vara till skickad order
    if (Status == OrderStatus.Shipped)
        throw new InvalidOperationException(...);

    // Affärsregel: Kvantitet måste vara positiv
    if (quantity <= 0)
        throw new ArgumentException(...);
}

// POÄNG: Logiken är skyddad och validerad
```

### Slide 4: "CQRS Pattern"
```
Command = något som förändrar data
  ↓
CreateOrderCommand
  ↓
CreateOrderCommandHandler
  ↓
Order.Create() + SaveToDB()

Query = något som läser data
  ↓
GetOrderQuery
  ↓
GetOrderQueryHandler
  ↓
Order.FindById() + MapToDTO()
```

### Slide 5: "DTO Mapping - Two-Way Conversion"
```
Database Row (SQL)
       ↓
Order Entity (C# with business logic)
       ↓
OrderDto (Safe, API-friendly)
       ↓
JSON Response to Client

Fördelar:
✓ Entiteten exponeras INTE
✓ API kan ändras utan att ändra entiteten
✓ Säkerhet: känslig data kan filtreras
✓ Flexibilitet: beräknade egenskaper
```

### Slide 6: "DeleteOrder - Ceremony Count"
```
Hur många filer behövde vi ändra för DeleteOrder?

Clean Architecture:
  - DeleteOrderCommand.cs (1 ny fil)
  - DeleteOrderCommandHandler.cs (1 ny fil)
  - OrderEndpoints.cs (1 modifierad fil)
  - (Repository redan hade DeleteAsync)

TOTALT: 2-3 filer

Varför är det bra?
✓ Ändringar är isolerade
✓ Risk för regression är låg
✓ Enkelt att testa
```

### Slide 7: "Testing - Fast & Isolated"
```
Unit Tests för Domain:
  ✓ 20 tests
  ✓ 0.6 sekunder
  ✓ ZERO database access
  ✓ ZERO HTTP calls
  ✓ ZERO external dependencies

Vad är det vi testar?
✓ Order-skapande
✓ Affärsregelvalidering
✓ Status-transitions
✓ Beräkningar
✓ Value Objects
```

---

## 💬 Svar på potentiella frågor

### F: "Varför inte bara en mapp med allting (VSA)?"
**A:** Både CA och VSA är giltiga. CA är bättre för:
- Större projekt med komplex affärslogik
- Teams med flera utvecklare
- Projekt som förväntas växa
VSA är bättre för:
- Enkla CRUD-operationer
- Små projekt
- Snabb MVP

### F: "Varför så många lager?"
**A:** Separation of concerns. För små projekt kan man slå ihop.
I detta exempel visar vi principerna - produktionskod kan variera.

### F: "MediatR - är det inte overengineering?"
**A:** För denna applikation: möjligt. Men MediatR erbjuder:
- Tydlig request-handling
- Enkelt att lägga till pipelines (validation, logging)
- CQRS-pattern stöd
- Testing-friendly

### F: "Varför DTO:er?"
**A:**
1. Säkerhet - exponera inte entiteter
2. Flexibilitet - API kan ändras oberoende
3. Kontroll - använd bara de fält du behöver
4. Performance - beräkna bara vad som krävs

### F: "Hur testar du mot databasen?"
**A:** Det gör vi inte här (Unit Tests). Integration Tests skulle:
- Starta en TestContainer med SQL
- Köra Handlers med faktisk EF Core
- Verifiera databaspersistens

### F: "Vad är nästa steg?"
**A:**
- Integration Tests (Layer 2)
- Logging & Monitoring
- Caching
- Validation (FluentValidation)
- Authentication & Authorization

---

## 🎬 Live Demo Script

### Demo 1: Build & Test (2 min)
```bash
# 1. Build
$ dotnet build
# → Build succeeded with 5 warnings

# 2. Test
$ dotnet test MyApp.Domain.Tests
# → 20 tests, 0.6s, all passed

# 3. FÖRKLARING:
# "Vi kör 20 tests på 600ms utan att röra databasen.
#  Det är testning av ren affärslogik."
```

### Demo 2: Run API Server (30 sec)
```bash
# 1. Navigate
$ cd MyApp.API
$ dotnet run

# 2. Öppna i webbläsare
$ https://localhost:7001/openapi/v1.json

# FÖRKLARING:
# "API:et startar på port 7001 och exponerar
#  3 endpoints via Swagger UI."
```

### Demo 3: Test Endpoints (3 min)
```bash
# 1. CREATE ORDER
POST /api/orders
{
  "orderNumber": "ORD-2026-DEMO",
  "shippingAddress": {...},
  "billingAddress": {...},
  "items": [{"productId": "PROD-001", ...}]
}
# → Response: 201 Created + OrderDto

# 2. GET ORDER
GET /api/orders/{id-from-create}
# → Response: 200 OK + OrderDto with all details

# 3. DELETE ORDER
DELETE /api/orders/{id}
# → Response: 204 No Content

# FÖRKLARING:
# "All data goes through MediatR handlers.
#  Handlers enforce business rules.
#  DTOs are returned, not entities."
```

### Demo 4: Show Code (2 min)
```csharp
// Öppna Order.cs - visa:
// 1. AddItem metoden (affärsregler)
// 2. Confirm metoden (status-transition)
// 3. DeleteOrder validation (kan inte radera shipped)

// FÖRKLARING:
// "Affärslogiken ligger här, inte i databasen.
//  Om regel behöver ändras - ändra här!"
```

---

## 📸 Screenshots att visa

1. **Mapstruktur** - Visa VS Code Explorer
2. **Order.cs** - Affärsregler
3. **OrderEndpoints.cs** - HTTP mappings
4. **Unit Tests Result** - 20/20 passed
5. **Swagger UI** - API documentation
6. **ARCHITECTURE.md** - Diagram

---

## ⏱️ Timing Guide

| Segment | Tid | Innehål |
|---------|-----|---------|
| Intro | 5 min | Vad, varför, vad ska vi se |
| Architecture | 10 min | Lager, The Rule, projektstuktur |
| Domain | 8 min | Entities, Value Objects, Rules |
| CQRS | 5 min | Commands, Queries, MediatR |
| Testing | 3 min | Unit Tests result (run live) |
| Demo | 5 min | API Server + Endpoints |
| Code Review | 2 min | Show Order.cs + OrderEndpoints |
| Q&A | 5-7 min | Frågor |
| **TOTALT** | **43-45 min** | |

---

## 🎯 Key Takeaways (Slide At End)

```
✓ Clean Architecture = Affärslogik i centrum
✓ The Dependency Rule = Beroenden pekar inåt
✓ Rich Domain Model = Logik ligger i entiteter
✓ CQRS = Tydlig separation av läs/skrivoperationer
✓ Testing = Enkelt utan databas
✓ DTOs = Skyddar entiteter från exponering
✓ Flexibility = Enkelt att byta databas/UI
✓ Scalability = Passar små och stora projekt

Det handlar inte om antal filer.
Det handlar om ARKITEKTUR-PRINCIPER.
```

---

## 💡 Pro Tips

1. **Prova inte att detaljförklara alla 500 rader i Order.cs**
   - Fokusera på 2-3 affärsregler
   - Visa bara AddItem och Confirm

2. **Gör gärna demo INTERAKTIVT**
   - Låt åhörare ställa frågor under vägen
   - Klicka på saker live om möjligt

3. **Jämför med något de känner till**
   - "VSA hade allt i en mapp"
   - "Traditionell MVC hade controllers med affärslogik"

4. **Visa REAL CODE, inte Powerpoint**
   - Öppna VS Code
   - Visa ARCHITECTURE.md
   - Visa test-resultat live

5. **Avsluta med att säga nästa steg**
   - Integration Tests
   - Logging
   - Real deployment story

---

## ✅ Pre-Presentation Checklist

- [ ] `dotnet build` - Verify no errors
- [ ] `dotnet test MyApp.Domain.Tests` - 20/20 passing
- [ ] `dotnet run` från MyApp.API - Server starts
- [ ] `https://localhost:7001/openapi` - Swagger loads
- [ ] VS Code öppen med projektet
- [ ] README.md i preview redo
- [ ] ARCHITECTURE.md i preview redo
- [ ] Internet connection (for screenshots/live demos)
- [ ] Terminal med git ready (för att visa commits om relevant)

---

## 🎁 Att Dela Med Åhörarna

```
Dokumentation Files:
- README.md - Quick start
- ARCHITECTURE.md - Detailed architecture
- FILE_STRUCTURE.md - File overview
- BUILD_AND_RUN.md - Build instructions
- IMPLEMENTATION_SUMMARY.md - What was built

GitHub (om repo är public):
- Link till repository
- Instructions för att klona
- Commands för att bygga
```

---

**Lycka till med presentationen! 🚀**

*Denna guide skapad för "Clean Architecture - Order Management"*
*Presentationslängd: 45 minuter*
*Demo time: ~5-10 minuter*
