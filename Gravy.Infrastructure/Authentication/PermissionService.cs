using Gravy.Domain.Entities;
using Gravy.Persistence;
using Microsoft.EntityFrameworkCore;

namespace Gravy.Infrastructure.Authentication;

/// <summary>
/// Service to retrieve permissions for a user.
/// </summary>
public class PermissionService(ApplicationDbContext context) : IPermissionService
{
    private readonly ApplicationDbContext _context = context;

    /// <summary>
    /// Retrieves all permissions for the specified user.
    /// </summary>
    /// <param name="userId">The user's unique identifier.</param>
    /// <returns>A hash set of permission names.</returns>
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
