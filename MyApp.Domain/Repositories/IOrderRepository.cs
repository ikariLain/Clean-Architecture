namespace MyApp.Domain.Repositories;

using MyApp.Domain.Entities;

/// <summary>
/// IOrderRepository - Ett kontrakt som definierar vad vi kan göra med Order-data
///
/// Denna interface är en del av domänen och definerar vad en repository MÅSTE kunna göra.
/// Det är viktigt att detta ligger i Domain, INTE i Infrastructure.
///
/// Anledning: Domain får ALDRIG bero på Infrastructure.
/// Infrastructure implementerar detta kontrakt, men Domain vet ingenting om SQL, EF Core, etc.
///
/// Detta är en av de viktigaste arkitektur-principerna i Clean Architecture.
/// </summary>
public interface IOrderRepository
{
    /// <summary>
    /// Hämta en order efter ID
    /// </summary>
    Task<Order?> GetByIdAsync(Guid orderId, CancellationToken cancellationToken = default);

    /// <summary>
    /// Hämta en order efter ordernummer
    /// </summary>
    Task<Order?> GetByOrderNumberAsync(string orderNumber, CancellationToken cancellationToken = default);

    /// <summary>
    /// Lägg till en ny order
    /// </summary>
    Task AddAsync(Order order, CancellationToken cancellationToken = default);

    /// <summary>
    /// Uppdatera en befintlig order
    /// </summary>
    Task UpdateAsync(Order order, CancellationToken cancellationToken = default);

    /// <summary>
    /// Radera en order
    /// </summary>
    Task DeleteAsync(Guid orderId, CancellationToken cancellationToken = default);

    /// <summary>
    /// Hämta alla orders
    /// </summary>
    Task<IEnumerable<Order>> GetAllAsync(CancellationToken cancellationToken = default);

    /// <summary>
    /// Spara alla ändringar
    /// </summary>
    Task SaveChangesAsync(CancellationToken cancellationToken = default);
}
