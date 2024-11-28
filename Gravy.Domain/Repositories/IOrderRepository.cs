using Gravy.Domain.Entities;

namespace Gravy.Domain.Repositories;

public interface IOrderRepository : IRepository<Order>
{
    Task<Order> GetByIdAsync(Guid id, CancellationToken cancellationToken = default);
    Task<List<Order>> GetByCustomerIdAsync(Guid customerId, CancellationToken cancellationToken = default);
    Task<List<Order>> GetByRestaurantIdAsync(Guid restaurantId, CancellationToken cancellationToken = default);
    void Add(Order order);
    void Update(Order order);
    void Remove(Order order);
}
