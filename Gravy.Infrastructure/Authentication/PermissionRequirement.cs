using Microsoft.AspNetCore.Authorization;

namespace Gravy.Infrastructure.Authentication;

public class PermissionRequirement(string permission) : IAuthorizationRequirement
{
    public string Permission { get; } = permission;
}