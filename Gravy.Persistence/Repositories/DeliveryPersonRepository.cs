using Gravy.Domain.Entities;
using Gravy.Domain.Repositories;

namespace Gravy.Persistence.Repositories;

public sealed class DeliveryPersonRepository(ApplicationDbContext dbContext) : IDeliveryPersonRepository
{
    private readonly ApplicationDbContext _dbContext = dbContext;

    public void Add(DeliveryPerson deliveryPerson) =>
        _dbContext.Set<DeliveryPerson>().Add(deliveryPerson);

    public void Update(DeliveryPerson deliveryPerson) =>
        _dbContext.Set<DeliveryPerson>().Update(deliveryPerson);
}
