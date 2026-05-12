namespace MyApp.Application.Orders.Commands;

using MediatR;

/// <summary>
/// DeleteOrderCommand - CQRS Command för att radera en order
///
/// Denna command är viktig för att demonstrera arkitektur-skillnader.
/// I en traditionell arkitektur skulle radering påverka många filer.
/// I Clean Architecture är det väl isolerat.
/// </summary>
public class DeleteOrderCommand : IRequest<bool>
{
    public Guid OrderId { get; set; }

    public DeleteOrderCommand(Guid orderId)
    {
        OrderId = orderId;
    }
}
