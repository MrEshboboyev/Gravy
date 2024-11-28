using Gravy.Domain.Entities;

namespace Gravy.Domain.Repositories;

public interface IRestaurantRepository : IRepository<Restaurant>
{
    Task<Restaurant> GetByIdAsync(Guid id, CancellationToken cancellationToken = default);
    Task<List<Restaurant>> GetByNameAsync(string name, CancellationToken cancellationToken = default);
    Task<List<Restaurant>> GetByOwnerIdAsync(Guid ownerId, CancellationToken cancellationToken = default); 
    void Add(Restaurant restaurant);
    void Update(Restaurant restaurant);
    void Remove(Restaurant restaurant);
}