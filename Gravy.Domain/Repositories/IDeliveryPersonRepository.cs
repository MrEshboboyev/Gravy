using Gravy.Domain.Entities;

namespace Gravy.Domain.Repositories;

public interface IDeliveryPersonRepository
{
    void Add(DeliveryPerson deliveryPerson);
    void Update(DeliveryPerson deliveryPerson);
}