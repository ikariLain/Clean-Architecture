namespace MyApp.Application.Orders.Commands;

using MediatR;
using MyApp.Domain.Repositories;

/// <summary>
/// Handler för DeleteOrderCommand
/// </summary>
public class DeleteOrderCommandHandler : IRequestHandler<DeleteOrderCommand, bool>
{
    private readonly IOrderRepository _orderRepository;

    public DeleteOrderCommandHandler(IOrderRepository orderRepository)
    {
        _orderRepository = orderRepository;
    }

    public async Task<bool> Handle(DeleteOrderCommand request, CancellationToken cancellationToken)
    {
        var order = await _orderRepository.GetByIdAsync(request.OrderId, cancellationToken);

        if (order == null)
            return false;

        // Affärsregel: Kan bara radera order som inte är skickade
        if (order.Status == Domain.Entities.OrderStatus.Shipped)
            throw new InvalidOperationException(
                "Cannot delete an order that has been shipped");

        await _orderRepository.DeleteAsync(request.OrderId, cancellationToken);
        await _orderRepository.SaveChangesAsync(cancellationToken);

        return true;
    }
}
