using Gravy.Domain.Entities;
using Gravy.Domain.Repositories;

namespace Gravy.Persistence.Orders.Repositories.OrderItems;

public sealed class OrderItemRepository(ApplicationDbContext dbContext) 
    : IOrderItemRepository
{
    private readonly ApplicationDbContext _dbContext = dbContext;

    public void Add(OrderItem orderItem) =>
        _dbContext.Set<OrderItem>().Add(orderItem);

    public void Update(OrderItem orderItem) =>
        _dbContext.Set<OrderItem>().Update(orderItem);
}
