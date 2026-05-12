namespace MyApp.Application.Orders.Commands;

using MediatR;
using MyApp.Application.DTOs;

/// <summary>
/// CreateOrderCommand - CQRS Command för att skapa en ny order
///
/// Command = något som ÄR FÖRÄNDRING (Create, Update, Delete)
/// Commands returnerar vanligtvis ett resultat eller ID
///
/// MediatR matcherar denna command till motsvarande handler
/// </summary>
public class CreateOrderCommand : IRequest<OrderDto>
{
    public required string OrderNumber { get; set; }
    public required AddressDto ShippingAddress { get; set; }
    public required AddressDto BillingAddress { get; set; }
    public required List<CreateOrderItemRequest> Items { get; set; }
}

/// <summary>
/// Request-objekt för OrderItem vid orderkreation
/// </summary>
public class CreateOrderItemRequest
{
    public required string ProductId { get; set; }
    public required string ProductName { get; set; }
    public decimal Price { get; set; }
    public int Quantity { get; set; }
}
