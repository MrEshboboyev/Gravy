using Gravy.Domain.Entities;
using Gravy.Domain.Repositories;
using Microsoft.EntityFrameworkCore;

namespace Gravy.Persistence.Users.Repositories.Roles;

public sealed class RoleRepository(ApplicationDbContext dbContext) : IRoleRepository
{
    private readonly ApplicationDbContext _dbContext = dbContext;

    public async Task<Role> GetByIdAsync(int id, CancellationToken cancellationToken = default)
        => await _dbContext
            .Set<Role>()
            .FirstOrDefaultAsync(r => r.Id == id, cancellationToken);

    public async Task<Role> GetByNameAsync(string name, CancellationToken cancellationToken = default)
        => await _dbContext
                .Set<Role>()
                .FirstOrDefaultAsync(r => r.Name == name, cancellationToken);

    public async Task<List<Role>> GetRolesAsync(CancellationToken cancellationToken = default)
        => await _dbContext
                .Set<Role>()
                .ToListAsync(cancellationToken);


    public void Add(Role role)
        => _dbContext
                .Set<Role>()
                .Add(role);

    public void Update(Role role)
        => _dbContext
                .Set<Role>()
                .Update(role);
}


