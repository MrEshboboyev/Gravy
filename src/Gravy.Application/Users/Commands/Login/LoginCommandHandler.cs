using Gravy.Application.Abstractions;
using Gravy.Application.Abstractions.Messaging;
using Gravy.Domain.Errors;
using Gravy.Domain.Repositories;
using Gravy.Domain.Shared;
using Gravy.Domain.ValueObjects;

namespace Gravy.Application.Users.Commands.Login;

internal sealed class LoginCommandHandler(
    IUserRepository userRepository,
    IJwtProvider jwtProvider, 
    IPasswordHasher passwordHasher) : ICommandHandler<LoginCommand, string>
{
    private readonly IUserRepository _userRepository = userRepository;
    private readonly IJwtProvider _jwtProvider = jwtProvider;
    private readonly IPasswordHasher _passwordHasher = passwordHasher;

    public async Task<Result<string>> Handle(LoginCommand request, CancellationToken cancellationToken)
    {
        #region Prepare Value Objects

        Result<Email> createEmailResult = Email.Create(request.Email);
        if (createEmailResult.IsFailure)
        {
            return Result.Failure<string>(
                createEmailResult.Error);
        }

        #endregion

        #region Try to Get User by Email.Value

        var user = await _userRepository.GetByEmailAsync(
            createEmailResult.Value,
            cancellationToken);

        #endregion

        #region Checking User is null and credentials is valid

        if (user is null || !_passwordHasher.Verify(request.Password, user.PasswordHash))
        {
            return Result.Failure<string>(
                DomainErrors.User.InvalidCredentials);
        }

        #endregion

        #region Generate JWT Token using custom provider

        var token = await _jwtProvider.GenerateAsync(user);

        #endregion

        return Result.Success(token);
    }
}

