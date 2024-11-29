using Gravy.Domain.Entities;
using Gravy.Domain.Repositories;
using Microsoft.EntityFrameworkCore;

namespace Gravy.Persistence.Repositories;

public sealed class OrderRepository(ApplicationDbContext dbContext) : IOrderRepository
{
    private readonly ApplicationDbContext _dbContext = dbContext;

    public async Task<Order> GetByIdAsync(Guid id, CancellationToken cancellationToken = default) =>
        await _dbContext
            .Set<Order>()
            .Include(o => o.OrderItems)
            .Include(o => o.Payment)
            .Include(o => o.Delivery)
            .FirstOrDefaultAsync(o => o.Id == id, cancellationToken);

    public async Task<List<Order>> GetByCustomerIdAsync(Guid customerId, 
        CancellationToken cancellationToken = default) =>
            await _dbContext
                .Set<Order>()
                .Where(o => o.CustomerId == customerId)
                .ToListAsync(cancellationToken);

    public async Task<List<Order>> GetByRestaurantIdAsync(Guid restaurantId, 
        CancellationToken cancellationToken = default) =>
        await _dbContext
            .Set<Order>()
            .Where(o => o.RestaurantId == restaurantId)
            .ToListAsync(cancellationToken);

    public void Add(Order order) =>
        _dbContext.Set<Order>().Add(order);

    public void Update(Order order) =>
        _dbContext.Set<Order>().Update(order);

    public void Remove(Order order) =>
        _dbContext.Set<Order>().Remove(order);
}
