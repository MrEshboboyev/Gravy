using Gravy.Domain.Entities;

namespace Gravy.Domain.Repositories;

public interface IDeliveryRepository
{
    Task<List<Delivery>> GetAllAsync(CancellationToken cancellationToken = default);
    void Add(Delivery delivery);
    void Update(Delivery delivery);
}