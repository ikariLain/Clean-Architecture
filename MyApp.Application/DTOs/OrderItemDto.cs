namespace MyApp.Application.DTOs;

/// <summary>
/// DTO för OrderItem - används för att överföra data utan att exponera entiteten
/// </summary>
public class OrderItemDto
{
    public Guid Id { get; set; }
    public string ProductId { get; set; } = string.Empty;
    public string ProductName { get; set; } = string.Empty;
    public decimal UnitPrice { get; set; }
    public int Quantity { get; set; }
    public decimal Total { get; set; }
}
