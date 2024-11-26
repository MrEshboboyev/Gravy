using Gravy.Application.Abstractions;
using Gravy.Domain.Entities;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using System.Data;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

namespace Gravy.Infrastructure.Authentication;

/// <summary>
/// Generates JWT tokens for authenticated users.
/// </summary>
internal sealed class JwtProvider(IOptions<JwtOptions> options) : IJwtProvider
{
    private readonly JwtOptions _options = options.Value;

    /// <summary>
    /// Generates a JWT token for the given user.
    /// </summary>
    public async Task<string> GenerateAsync(User user)
    {
        var claims = new List<Claim>
        {
                new(JwtRegisteredClaimNames.Sub, user.Id.ToString()),
                new(JwtRegisteredClaimNames.Email, user.Email.Value)
        };

        #region Add Roles and Permissions
        // Add roles
        claims.AddRange(user.Roles.Select(role => new Claim(ClaimTypes.Role, role.Name)));

        var permissions = user.Roles
            .SelectMany(role => role.Permissions)
            .Select(permission => permission.Name)
            .ToHashSet();

        // Add permissions
        claims.AddRange(permissions.Select(permission => new Claim(CustomClaims.Permissions, permission)));
        #endregion

        var signingCredentials = new SigningCredentials(
             new SymmetricSecurityKey(
                 Encoding.UTF8.GetBytes(_options.SecretKey)),
             SecurityAlgorithms.HmacSha256);

        var token = new JwtSecurityToken(
            _options.Issuer,
            _options.Audience,
            claims,
            null,
            DateTime.UtcNow.AddHours(1),
            signingCredentials);

        string tokenValue = new JwtSecurityTokenHandler()
             .WriteToken(token);

        return tokenValue;
    }
}

