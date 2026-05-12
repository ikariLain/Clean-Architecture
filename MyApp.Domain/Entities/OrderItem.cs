namespace MyApp.Domain.Entities;

/// <summary>
/// OrderItem-entitet som representerar en vara i en order
/// </summary>
public class OrderItem
{
    private OrderItem() { }

    public Guid Id { get; private set; }
    public string ProductId { get; private set; } = string.Empty;
    public string ProductName { get; private set; } = string.Empty;
    public decimal UnitPrice { get; private set; }
    public int Quantity { get; private set; }

    /// <summary>
    /// Beräkna totalpris för denna vara
    /// </summary>
    public decimal GetTotal() => UnitPrice * Quantity;

    /// <summary>
    /// Factory-metod för att skapa en ny orderitem
    /// </summary>
    public static OrderItem Create(string productId, string productName, decimal price, int quantity)
    {
        if (string.IsNullOrWhiteSpace(productId))
            throw new ArgumentException("Product ID cannot be empty", nameof(productId));

        if (string.IsNullOrWhiteSpace(productName))
            throw new ArgumentException("Product name cannot be empty", nameof(productName));

        if (price < 0)
            throw new ArgumentException("Price cannot be negative", nameof(price));

        if (quantity <= 0)
            throw new ArgumentException("Quantity must be greater than 0", nameof(quantity));

        return new OrderItem
        {
            Id = Guid.NewGuid(),
            ProductId = productId,
            ProductName = productName,
            UnitPrice = price,
            Quantity = quantity
        };
    }

    /// <summary>
    /// Uppdatera kvantiteten för denna vara
    /// </summary>
    public void UpdateQuantity(int newQuantity)
    {
        if (newQuantity <= 0)
            throw new ArgumentException("Quantity must be greater than 0", nameof(newQuantity));

        Quantity = newQuantity;
    }
}
