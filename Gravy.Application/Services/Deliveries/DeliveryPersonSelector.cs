using Gravy.Domain.Entities;
using Gravy.Domain.Repositories;

namespace Gravy.Application.Services.Deliveries;

public sealed class DeliveryPersonSelector(IDeliveryPersonRepository 
    deliveryPersonRepository) : IDeliveryPersonSelector
{
    private readonly IDeliveryPersonRepository _deliveryPersonRepository = 
        deliveryPersonRepository;

    public async Task<DeliveryPerson?> SelectBestDeliveryPersonAsync(
        Order order,
        CancellationToken cancellationToken)
    {
        // Fetch all available delivery persons
        var availableDeliveryPersons = await _deliveryPersonRepository
            .GetAllAsync(cancellationToken);

        // Extract Location from DeliveryAddress
        var deliveryLocation = order.DeliveryAddress.ToLocation();

        // Filter delivery persons by distance to the order's delivery address
        var candidates = availableDeliveryPersons
            .Where(dp => dp.IsAvailableForDelivery(deliveryLocation))
            .OrderBy(dp => dp.DistanceTo(deliveryLocation));

        // Return the closest candidate, if available
        return candidates.FirstOrDefault();
    }
}
