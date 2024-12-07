using Gravy.Domain.Entities;
using Gravy.Domain.Repositories;
using Microsoft.EntityFrameworkCore;

namespace Gravy.Persistence.Restaurants.Repositories;

public sealed class RestaurantRepository(ApplicationDbContext dbContext) : IRestaurantRepository
{
    private readonly ApplicationDbContext _dbContext = dbContext;

    public async Task<Restaurant> GetByIdAsync(Guid id, CancellationToken cancellationToken = default) =>
        await _dbContext
            .Set<Restaurant>()
            .Include(r => r.MenuItems)
            .FirstOrDefaultAsync(r => r.Id == id, cancellationToken);

    public async Task<Restaurant> GetByNameAsync(string name,
        CancellationToken cancellationToken = default) =>
        await _dbContext
            .Set<Restaurant>()
            .Include(r => r.MenuItems)
            .FirstOrDefaultAsync(r => r.Name == name, cancellationToken);

    public async Task<List<Restaurant>> GetByOwnerIdAsync(Guid ownerId,
        CancellationToken cancellationToken = default) =>
        await _dbContext
            .Set<Restaurant>()
            .Include(r => r.MenuItems)
            .Where(r => r.OwnerId == ownerId)
            .ToListAsync(cancellationToken);


    public async Task<List<Restaurant>> SearchByTermAsync(string searchTerm,
        CancellationToken cancellationToken = default) =>
        await _dbContext
            .Set<Restaurant>()
            .Include(r => r.MenuItems)
            .Where(r => r.Name.Contains(searchTerm))
            .ToListAsync(cancellationToken);

    public void Add(Restaurant restaurant) =>
        _dbContext.Set<Restaurant>().Add(restaurant);

    public void Update(Restaurant restaurant) =>
        _dbContext.Set<Restaurant>().Update(restaurant);

    public void Remove(Restaurant restaurant) =>
        _dbContext.Set<Restaurant>().Remove(restaurant);
}
