using Microsoft.AspNetCore.Authorization;
using Microsoft.Extensions.Options;

namespace Gravy.Infrastructure.Authentication;

/// <summary>
/// Dynamically generates authorization policies for permissions.
/// </summary>
public class PermissionAuthorizationPolicyProvider(
    IOptions<AuthorizationOptions> options)
        : DefaultAuthorizationPolicyProvider(options)
{
    public override async Task<AuthorizationPolicy> GetPolicyAsync(string policyName)
    {
        AuthorizationPolicy policy = await base.GetPolicyAsync(policyName);
        if (policy is not null)
        {
            return policy;
        }

        // Create a new policy if not found
        return new AuthorizationPolicyBuilder()
            .AddRequirements(new PermissionRequirement(policyName))
            .Build();
    }
}