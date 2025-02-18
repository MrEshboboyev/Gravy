using FluentAssertions;
using Gravy.Application.Abstractions;
using Gravy.Application.Users.Commands.Login;
using Gravy.Domain.Entities;
using Gravy.Domain.Errors;
using Gravy.Domain.Repositories;
using Gravy.Domain.Shared;
using Gravy.Domain.ValueObjects;
using Moq;

namespace Gravy.Application.UnitTests.Users.Commands;

public class LoginCommandHandlerTests
{
    #region Fields & Mock Setup

    private readonly LoginCommandHandler _handler;

    // Mock dependencies
    private readonly Mock<IUserRepository> _userRepositoryMock;
    private readonly Mock<IPasswordHasher> _passwordHasherMock;
    private readonly Mock<IJwtProvider> _jwtProviderMock;

    public LoginCommandHandlerTests()
    {
        // Initialize mocks
        _userRepositoryMock = new Mock<IUserRepository>();
        _passwordHasherMock = new Mock<IPasswordHasher>();
        _jwtProviderMock = new Mock<IJwtProvider>();

        // Initialize handler with mocks
        _handler = new LoginCommandHandler(
            _userRepositoryMock.Object,
            _jwtProviderMock.Object,
            _passwordHasherMock.Object);
    }

    #endregion

    #region Test Methods

    /// <summary>
    /// Test Case: Successfully handle login when all conditions are met.
    /// </summary>
    [Fact]
    public async Task Handle_Should_ReturnToken_WhenCredentialsAreValid()
    {
        // Arrange
        var email = "test@example.com";
        var password = "CorrectPassword";
        var user = CreateTestUser(email, "hashedPassword123");
        var jwtToken = "jwt-token-123";

        var request = new LoginCommand(email, password);

        // Mock Email creation
        var emailResult = Result.Success(Email.Create(email).Value);

        // Mock repository to return the user
        _userRepositoryMock
            .Setup(repo => repo.GetByEmailAsync(
                It.Is<Email>(e => e.Value == email),
                It.IsAny<CancellationToken>()))
            .ReturnsAsync(user);

        // Mock password verification
        _passwordHasherMock
            .Setup(hasher => hasher.Verify(password, user.PasswordHash))
            .Returns(true);

        // Mock JWT provider to return a token
        _jwtProviderMock
            .Setup(provider => provider.GenerateAsync(user))
            .ReturnsAsync(jwtToken);

        // Act
        var result = await _handler.Handle(request, CancellationToken.None);

        // Assert
        result.IsSuccess.Should().BeTrue();
        result.Value.Should().Be(jwtToken);

        _userRepositoryMock.Verify(repo => repo.GetByEmailAsync(
            It.IsAny<Email>(), 
            It.IsAny<CancellationToken>()),
            Times.Once);

        _passwordHasherMock.Verify(hasher =>
            hasher.Verify(password, user.PasswordHash), 
            Times.Once);

        _jwtProviderMock.Verify(provider =>
            provider.GenerateAsync(user), 
            Times.Once);
    }

    /// <summary>
    /// Test Case: Return failure when email is invalid.
    /// </summary>
    [Fact]
    public async Task Handle_Should_ReturnFailure_WhenEmailIsInvalid()
    {
        // Arrange
        var email = "invalid_email"; // Invalid email
        var password = "SomePassword";
        var request = new LoginCommand(email, password);

        // Mock Email creation failure
        var emailResult = Result.Failure<Email>(DomainErrors.Email.InvalidFormat);

        // Act
        var result = await _handler.Handle(request, CancellationToken.None);

        // Assert
        result.IsSuccess.Should().BeFalse();
        result.Error.Should().Be(DomainErrors.Email.InvalidFormat);

        _userRepositoryMock.Verify(
            repo => repo.GetByEmailAsync(
                It.IsAny<Email>(), 
                It.IsAny<CancellationToken>()),
            Times.Never);
        _passwordHasherMock.Verify(hasher => hasher.Verify(
            It.IsAny<string>(), 
            It.IsAny<string>()),
            Times.Never);
        _jwtProviderMock.Verify(provider => provider.GenerateAsync(
            It.IsAny<User>()), 
            Times.Never);
    }

    /// <summary>
    /// Test Case: Return failure when user is not found in the repository.
    /// </summary>
    [Fact]
    public async Task Handle_Should_ReturnFailure_WhenUserIsNotFound()
    {
        // Arrange
        var email = "test@example.com";
        var password = "SomePassword";
        var request = new LoginCommand(email, password);

        // Mock Email creation success
        var emailResult = Result.Success(Email.Create(email).Value);

        // Mock repository to return null (user not found)
        _userRepositoryMock
            .Setup(repo => repo.GetByEmailAsync(
                It.Is<Email>(e => e.Value == email),
                It.IsAny<CancellationToken>()))
            .ReturnsAsync((User)null!);

        // Act
        var result = await _handler.Handle(request, CancellationToken.None);

        // Assert
        result.IsSuccess.Should().BeFalse();
        result.Error.Should().Be(DomainErrors.User.InvalidCredentials);

        _passwordHasherMock.Verify(hasher => hasher.Verify(
            It.IsAny<string>(),
            It.IsAny<string>()), 
            Times.Never);
        _jwtProviderMock.Verify(provider => provider.GenerateAsync(
            It.IsAny<User>()), 
            Times.Never);
    }

    /// <summary>
    /// Test Case: Return failure when password is invalid.
    /// </summary>
    [Fact]
    public async Task Handle_Should_ReturnFailure_WhenPasswordIsInvalid()
    {
        // Arrange
        var email = "test@example.com";
        var password = "WrongPassword";
        var user = CreateTestUser(email, "hashedPassword123");
        var request = new LoginCommand(email, password);

        // Mock Email creation success
        var emailResult = Result.Success(Email.Create(email).Value);

        // Mock repository to return user
        _userRepositoryMock
            .Setup(repo => repo.GetByEmailAsync(
                It.Is<Email>(e => e.Value == email),
                It.IsAny<CancellationToken>()))
            .ReturnsAsync(user);

        // Mock password verification failure
        _passwordHasherMock
            .Setup(hasher => hasher.Verify(password, user.PasswordHash))
            .Returns(false);

        // Act
        var result = await _handler.Handle(request, CancellationToken.None);

        // Assert
        result.IsSuccess.Should().BeFalse();
        result.Error.Should().Be(DomainErrors.User.InvalidCredentials);

        _jwtProviderMock.Verify(provider => provider.GenerateAsync(
            It.IsAny<User>()), 
            Times.Never);
    }

    /// <summary>
    /// Helper method to create a test user with email and password hash.
    /// </summary>
    private static User CreateTestUser(string email, string passwordHash)
    {
        var user = User.Create(Guid.NewGuid(),
            Email.Create(email).Value,
            passwordHash,
            FirstName.Create("firstName").Value,
            LastName.Create("lastName").Value);
        return user;
    }

    #endregion
}