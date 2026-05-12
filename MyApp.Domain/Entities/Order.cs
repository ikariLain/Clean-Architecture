namespace MyApp.Domain.Entities;

using MyApp.Domain.ValueObjects;

/// <summary>
/// Order-entitet med rik affärslogik (DDD - Rich Domain Model)
/// Denna klass ansvarar för sin egen affärsregler och validering
/// </summary>
public class Order
{
    // Privat konstruktor för att styra skapandet
    private Order() { }

    public Guid Id { get; private set; }
    public string OrderNumber { get; private set; } = string.Empty;
    public DateTime CreatedAt { get; private set; }
    public DateTime? UpdatedAt { get; private set; }
    public OrderStatus Status { get; private set; } = OrderStatus.Pending;

    // Value Object - Address är ett värdeobjekt som representerar en adress
    public required Address ShippingAddress { get; init; }
    public required Address BillingAddress { get; init; }

    // Relationer
    private readonly List<OrderItem> _items = [];
    public IReadOnlyCollection<OrderItem> Items => _items.AsReadOnly();

    // Business logic - beräkna totalpris direkt i entiteten
    public decimal TotalPrice => _items.Sum(item => item.GetTotal());
    public int TotalItems => _items.Sum(item => item.Quantity);

    /// <summary>
    /// Factory-metod för att skapa en ny order
    /// Denna metod tillämpar alla affärsregler för orderkreation
    /// </summary>
    public static Order Create(
        string orderNumber,
        Address shippingAddress,
        Address billingAddress)
    {
        // Affärsregel: Ordernummer kan inte vara tomt
        if (string.IsNullOrWhiteSpace(orderNumber))
            throw new ArgumentException("Order number cannot be empty", nameof(orderNumber));

        // Affärsregel: Adresser måste vara giltiga
        if (shippingAddress == null)
            throw new ArgumentNullException(nameof(shippingAddress));

        if (billingAddress == null)
            throw new ArgumentNullException(nameof(billingAddress));

        return new Order
        {
            Id = Guid.NewGuid(),
            OrderNumber = orderNumber,
            CreatedAt = DateTime.UtcNow,
            Status = OrderStatus.Pending,
            ShippingAddress = shippingAddress,
            BillingAddress = billingAddress
        };
    }

    /// <summary>
    /// Lägg till en vara till ordern
    /// Denna metod tillämpar affärsregler för att lägga till varor
    /// </summary>
    public void AddItem(string productId, string productName, decimal price, int quantity)
    {
        // Affärsregel: Kan inte lägga till varor till en skickad eller avbruten order
        if (Status == OrderStatus.Shipped || Status == OrderStatus.Cancelled)
            throw new InvalidOperationException(
                $"Cannot add items to order with status '{Status}'");

        // Affärsregel: Quantity måste vara större än 0
        if (quantity <= 0)
            throw new ArgumentException("Quantity must be greater than 0", nameof(quantity));

        // Affärsregel: Pris måste vara större än eller lika med 0
        if (price < 0)
            throw new ArgumentException("Price cannot be negative", nameof(price));

        var existingItem = _items.FirstOrDefault(i => i.ProductId == productId);

        if (existingItem != null)
        {
            // Uppdatera befintlig vara
            existingItem.UpdateQuantity(existingItem.Quantity + quantity);
        }
        else
        {
            // Lägg till ny vara
            var newItem = OrderItem.Create(productId, productName, price, quantity);
            _items.Add(newItem);
        }

        UpdatedAt = DateTime.UtcNow;
    }

    /// <summary>
    /// Ta bort en vara från ordern
    /// </summary>
    public void RemoveItem(string productId)
    {
        // Affärsregel: Kan inte ta bort varor från en skickad eller avbruten order
        if (Status == OrderStatus.Shipped || Status == OrderStatus.Cancelled)
            throw new InvalidOperationException(
                $"Cannot remove items from order with status '{Status}'");

        var item = _items.FirstOrDefault(i => i.ProductId == productId);
        if (item != null)
        {
            _items.Remove(item);
            UpdatedAt = DateTime.UtcNow;
        }
    }

    /// <summary>
    /// Bekräfta ordern
    /// Affärsregel: Ordern måste innehålla minst en vara
    /// </summary>
    public void Confirm()
    {
        if (_items.Count == 0)
            throw new InvalidOperationException("Cannot confirm an order without items");

        if (Status != OrderStatus.Pending)
            throw new InvalidOperationException("Only pending orders can be confirmed");

        Status = OrderStatus.Confirmed;
        UpdatedAt = DateTime.UtcNow;
    }

    /// <summary>
    /// Skicka ordern
    /// </summary>
    public void Ship()
    {
        if (Status != OrderStatus.Confirmed)
            throw new InvalidOperationException("Only confirmed orders can be shipped");

        Status = OrderStatus.Shipped;
        UpdatedAt = DateTime.UtcNow;
    }

    /// <summary>
    /// Avbryt ordern
    /// </summary>
    public void Cancel()
    {
        if (Status == OrderStatus.Shipped || Status == OrderStatus.Cancelled)
            throw new InvalidOperationException(
                $"Cannot cancel order with status '{Status}'");

        Status = OrderStatus.Cancelled;
        UpdatedAt = DateTime.UtcNow;
    }
}
