namespace MyApp.Infrastructure;

using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using MyApp.Domain.Repositories;
using MyApp.Infrastructure.Persistence;
using MyApp.Infrastructure.Persistence.Repositories;

/// <summary>
/// Extension-metoder för att registrera Infrastructure-services
/// 
/// Detta är ett vanligt mönster för att hålla Dependency Injection
/// konfigurationen organiserad och återanvändbar.
/// 
/// Användning i Program.cs:
/// services.AddInfrastructure("connection string");
/// </summary>
public static class InfrastructureServiceExtensions
{
    public static IServiceCollection AddInfrastructure(
        this IServiceCollection services,
        string connectionString)
    {
        // Registrera DbContext
        services.AddDbContext<ApplicationDbContext>(options =>
            options.UseSqlServer(connectionString));

        // Registrera Repository
        services.AddScoped<IOrderRepository, OrderRepository>();

        return services;
    }
}
