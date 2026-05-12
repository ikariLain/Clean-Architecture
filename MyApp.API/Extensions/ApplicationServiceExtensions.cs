namespace MyApp.API.Extensions;

using MediatR;

/// <summary>
/// Extension för att registrera Application-services
/// </summary>
public static class ApplicationServiceExtensions
{
    public static IServiceCollection AddApplication(this IServiceCollection services)
    {
        // Registrera MediatR med alla handlers från Application-projektet
        services.AddMediatR(cfg =>
            cfg.RegisterServicesFromAssemblies(
                typeof(ApplicationServiceExtensions).Assembly,
                // Referera till ett typ från Application-projektet
                typeof(MyApp.Application.Orders.Commands.CreateOrderCommand).Assembly));

        return services;
    }
}
