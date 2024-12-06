using Gravy.Domain.Entities;

namespace Gravy.Domain.Repositories;

public interface IDeliveryPersonAvailabilityRepository
{
    Task<List<DeliveryPersonAvailability>> GetAllAsync(CancellationToken 
        cancellationToken = default);
    void Add(DeliveryPersonAvailability deliveryPersonAvailability);
    void Update(DeliveryPersonAvailability deliveryPersonAvailability);
}