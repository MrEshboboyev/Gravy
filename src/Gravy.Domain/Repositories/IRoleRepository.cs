using Gravy.Domain.Entities;

namespace Gravy.Domain.Repositories;

public interface IRoleRepository
{
    Task<Role> GetByIdAsync(int id, CancellationToken cancellationToken = default);
    Task<Role> GetByNameAsync(string name, CancellationToken cancellationToken = default);
    Task<List<Role>> GetRolesAsync(CancellationToken cancellationToken = default);
    void Add(Role role);
    void Update(Role role);
}

