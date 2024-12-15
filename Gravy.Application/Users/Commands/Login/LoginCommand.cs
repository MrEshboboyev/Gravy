using Gravy.Application.Abstractions.Messaging;

namespace Gravy.Application.Users.Commands.Login;

public record LoginCommand(
    string Email, 
    string Password) : ICommand<string>;
