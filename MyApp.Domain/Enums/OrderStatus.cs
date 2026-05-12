namespace MyApp.Domain.Entities;

/// <summary>
/// OrderStatus - Enum som representerar möjliga statar för en order
/// </summary>
public enum OrderStatus
{
    Pending = 0,
    Confirmed = 1,
    Shipped = 2,
    Cancelled = 3
}
