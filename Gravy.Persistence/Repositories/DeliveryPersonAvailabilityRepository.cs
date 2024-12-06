using Gravy.Domain.Entities;
using Gravy.Domain.Repositories;
using Microsoft.EntityFrameworkCore;

namespace Gravy.Persistence.Repositories;

public sealed class DeliveryPersonAvailabilityRepository(ApplicationDbContext dbContext) 
    : IDeliveryPersonAvailabilityRepository
{
    private readonly ApplicationDbContext _dbContext = dbContext;

    public async Task<List<DeliveryPersonAvailability>> GetAllAsync(
        CancellationToken cancellationToken = default) =>
            await _dbContext
            .Set<DeliveryPersonAvailability>()
            .ToListAsync(cancellationToken);

    public void Add(DeliveryPersonAvailability deliveryPersonAvailability) =>
        _dbContext.Set<DeliveryPersonAvailability>().Add(deliveryPersonAvailability);

    public void Update(DeliveryPersonAvailability deliveryPersonAvailability) =>
        _dbContext.Set<DeliveryPersonAvailability>().Update(deliveryPersonAvailability);
}
