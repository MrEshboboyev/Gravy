namespace Gravy.Presentation.Contracts.Users;

public sealed record RegisterUserRequest(
    string Email,
    string Password,
    string FirstName,
    string LastName);