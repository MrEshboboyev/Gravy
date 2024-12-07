using Gravy.Domain.Entities;
using Gravy.Domain.Repositories;
using Microsoft.EntityFrameworkCore;

namespace Gravy.Persistence.Users.Repositories.DeliveryPersons;

public sealed class DeliveryPersonRepository(ApplicationDbContext dbContext) : IDeliveryPersonRepository
{
    private readonly ApplicationDbContext _dbContext = dbContext;

    public async Task<List<DeliveryPerson>> GetAllAsync(
        CancellationToken cancellationToken = default) =>
            await _dbContext
            .Set<DeliveryPerson>()
            .ToListAsync(cancellationToken);

    public void Add(DeliveryPerson deliveryPerson) =>
        _dbContext.Set<DeliveryPerson>().Add(deliveryPerson);

    public void Update(DeliveryPerson deliveryPerson) =>
        _dbContext.Set<DeliveryPerson>().Update(deliveryPerson);
}
