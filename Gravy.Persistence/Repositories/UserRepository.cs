using Gravy.Domain.Entities;
using Gravy.Domain.Repositories;
using Gravy.Domain.ValueObjects;
using Microsoft.EntityFrameworkCore;

namespace Gravy.Persistence.Repositories;

public sealed class UserRepository(ApplicationDbContext dbContext) : IUserRepository
{
    private readonly ApplicationDbContext _dbContext = dbContext;

    public async Task<List<User>> GetUsersAsync(CancellationToken cancellationToken = default)
        => await _dbContext.Set<User>()
            .Include(u => u.CustomerDetails)
            .Include(u => u.DeliveryPersonDetails)
            .ToListAsync(cancellationToken);

    public async Task<User> GetByIdAsync(Guid id, CancellationToken cancellationToken = default) =>
        await _dbContext
            .Set<User>()
            .Include(u => u.Roles)
            .Include(u => u.CustomerDetails)
            .Include(u => u.DeliveryPersonDetails)
            .FirstOrDefaultAsync(user => user.Id == id, cancellationToken);

    public async Task<User> GetByEmailAsync(Email email, CancellationToken cancellationToken = default) =>
        await _dbContext
            .Set<User>()
            .Include(u => u.Roles)
            .ThenInclude(r => r.Permissions)
            .FirstOrDefaultAsync(user => user.Email == email, cancellationToken);

    public async Task<bool> IsEmailUniqueAsync(
        Email email,
        CancellationToken cancellationToken = default) =>
        !await _dbContext
            .Set<User>()
            .AnyAsync(user => user.Email == email, cancellationToken);

    public void Add(User user) =>
        _dbContext.Set<User>().Add(user);

    public void Update(User user) =>
        _dbContext.Set<User>().Update(user);

    #region Customer related
    public async Task<User> GetByIdWithCustomerDetailsAsync(Guid id, CancellationToken cancellationToken = default) =>
        await _dbContext
            .Set<User>()
            .Include(u => u.CustomerDetails)
            .FirstOrDefaultAsync(user => user.Id == id, cancellationToken);
    #endregion

    #region Delivery-Person related
    public async Task<User> GetByIdWithDeliveryPersonDetailsAsync(Guid id, CancellationToken cancellationToken = default) =>
        await _dbContext
            .Set<User>()
            .Include(u => u.DeliveryPersonDetails)
            .FirstOrDefaultAsync(user => user.Id == id, cancellationToken);
    #endregion
}