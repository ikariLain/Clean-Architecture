namespace MyApp.Application.DTOs;

using MyApp.Domain.Entities;

/// <summary>
/// DTO för Order - denna exponeras till API:et, INTE entiteten
/// </summary>
public class OrderDto
{
    public Guid Id { get; set; }
    public string OrderNumber { get; set; } = string.Empty;
    public OrderStatus Status { get; set; }
    public DateTime CreatedAt { get; set; }
    public DateTime? UpdatedAt { get; set; }
    public decimal TotalPrice { get; set; }
    public int TotalItems { get; set; }
    public AddressDto ShippingAddress { get; set; } = null!;
    public AddressDto BillingAddress { get; set; } = null!;
    public List<OrderItemDto> Items { get; set; } = [];
}
