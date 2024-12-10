using Gravy.Domain.Entities;
using Gravy.Domain.Repositories;
using Microsoft.EntityFrameworkCore;

namespace Gravy.Persistence.Restaurants.Repositories.MenuItems;

public sealed class MenuItemRepository(ApplicationDbContext dbContext) : IMenuItemRepository
{
    private readonly ApplicationDbContext _dbContext = dbContext;

    public async Task<MenuItem> GetByIdAsync(Guid id, CancellationToken cancellationToken) =>
        await _dbContext
            .Set<MenuItem>()
            .FirstOrDefaultAsync(mi => mi.Id == id, cancellationToken);

    public void Add(MenuItem menuItem) =>
        _dbContext.Set<MenuItem>().Add(menuItem);

    public void Update(MenuItem menuItem) =>
        _dbContext.Set<MenuItem>().Update(menuItem);
}
