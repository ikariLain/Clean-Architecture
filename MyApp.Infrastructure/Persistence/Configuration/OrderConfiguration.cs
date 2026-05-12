namespace MyApp.Infrastructure.Persistence.Configuration;

using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using MyApp.Domain.Entities;
using MyApp.Domain.ValueObjects;

/// <summary>
/// EntityTypeConfiguration för Order
///
/// Fluent API konfigurar hur Order mappas till databastabeller.
/// Domän-klassen förblir ren från databasattribut (No Data Annotations).
///
/// Detta visar en av fördelarna med Clean Architecture:
/// Domänen är obunden från databaskonfiguration.
/// Du kan enkelt byta databas utan att ändra domänklassen.
/// </summary>
public class OrderConfiguration : IEntityTypeConfiguration<Order>
{
    public void Configure(EntityTypeBuilder<Order> builder)
    {
        // Primärnyckel
        builder.HasKey(o => o.Id);

        // Egenskaper
        builder.Property(o => o.Id)
            .ValueGeneratedNever();

        builder.Property(o => o.OrderNumber)
            .IsRequired()
            .HasMaxLength(50);

        builder.Property(o => o.Status)
            .IsRequired()
            .HasConversion<int>();

        builder.Property(o => o.CreatedAt)
            .IsRequired();

        builder.Property(o => o.UpdatedAt)
            .IsRequired(false);

        // Värdeobjekt - Address mappas som Own Types
        builder.OwnsOne(o => o.ShippingAddress, address =>
        {
            address.Property(a => a.Street).HasMaxLength(100).IsRequired();
            address.Property(a => a.City).HasMaxLength(50).IsRequired();
            address.Property(a => a.State).HasMaxLength(50).IsRequired();
            address.Property(a => a.PostalCode).HasMaxLength(20).IsRequired();
            address.Property(a => a.Country).HasMaxLength(50).IsRequired();
            address.ToTable("OrderAddresses", table => table.ExcludeFromMigrations());
        });

        builder.OwnsOne(o => o.BillingAddress, address =>
        {
            address.Property(a => a.Street).HasMaxLength(100).IsRequired();
            address.Property(a => a.City).HasMaxLength(50).IsRequired();
            address.Property(a => a.State).HasMaxLength(50).IsRequired();
            address.Property(a => a.PostalCode).HasMaxLength(20).IsRequired();
            address.Property(a => a.Country).HasMaxLength(50).IsRequired();
            address.ToTable("OrderAddresses", table => table.ExcludeFromMigrations());
        });

        // Relationer - Items
        builder.HasMany(o => o.Items)
            .WithOne()
            .HasForeignKey("OrderId");

        // Index för snabbare sökningar
        builder.HasIndex(o => o.OrderNumber)
            .IsUnique();

        // Tabell-namn
        builder.ToTable("Orders");
    }
}
