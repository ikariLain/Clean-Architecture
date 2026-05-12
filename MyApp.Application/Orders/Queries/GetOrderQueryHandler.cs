namespace MyApp.Application.Orders.Queries;

using MediatR;
using MyApp.Application.DTOs;
using MyApp.Application.Mapping;
using MyApp.Domain.Repositories;

/// <summary>
/// Handler för GetOrderQuery
/// </summary>
public class GetOrderQueryHandler : IRequestHandler<GetOrderQuery, OrderDto?>
{
    private readonly IOrderRepository _orderRepository;

    public GetOrderQueryHandler(IOrderRepository orderRepository)
    {
        _orderRepository = orderRepository;
    }

    public async Task<OrderDto?> Handle(GetOrderQuery request, CancellationToken cancellationToken)
    {
        var order = await _orderRepository.GetByIdAsync(request.OrderId, cancellationToken);

        if (order == null)
            return null;

        return OrderMapper.ToDto(order);
    }
}
