using Gravy.Domain.Entities;

namespace Gravy.Domain.Repositories;

public interface IDeliveryPersonAvailabilityRepository
{
    Task<List<DeliveryPersonAvailability>> GetAllAsync(CancellationToken 
        cancellationToken = default);
    Task<IEnumerable<DeliveryPersonAvailability>> GetOverlappingAvailabilities(
        Guid deliveryPersonId,
        DateTime startTimeUtc,
        DateTime endTimeUtc);
    void Add(DeliveryPersonAvailability deliveryPersonAvailability);
    void Update(DeliveryPersonAvailability deliveryPersonAvailability);
}