using MyApp.API.Endpoints;
using MyApp.API.Extensions;
using MyApp.Infrastructure;

var builder = WebApplication.CreateBuilder(args);

// ==================== COMPOSITION ROOT ====================
// Detta är där vi kopplar ihop alla beroenden (Dependency Injection)
// Clean Architecture-principen: Beroenden pekar inåt mot Domain

// 1. Registrera Application-services (MediatR och handlers)
builder.Services.AddApplication();

// 2. Registrera Infrastructure-services (EF Core, Repository, DbContext)
var connectionString = builder.Configuration.GetConnectionString("DefaultConnection")
    ?? "Server=(localdb)\\mssqllocaldb;Database=MyAppDb;Trusted_Connection=true;";
builder.Services.AddInfrastructure(connectionString);

// 3. Lägg till OpenAPI
builder.Services.AddOpenApi();

// ==================== BUILD APP ====================
var app = builder.Build();

// Configure the HTTP request pipeline
if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
}

app.UseHttpsRedirection();

// ==================== MAP ENDPOINTS ====================
// Registrera Order-endpoints
app.MapOrderEndpoints();

app.Run();
