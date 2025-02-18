using Microsoft.AspNetCore.Authorization;

namespace Gravy.Infrastructure.Authentication;

/// <summary>
/// Represents a requirement for a specific permission.
/// </summary>
public class PermissionRequirement : IAuthorizationRequirement
{
    /// <summary>
    /// The required permission name.
    /// </summary>
    public string Permission { get; }

    public PermissionRequirement(string permission)
    {
        Permission = permission;
    }
}