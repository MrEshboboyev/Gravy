using FluentAssertions;
using Gravy.Application.Abstractions;
using Gravy.Application.Users.Commands.CreateUser;
using Gravy.Domain.Entities;
using Gravy.Domain.Errors;
using Gravy.Domain.Repositories;
using Gravy.Domain.ValueObjects;
using Moq;

namespace Gravy.Application.UnitTests.Users.Commands;

/// <summary>
/// Unit tests for CreateUserCommandHandler.
/// Ensures command handler behavior is correct under specific scenarios.
/// </summary>
public class CreateUserCommandHandlerTests
{
    #region Fields & Mock Setup

    // Mock dependencies to isolate the command handler logic
    private readonly Mock<IUserRepository> _userRepositoryMock = new();
    private readonly Mock<IUnitOfWork> _unitOfWorkMock = new();
    private readonly Mock<IPasswordHasher> _passwordHasherMock = new();

    #endregion

    #region Test Methods

    /// <summary>
    /// Test case: Handle should return failure result if the email is not unique.
    /// </summary>
    [Fact]
    public async Task Handle_Should_ReturnFailureResult_WhenEmailIsNotUnique()
    {
        // Arrange: Set up the test scenario

        // Command input simulating user creation request
        var command = new CreateUserCommand(
            "email@test.com", 
            "password", 
            "first",
            "last");

        // Mock the behavior of IsEmailUniqueAsync to return 'false', 
        // meaning the email already exists in the repository
        _userRepositoryMock
            .Setup(
                x => x.IsEmailUniqueAsync(
                    It.IsAny<Email>(),
                    It.IsAny<CancellationToken>()))
            .ReturnsAsync(false);

        // Instantiate the handler with mocked dependencies
        var handler = new CreateUserCommandHandler(
            _userRepositoryMock.Object,
            _unitOfWorkMock.Object,
            _passwordHasherMock.Object);

        // Act: Execute the command handler
        var result = await handler.Handle(command, CancellationToken.None);

        // Assert: Verify that the handler returns a failure result
        result.IsFailure.Should().BeTrue();
        result.Error.Should().Be(DomainErrors.User.EmailAlreadyInUse);
    }

    /// <summary>
    /// Test case: Handle should return success result if the email is unique.
    /// </summary>
    [Fact]
    public async Task Handle_Should_ReturnSuccessResult_WhenEmailIsUnique()
    {
        // Arrange: Set up the test scenario

        // Command input simulating user creation request
        var command = new CreateUserCommand(
            "email@test.com",
            "password",
            "first",
            "last");

        // Mock the behavior of IsEmailUniqueAsync to return 'true', 
        // meaning the email not exists in the repository
        _userRepositoryMock
            .Setup(
                x => x.IsEmailUniqueAsync(
                    It.IsAny<Email>(),
                    It.IsAny<CancellationToken>()))
            .ReturnsAsync(true);

        // Instantiate the handler with mocked dependencies
        var handler = new CreateUserCommandHandler(
            _userRepositoryMock.Object,
            _unitOfWorkMock.Object,
            _passwordHasherMock.Object);

        // Act: Execute the command handler
        var result = await handler.Handle(command, CancellationToken.None);

        // Assert: Verify that the handler returns a success result
        result.IsSuccess.Should().BeTrue();
        result.Value.Should().NotBeEmpty();
    }

    [Fact]
    public async Task Handle_Should_CallAddOnRepository_WhenEmailIsUnique()
    {
        // Arrange: Set up the test scenario
        var command = new CreateUserCommand(
            "email@test.com",
            "password",
            "first",
            "last");

        // Mock the behavior of IsEmailUniqueAsync to return 'true',
        // meaning the email does not exist in the repository
        _userRepositoryMock
            .Setup(
                x => x.IsEmailUniqueAsync(
                    It.IsAny<Email>(),
                    It.IsAny<CancellationToken>()))
            .ReturnsAsync(true);

        User capturedUser = null!;
        _userRepositoryMock
            .Setup(x => x.Add(It.IsAny<User>()))
            .Callback<User>(user => capturedUser = user);

        // Instantiate the handler with mocked dependencies
        var handler = new CreateUserCommandHandler(
            _userRepositoryMock.Object,
            _unitOfWorkMock.Object,
            _passwordHasherMock.Object);

        // Act: Execute the command handler
        var result = await handler.Handle(command, CancellationToken.None);

        // Assert
        _userRepositoryMock.Verify(
            x => x.Add(It.IsAny<User>()),
            Times.Once);

        Assert.NotNull(capturedUser);
        Assert.Equal(result.Value, capturedUser.Id);
    }

    [Fact]
    public async Task Handle_Should_NotCallUnitOfWork_WhenEmailIsNotUnique()
    {
        // Arrange: Set up the test scenario

        // Command input simulating user creation request
        var command = new CreateUserCommand(
            "email@test.com",
            "password",
            "first",
            "last");

        // Mock the behavior of IsEmailUniqueAsync to return 'true', 
        // meaning the email not exists in the repository
        _userRepositoryMock
            .Setup(
                x => x.IsEmailUniqueAsync(
                    It.IsAny<Email>(),
                    It.IsAny<CancellationToken>()))
            .ReturnsAsync(false);

        // Instantiate the handler with mocked dependencies
        var handler = new CreateUserCommandHandler(
            _userRepositoryMock.Object,
            _unitOfWorkMock.Object,
            _passwordHasherMock.Object);

        // Act: Execute the command handler
        var result = await handler.Handle(command, CancellationToken.None);

        // Assert
        _unitOfWorkMock.Verify(
            x => x.SaveChangesAsync(It.IsAny<CancellationToken>()),
            Times.Never);
    }
    #endregion
}
