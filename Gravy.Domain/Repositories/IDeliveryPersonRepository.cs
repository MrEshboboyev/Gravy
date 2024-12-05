using Gravy.Domain.Entities;

namespace Gravy.Domain.Repositories;

public interface IDeliveryPersonRepository
{
    Task<List<DeliveryPerson>> GetAllAsync(CancellationToken cancellationToken = default);
    void Add(DeliveryPerson deliveryPerson);
    void Update(DeliveryPerson deliveryPerson);
}