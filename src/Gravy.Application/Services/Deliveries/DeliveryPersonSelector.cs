using Gravy.Domain.Entities;
using Gravy.Domain.Repositories;

namespace Gravy.Application.Services.Deliveries;

public sealed class DeliveryPersonSelector(IDeliveryPersonRepository 
    deliveryPersonRepository) : IDeliveryPersonSelector
{
    private readonly IDeliveryPersonRepository _deliveryPersonRepository = 
        deliveryPersonRepository;

    public async Task<DeliveryPerson> SelectBestDeliveryPersonAsync(
        Order order,
        CancellationToken cancellationToken)
    {
        // Fetch all available delivery persons
        var allDeliveryPersons = await _deliveryPersonRepository
            .GetAllAsync(cancellationToken);

        // Extract Location from DeliveryAddress
        var deliveryLocation = order.DeliveryAddress.ToLocation();

        #region Get Available For Delivery Persons

        var availablePersonsForDelivery = allDeliveryPersons
            .Where(dp => dp.IsAvailableForDelivery(deliveryLocation));

        #endregion

        #region Checking isAvailable in this moment

        var availableDeliveryPersonsAtTheMoment 
            = availablePersonsForDelivery
                .Where(dp => dp.IsAvailableAt(DateTime.UtcNow));

        #endregion

        #region Filter delivery persons by distance to the order's delivery address
        
        var candidates = availableDeliveryPersonsAtTheMoment
            .OrderBy(dp => dp.DistanceTo(deliveryLocation));

        #endregion

        #region Get best candidate

        var bestCandidate = candidates.FirstOrDefault();

        #endregion

        #region Checking best candidate is null

        if (bestCandidate is null) return bestCandidate;

        #endregion

        #region Update this best candidate and return

        bestCandidate.SetAvailability(false);
        _deliveryPersonRepository.Update(bestCandidate);

        // Return the closest candidate, if available
        return bestCandidate;

        #endregion
    }
}
