using Microsoft.AspNetCore.Authorization;
using Microsoft.Extensions.DependencyInjection;

namespace Gravy.Infrastructure.Authentication;

/// <summary>
/// Handles permission requirements by checking user claims.
/// </summary>
public class PermissionAuthorizationHandler(IServiceScopeFactory serviceScopeFactory) 
    : AuthorizationHandler<PermissionRequirement>
{
    private readonly IServiceScopeFactory _serviceScopeFactory = serviceScopeFactory;

    /// <summary>
    /// Evaluates if the user has the required permission.
    /// </summary>
    protected override Task HandleRequirementAsync(
        AuthorizationHandlerContext context,
        PermissionRequirement requirement)
    {
        // Fetch the user's permissions from claims
        HashSet<string> permissions = context
                .User
                .Claims
                .Where(x => x.Type == CustomClaims.Permissions)
                .Select(x => x.Value)
                .ToHashSet();

        // Succeed if the user has the required permission
        if (permissions.Contains(requirement.Permission))
        {
            context.Succeed(requirement);
        }

        return Task.CompletedTask;
    }
}