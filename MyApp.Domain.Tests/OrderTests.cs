namespace MyApp.Domain.Tests;

using Xunit;
using MyApp.Domain.Entities;
using MyApp.Domain.ValueObjects;

/// <summary>
/// Unit Tests för Order-entiteten
///
/// Varför är detta viktigt?
/// - Visar att vi kan testa affärslogik på millisekunder
/// - INGEN databas behövs
/// - INGEN webserver behövs
/// - Endast ren domänlogik testas
///
/// Detta är en av de största fördelarna med Clean Architecture:
/// Snabba, determiniska tester av kärnlogiken
/// </summary>
public class OrderTests
{
    // Setup
    private readonly Address _validAddress = Address.Create(
        "Main Street 123",
        "Stockholm",
        "Stockholm",
        "10100",
        "Sweden");

    [Fact]
    public void Create_WithValidData_ShouldSucceed()
    {
        // Arrange & Act
        var order = Order.Create(
            "ORD-001",
            _validAddress,
            _validAddress);

        // Assert
        Assert.NotNull(order);
        Assert.Equal("ORD-001", order.OrderNumber);
        Assert.Equal(OrderStatus.Pending, order.Status);
        Assert.Empty(order.Items);
    }

    [Fact]
    public void Create_WithEmptyOrderNumber_ShouldThrow()
    {
        // Arrange, Act & Assert
        Assert.Throws<ArgumentException>(() =>
            Order.Create(string.Empty, _validAddress, _validAddress));
    }

    [Fact]
    public void Create_WithNullShippingAddress_ShouldThrow()
    {
        // Arrange, Act & Assert
        Assert.Throws<ArgumentNullException>(() =>
            Order.Create("ORD-001", null!, _validAddress));
    }

    [Fact]
    public void AddItem_WithValidData_ShouldAddItem()
    {
        // Arrange
        var order = Order.Create("ORD-001", _validAddress, _validAddress);

        // Act
        order.AddItem("PROD-001", "Widget", 99.99m, 2);

        // Assert
        Assert.Single(order.Items);
        Assert.Equal(99.99m * 2, order.TotalPrice);
        Assert.Equal(2, order.TotalItems);
    }

    [Fact]
    public void AddItem_WithNegativeQuantity_ShouldThrow()
    {
        // Arrange
        var order = Order.Create("ORD-001", _validAddress, _validAddress);

        // Act & Assert
        Assert.Throws<ArgumentException>(() =>
            order.AddItem("PROD-001", "Widget", 99.99m, -1));
    }

    [Fact]
    public void AddItem_WithNegativePrice_ShouldThrow()
    {
        // Arrange
        var order = Order.Create("ORD-001", _validAddress, _validAddress);

        // Act & Assert
        Assert.Throws<ArgumentException>(() =>
            order.AddItem("PROD-001", "Widget", -99.99m, 1));
    }

    [Fact]
    public void AddItem_ToShippedOrder_ShouldThrow()
    {
        // Arrange
        var order = Order.Create("ORD-001", _validAddress, _validAddress);
        order.AddItem("PROD-001", "Widget", 99.99m, 1);
        order.Confirm();
        order.Ship();

        // Act & Assert
        Assert.Throws<InvalidOperationException>(() =>
            order.AddItem("PROD-002", "Gadget", 49.99m, 1));
    }

    [Fact]
    public void RemoveItem_ShouldRemoveItem()
    {
        // Arrange
        var order = Order.Create("ORD-001", _validAddress, _validAddress);
        order.AddItem("PROD-001", "Widget", 99.99m, 2);
        order.AddItem("PROD-002", "Gadget", 49.99m, 1);

        // Act
        order.RemoveItem("PROD-001");

        // Assert
        Assert.Single(order.Items);
        Assert.Equal(49.99m, order.TotalPrice);
    }

    [Fact]
    public void Confirm_WithItems_ShouldSucceed()
    {
        // Arrange
        var order = Order.Create("ORD-001", _validAddress, _validAddress);
        order.AddItem("PROD-001", "Widget", 99.99m, 1);

        // Act
        order.Confirm();

        // Assert
        Assert.Equal(OrderStatus.Confirmed, order.Status);
    }

    [Fact]
    public void Confirm_WithoutItems_ShouldThrow()
    {
        // Arrange
        var order = Order.Create("ORD-001", _validAddress, _validAddress);

        // Act & Assert
        Assert.Throws<InvalidOperationException>(() => order.Confirm());
    }

    [Fact]
    public void Ship_ConfirmedOrder_ShouldSucceed()
    {
        // Arrange
        var order = Order.Create("ORD-001", _validAddress, _validAddress);
        order.AddItem("PROD-001", "Widget", 99.99m, 1);
        order.Confirm();

        // Act
        order.Ship();

        // Assert
        Assert.Equal(OrderStatus.Shipped, order.Status);
    }

    [Fact]
    public void Ship_UnconfirmedOrder_ShouldThrow()
    {
        // Arrange
        var order = Order.Create("ORD-001", _validAddress, _validAddress);

        // Act & Assert
        Assert.Throws<InvalidOperationException>(() => order.Ship());
    }

    [Fact]
    public void Cancel_PendingOrder_ShouldSucceed()
    {
        // Arrange
        var order = Order.Create("ORD-001", _validAddress, _validAddress);

        // Act
        order.Cancel();

        // Assert
        Assert.Equal(OrderStatus.Cancelled, order.Status);
    }

    [Fact]
    public void Cancel_ShippedOrder_ShouldThrow()
    {
        // Arrange
        var order = Order.Create("ORD-001", _validAddress, _validAddress);
        order.AddItem("PROD-001", "Widget", 99.99m, 1);
        order.Confirm();
        order.Ship();

        // Act & Assert
        Assert.Throws<InvalidOperationException>(() => order.Cancel());
    }

}
