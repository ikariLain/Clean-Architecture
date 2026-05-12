namespace MyApp.API.Endpoints;

using MediatR;
using MyApp.Application.Orders.Commands;
using MyApp.Application.Orders.Queries;

/// <summary>
/// Endpoints för Order-hantering
/// 
/// Dessa är Minimal APIs - ett enkelt och modernt sätt att skapa endpoints i ASP.NET Core.
/// Varje endpoint är bara en mappning mellan HTTP och MediatR.
/// 
/// Flöde:
/// HTTP POST /api/orders → CreateOrderCommand → Handler → Databas → DTO Response
/// HTTP GET /api/orders/{id} → GetOrderQuery → Handler → Databas → DTO Response
/// HTTP DELETE /api/orders/{id} → DeleteOrderCommand → Handler → Databas → bool
/// </summary>
public static class OrderEndpoints
{
    public static void MapOrderEndpoints(this WebApplication app)
    {
        var group = app.MapGroup("/api/orders")
            .WithName("Orders");

        group.MapPost("/", CreateOrder)
            .WithName("CreateOrder")
            .WithDescription("Skapa en ny order");

        group.MapGet("/{id}", GetOrder)
            .WithName("GetOrder")
            .WithDescription("Hämta en order efter ID");

        group.MapDelete("/{id}", DeleteOrder)
            .WithName("DeleteOrder")
            .WithDescription("Radera en order");
    }

    /// <summary>
    /// POST /api/orders - Skapa en ny order
    /// </summary>
    private static async Task<IResult> CreateOrder(
        CreateOrderCommand command,
        IMediator mediator)
    {
        var result = await mediator.Send(command);
        return Results.Created($"/api/orders/{result.Id}", result);
    }

    /// <summary>
    /// GET /api/orders/{id} - Hämta en order
    /// </summary>
    private static async Task<IResult> GetOrder(
        Guid id,
        IMediator mediator)
    {
        var query = new GetOrderQuery(id);
        var result = await mediator.Send(query);

        return result == null
            ? Results.NotFound()
            : Results.Ok(result);
    }

    /// <summary>
    /// DELETE /api/orders/{id} - Radera en order
    /// </summary>
    private static async Task<IResult> DeleteOrder(
        Guid id,
        IMediator mediator)
    {
        var command = new DeleteOrderCommand(id);
        var result = await mediator.Send(command);

        return result
            ? Results.NoContent()
            : Results.NotFound();
    }
}
