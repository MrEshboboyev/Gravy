using Gravy.Application.Abstractions;
using Gravy.Application.Abstractions.Messaging;
using Gravy.Domain.Entities;
using Gravy.Domain.Errors;
using Gravy.Domain.Repositories;
using Gravy.Domain.Shared;
using Gravy.Domain.ValueObjects;

namespace Gravy.Application.Users.Commands.CreateUser;

internal sealed class CreateUserCommandHandler(
    IUserRepository userRepository,
    IUnitOfWork unitOfWork,
    IPasswordHasher passwordHasher) : ICommandHandler<CreateUserCommand, Guid>
{
    private readonly IUserRepository _userRepository = userRepository;
    private readonly IUnitOfWork _unitOfWork = unitOfWork;
    private readonly IPasswordHasher _passwordHasher = passwordHasher;

    public async Task<Result<Guid>> Handle(CreateUserCommand request, CancellationToken cancellationToken)
    {
        #region Create Email and checking email unique

        Result<Email> emailResult = Email.Create(request.Email);
        if (!await _userRepository.IsEmailUniqueAsync(emailResult.Value, cancellationToken))
        {
            return Result.Failure<Guid>(DomainErrors.User.EmailAlreadyInUse);
        }

        #endregion

        #region Create Other value objects (FirstName, LastName) 

        Result<FirstName> createFirstNameResult = FirstName.Create(request.FirstName);
        if (createFirstNameResult.IsFailure)
        {
            return Result.Failure<Guid>(
                createFirstNameResult.Error);
        }

        Result<LastName> createLastNameResult = LastName.Create(request.LastName);
        if (createLastNameResult.IsFailure)
        {
            return Result.Failure<Guid>(
                createLastNameResult.Error);
        }

        #endregion

        #region Create Password Hash

        var passwordHash = _passwordHasher.Hash(request.Password);
        var user = User.Create(
            Guid.NewGuid(),
            emailResult.Value,
            passwordHash,
            createFirstNameResult.Value,
            createLastNameResult.Value
            );

        #endregion

        #region Add and Update database

        _userRepository.Add(user);
        await _unitOfWork.SaveChangesAsync(cancellationToken);

        #endregion

        return Result.Success(user.Id);
    }
}