using Gravy.Application.Abstractions.Messaging;

namespace Gravy.Application.Users.Commands;

public sealed record CreateUserCommand(
    string Email,
    string Password,
    string FirstName,
    string LastName) : ICommand<Guid>;
