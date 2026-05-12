namespace MyApp.Application.Orders.Queries;

using MediatR;
using MyApp.Application.DTOs;

/// <summary>
/// GetOrderQuery - CQRS Query för att hämta en order
///
/// Query = något som INTE förändrar data (bara läser)
/// Queries returnerar data
///
/// Skillnad Query vs Command:
/// - Command: Skapar, uppdaterar, raderar (förändring)
/// - Query: Läser data (ingen förändring)
/// </summary>
public class GetOrderQuery : IRequest<OrderDto?>
{
    public Guid OrderId { get; set; }

    public GetOrderQuery(Guid orderId)
    {
        OrderId = orderId;
    }
}
