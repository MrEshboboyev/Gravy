using Gravy.Domain.Entities;

namespace Gravy.Domain.Repositories;

public interface IOrderItemRepository
{
    void Add(OrderItem orderItem);
    void Update(OrderItem orderItem);
}