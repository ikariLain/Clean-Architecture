namespace MyApp.Application.Mapping;

using MyApp.Application.DTOs;
using MyApp.Domain.Entities;
using MyApp.Domain.ValueObjects;

/// <summary>
/// Manuell mappning mellan entiteter och DTOs
/// 
/// Varför manuell mappning istället för AutoMapper?
/// - .NET 10 AOT-kompatibilitet
/// - Explicit och lätt att förstå vad som mappas
/// - Bättre prestanda utan reflection
/// 
/// Mappningsflödet:
/// Databas → Order (Entity) → OrderDto (DTO) → JSON Response
/// </summary>
public static class OrderMapper
{
    /// <summary>
    /// Mappa Order-entitet till OrderDto
    /// </summary>
    public static OrderDto ToDto(Order order)
    {
        return new OrderDto
        {
            Id = order.Id,
            OrderNumber = order.OrderNumber,
            Status = order.Status,
            CreatedAt = order.CreatedAt,
            UpdatedAt = order.UpdatedAt,
            TotalPrice = order.TotalPrice,
            TotalItems = order.TotalItems,
            ShippingAddress = AddressToDto(order.ShippingAddress),
            BillingAddress = AddressToDto(order.BillingAddress),
            Items = order.Items.Select(ItemToDto).ToList()
        };
    }

    /// <summary>
    /// Mappa OrderItem-entitet till OrderItemDto
    /// </summary>
    public static OrderItemDto ItemToDto(OrderItem item)
    {
        return new OrderItemDto
        {
            Id = item.Id,
            ProductId = item.ProductId,
            ProductName = item.ProductName,
            UnitPrice = item.UnitPrice,
            Quantity = item.Quantity,
            Total = item.GetTotal()
        };
    }

    /// <summary>
    /// Mappa Address-värdeobjekt till AddressDto
    /// </summary>
    public static AddressDto AddressToDto(Address address)
    {
        return new AddressDto
        {
            Street = address.Street,
            City = address.City,
            State = address.State,
            PostalCode = address.PostalCode,
            Country = address.Country
        };
    }

    /// <summary>
    /// Mappa AddressDto till Address-värdeobjekt
    /// </summary>
    public static Address DtoToAddress(AddressDto dto)
    {
        return Address.Create(
            dto.Street,
            dto.City,
            dto.State,
            dto.PostalCode,
            dto.Country);
    }
}
