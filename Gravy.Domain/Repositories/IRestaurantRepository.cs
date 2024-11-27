using Gravy.Domain.Entities;

namespace Gravy.Domain.Repositories;

public interface IRestaurantRepository : IRepository<Restaurant>
{
    Task<List<Restaurant>> GetByNameAsync(string name, CancellationToken cancellationToken = default);
    Task<Restaurant> GetByIdAsync(Guid id, CancellationToken cancellationToken = default);
    Task<List<Restaurant>> GetByOwnerIdAsync(Guid ownerId, CancellationToken cancellationToken = default); // New
    void Add(Restaurant restaurant);
    void Remove(Restaurant restaurant);
}