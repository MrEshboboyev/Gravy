namespace Gravy.Presentation.Contracts.Users;

public record LoginRequest(
    string Email, 
    string Password);
