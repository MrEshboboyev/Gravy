﻿using Gravy.Application.Abstractions;
using Gravy.Application.Abstractions.Messaging;
using Gravy.Domain.Entities;
using Gravy.Domain.Errors;
using Gravy.Domain.Repositories;
using Gravy.Domain.Shared;
using Gravy.Domain.ValueObjects;

namespace Gravy.Application.Users.Commands.Login;

internal sealed class LoginCommandHandler(IUserRepository userRepository,
    IJwtProvider jwtProvider) : ICommandHandler<LoginCommand, string>
{
    private readonly IUserRepository _userRepository = userRepository;
    private readonly IJwtProvider _jwtProvider = jwtProvider;

    public async Task<Result<string>> Handle(LoginCommand request, CancellationToken cancellationToken)
    {
        Result<Email> email = Email.Create(request.Email);

        User user = await _userRepository.GetByEmailAsync(
            email.Value,
            cancellationToken);

        if (user is null)
        {
            return Result.Failure<string>(
                DomainErrors.User.InvalidCredentials);
        }

        string token = await _jwtProvider.GenerateAsync(user);
        return token;
    }
}
