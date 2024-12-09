using Gravy.Domain.Entities;

namespace Gravy.Application.Services.Deliveries;

public interface IDeliveryPersonSelector
{
    /// <summary>
    /// Selects the best delivery person for the given order.
    /// </summary>
    /// <param name="order">The order to assign a delivery person for.</param>
    /// <param name="cancellationToken">Cancellation token.</param>
    /// <returns>
    /// The best available delivery person or null if none are available.
    /// </returns>
    Task<DeliveryPerson?> SelectBestDeliveryPersonAsync(
        Order order,
        CancellationToken cancellationToken);
}
