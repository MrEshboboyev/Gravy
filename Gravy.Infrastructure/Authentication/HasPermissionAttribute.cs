using Gravy.Domain.Enums;
using Microsoft.AspNetCore.Authorization;

namespace Gravy.Infrastructure.Authentication;

public sealed class HasPermissionAttribute(Permission permission) : 
    AuthorizeAttribute(policy: permission.ToString())
{
}
