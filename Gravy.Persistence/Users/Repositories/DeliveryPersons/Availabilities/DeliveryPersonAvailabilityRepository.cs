using Gravy.Domain.Entities;
using Gravy.Domain.Repositories;
using Microsoft.EntityFrameworkCore;

namespace Gravy.Persistence.Users.Repositories.DeliveryPersons.Availabilities;

public sealed class DeliveryPersonAvailabilityRepository(ApplicationDbContext dbContext)
    : IDeliveryPersonAvailabilityRepository
{
    private readonly ApplicationDbContext _dbContext = dbContext;

    public async Task<List<DeliveryPersonAvailability>> GetAllAsync(
        CancellationToken cancellationToken = default) =>
            await _dbContext
            .Set<DeliveryPersonAvailability>()
            .ToListAsync(cancellationToken);

    public async Task<IEnumerable<DeliveryPersonAvailability>> GetOverlappingAvailabilities(
        Guid deliveryPersonId,
        DateTime startTimeUtc,
        DateTime endTimeUtc)
    {
        return await _dbContext
            .Set<DeliveryPersonAvailability>()
            .Where(a => a.DeliveryPersonId == deliveryPersonId &&
                        (a.StartTimeUtc <= startTimeUtc && a.EndTimeUtc >= startTimeUtc || // Starts in the middle of an existing range
                         a.StartTimeUtc <= endTimeUtc && a.EndTimeUtc >= endTimeUtc ||    // Ends in the middle of an existing range
                         a.StartTimeUtc >= startTimeUtc && a.EndTimeUtc <= endTimeUtc))  // Completely overlaps an existing range
            .ToListAsync();
    }

    public void Add(DeliveryPersonAvailability deliveryPersonAvailability) =>
        _dbContext.Set<DeliveryPersonAvailability>().Add(deliveryPersonAvailability);

    public void Update(DeliveryPersonAvailability deliveryPersonAvailability) =>
        _dbContext.Set<DeliveryPersonAvailability>().Update(deliveryPersonAvailability);
}
