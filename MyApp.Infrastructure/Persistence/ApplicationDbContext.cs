namespace MyApp.Infrastructure.Persistence;

using Microsoft.EntityFrameworkCore;
using MyApp.Domain.Entities;
using MyApp.Infrastructure.Persistence.Configuration;

/// <summary>
/// ApplicationDbContext - EF Core DbContext
/// 
/// Denna klass är bron mellan domänen och databasen.
/// Den definierar vilka entiteter som ska sparas i databasen
/// och hur de mappas till tabeller.
/// </summary>
public class ApplicationDbContext : DbContext
{
    public DbSet<Order> Orders => Set<Order>();
    public DbSet<OrderItem> OrderItems => Set<OrderItem>();

    public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
        : base(options)
    {
    }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        // Använd Fluent API konfigurationer
        modelBuilder.ApplyConfiguration(new OrderConfiguration());
        modelBuilder.ApplyConfiguration(new OrderItemConfiguration());

        // Seed-data (valfritt, för testning)
        SeedData(modelBuilder);
    }

    private static void SeedData(ModelBuilder modelBuilder)
    {
        // Du kan lägga till seed-data här om det behövs
    }
}
