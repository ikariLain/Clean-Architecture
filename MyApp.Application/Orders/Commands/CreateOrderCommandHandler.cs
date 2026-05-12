namespace MyApp.Application.Orders.Commands;

using MediatR;
using MyApp.Application.DTOs;
using MyApp.Application.Mapping;
using MyApp.Domain.Entities;
using MyApp.Domain.Repositories;

/// <summary>
/// Handler för CreateOrderCommand
///
/// Handler är klassen som faktiskt GÖR något när kommandot skickas.
/// Den är helt oberoende av HTTP/API-lagret.
/// Den kan testas enkelt utan att behöva mocka webserver eller databas.
/// </summary>
public class CreateOrderCommandHandler : IRequestHandler<CreateOrderCommand, OrderDto>
{
    private readonly IOrderRepository _orderRepository;

    public CreateOrderCommandHandler(IOrderRepository orderRepository)
    {
        _orderRepository = orderRepository;
    }

    public async Task<OrderDto> Handle(CreateOrderCommand request, CancellationToken cancellationToken)
    {
        // Validera att ordernummer inte redan existerar
        var existingOrder = await _orderRepository.GetByOrderNumberAsync(
            request.OrderNumber,
            cancellationToken);

        if (existingOrder != null)
            throw new InvalidOperationException(
                $"An order with number '{request.OrderNumber}' already exists.");

        // Mappa DTOs till Value Objects
        var shippingAddress = OrderMapper.DtoToAddress(request.ShippingAddress);
        var billingAddress = OrderMapper.DtoToAddress(request.BillingAddress);

        // Skapa ordern genom factory-metoden (detta tillämpar affärsregler)
        var order = Order.Create(
            request.OrderNumber,
            shippingAddress,
            billingAddress);

        // Lägg till varor
        foreach (var item in request.Items)
        {
            order.AddItem(item.ProductId, item.ProductName, item.Price, item.Quantity);
        }

        // Bekräfta ordern
        order.Confirm();

        // Spara i databas
        await _orderRepository.AddAsync(order, cancellationToken);
        await _orderRepository.SaveChangesAsync(cancellationToken);

        // Mappa tillbaka till DTO för response
        return OrderMapper.ToDto(order);
    }
}
