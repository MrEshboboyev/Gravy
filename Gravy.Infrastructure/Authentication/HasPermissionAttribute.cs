using Gravy.Domain.Enums;
using Microsoft.AspNetCore.Authorization;

namespace Gravy.Infrastructure.Authentication;

/// <summary>
/// Attribute to enforce permission-based authorization on an endpoint.
/// </summary>
public sealed class HasPermissionAttribute : 
    AuthorizeAttribute
{
    /// <summary>
    /// Creates an instance of the attribute with the specified permission.
    /// </summary>
    /// <param name="permission">The required permission.</param>
    public HasPermissionAttribute(Permission permission)
        : base(policy: permission.ToString())
    {
    }
}
