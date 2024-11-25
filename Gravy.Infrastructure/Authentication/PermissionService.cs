using Gravy.Domain.Entities;
using Gravy.Persistence;
using Microsoft.EntityFrameworkCore;

namespace Gravy.Infrastructure.Authentication;

public class PermissionService(ApplicationDbContext context) : IPermissionService
{
    private readonly ApplicationDbContext _context = context;

    public async Task<HashSet<string>> GetPermissionsAsync(Guid userId)
    {
        ICollection<Role>[] roles = await _context.Set<User>()
             .Include(x => x.Roles)
             .ThenInclude(x => x.Permissions)
             .Where(x => x.Id == userId)
             .Select(x => x.Roles)
             .ToArrayAsync();

        return roles
            .SelectMany(x => x)
            .SelectMany(x => x.Permissions)
            .Select(x => x.Name)
            .ToHashSet();
    }
}
