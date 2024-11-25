using Gravy.Domain.Entities;

namespace Gravy.Application.Abstractions;

public interface IJwtProvider
{
    Task<string> GenerateAsync(User user);
}
