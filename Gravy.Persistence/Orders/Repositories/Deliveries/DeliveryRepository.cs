using Gravy.Domain.Entities;
using Gravy.Domain.Repositories;
using Microsoft.EntityFrameworkCore;

namespace Gravy.Persistence.Orders.Repositories.Deliveries;

public sealed class DeliveryRepository(ApplicationDbContext dbContext) 
    : IDeliveryRepository
{
    private readonly ApplicationDbContext _dbContext = dbContext;

    public async Task<List<Delivery>> GetAllAsync(CancellationToken cancellationToken) =>
        await _dbContext
            .Set<Delivery>()
            .ToListAsync(cancellationToken);

    public void Add(Delivery menuItem) =>
        _dbContext.Set<Delivery>().Add(menuItem);

    public void Update(Delivery menuItem) =>
        _dbContext.Set<Delivery>().Update(menuItem);
}
