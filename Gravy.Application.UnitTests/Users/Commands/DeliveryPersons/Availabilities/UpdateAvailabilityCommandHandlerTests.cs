using FluentAssertions;
using Gravy.Application.Users.Commands.DeliveryPersons.Availabilities.UpdateAvailability;
using Gravy.Domain.Entities;
using Gravy.Domain.Errors;
using Gravy.Domain.Repositories;
using Gravy.Domain.ValueObjects;
using Moq;

namespace Gravy.Application.UnitTests.Users.Commands.DeliveryPersons.Availabilities;

public class UpdateAvailabilityCommandHandlerTests
{
    #region Fields & Mock Setup

    private readonly Mock<IUserRepository> _userRepositoryMock;
    private readonly Mock<IDeliveryPersonAvailabilityRepository> _deliveryPersonAvailabilityRepositoryMock;
    private readonly Mock<IUnitOfWork> _unitOfWorkMock;
    private readonly UpdateAvailabilityCommandHandler _handler;

    public UpdateAvailabilityCommandHandlerTests()
    {
        _userRepositoryMock = new Mock<IUserRepository>();
        _deliveryPersonAvailabilityRepositoryMock = new Mock<IDeliveryPersonAvailabilityRepository>();
        _unitOfWorkMock = new Mock<IUnitOfWork>();

        _handler = new UpdateAvailabilityCommandHandler(
            _userRepositoryMock.Object,
            _deliveryPersonAvailabilityRepositoryMock.Object,
            _unitOfWorkMock.Object);
    }

    #endregion

    #region Test methods

    /// <summary>
    /// Test case: Successfully update availability when all conditions are met.
    /// </summary>
    [Fact]
    public async Task Handle_Should_UpdateAvailabilityAndSaveChanges_WhenAllConditionsAreMet()
    {
        // Arrange
        var userId = Guid.NewGuid();
        var startTimeUtc = DateTime.UtcNow.AddHours(1);
        var endTimeUtc = DateTime.UtcNow.AddHours(3);

        var user = CreateTestUser(userId);

        user.AddDeliveryPersonDetails(
            Vehicle.Create("Car", "XYZ123").Value,
            Location.Create(10, 20).Value);

        var availability = user.AddDeliveryPersonAvailability(
            startTimeUtc,
            endTimeUtc).Value;

        var command = new UpdateAvailabilityCommand(
            userId, 
            availability.Id, 
            startTimeUtc, 
            endTimeUtc);


        // Mock: User repository returns user
        _userRepositoryMock
            .Setup(repo => repo.GetByIdWithDeliveryPersonDetailsAsync(
                userId, 
                It.IsAny<CancellationToken>()))
            .ReturnsAsync(user);

        // Mock: Successful availability update
        user.UpdateDeliveryPersonAvailability(
            availability.Id, 
            startTimeUtc, 
            endTimeUtc);

        // Act
        var result = await _handler.Handle(command, CancellationToken.None);

        // Assert
        result.IsSuccess.Should().BeTrue();

        _deliveryPersonAvailabilityRepositoryMock.Verify(
            repo => repo.Update(
                It.IsAny<DeliveryPersonAvailability>()), 
            Times.Once);

        _unitOfWorkMock.Verify(
            uow => uow.SaveChangesAsync(
                It.IsAny<CancellationToken>()),
            Times.Once);
    }

    /// <summary>
    /// Test case: Returns failure when user is not found.
    /// </summary>
    [Fact]
    public async Task Handle_Should_ReturnFailure_WhenUserIsNotFound()
    {
        // Arrange
        var userId = Guid.NewGuid();
        var command = new UpdateAvailabilityCommand(
            userId, 
            Guid.NewGuid(),
            DateTime.UtcNow, 
            DateTime.UtcNow.AddHours(1));

        // Mock: User repository returns null
        _userRepositoryMock
            .Setup(repo => repo.GetByIdWithDeliveryPersonDetailsAsync(
                userId, 
                It.IsAny<CancellationToken>()))
            .ReturnsAsync((User)null!);

        // Act
        var result = await _handler.Handle(command, CancellationToken.None);

        // Assert
        result.IsSuccess.Should().BeFalse();
        result.Error.Should().Be(DomainErrors.User.NotFound(userId));
    }

    /// <summary>
    /// Test case: Returns failure when user delivery person details do not exist.
    /// </summary>
    [Fact]
    public async Task Handle_Should_ReturnFailure_WhenDeliveryPersonDetailsDoNotExist()
    {
        // Arrange
        var userId = Guid.NewGuid();
        var command = new UpdateAvailabilityCommand(
            userId, 
            Guid.NewGuid(), 
            DateTime.UtcNow, 
            DateTime.UtcNow.AddHours(1));

        var user = CreateTestUser(userId);

        // Mock: User repository returns user without DeliveryPersonDetails
        _userRepositoryMock
            .Setup(repo => repo.GetByIdWithDeliveryPersonDetailsAsync(
                userId, 
                It.IsAny<CancellationToken>()))
            .ReturnsAsync(user);

        // Act
        var result = await _handler.Handle(command, CancellationToken.None);

        // Assert
        result.IsSuccess.Should().BeFalse();
        result.Error.Should().Be(DomainErrors.User.DeliveryPersonDetailsNotExist(userId));
    }

    /// <summary>
    /// Test case: Returns failure when updating availability fails due to invalid data.
    /// </summary>
    [Fact]
    public async Task Handle_Should_ReturnFailure_WhenAvailabilityUpdateFails()
    {
        // Arrange
        var userId = Guid.NewGuid();
        var availabilityId = Guid.NewGuid();
        var startTimeUtc = DateTime.UtcNow.AddHours(1);
        var endTimeUtc = DateTime.UtcNow.AddHours(3);

        var command = new UpdateAvailabilityCommand(
            userId,
            availabilityId, 
            startTimeUtc, 
            endTimeUtc);

        var user = CreateTestUser(userId);

        user.AddDeliveryPersonDetails(
            Vehicle.Create("Car", "XYZ123").Value,
            Location.Create(10, 20).Value);

        // Mock: User repository returns user
        _userRepositoryMock
            .Setup(repo => repo.GetByIdWithDeliveryPersonDetailsAsync(
                userId, 
                It.IsAny<CancellationToken>()))
            .ReturnsAsync(user);

        // Mock: UpdateDeliveryPersonAvailability fails
        user.UpdateDeliveryPersonAvailability(
                availabilityId, 
                startTimeUtc, 
                endTimeUtc)
            .IsFailure.Should().BeTrue(); // Simulating failure

        // Act
        var result = await _handler.Handle(command, CancellationToken.None);

        // Assert
        result.IsSuccess.Should().BeFalse();
        result.Error.Should().NotBeNull();
    }

    #endregion

    #region Helpers

    /// <summary>
    /// Helper: Creates a test user.
    /// </summary>
    private static User CreateTestUser(Guid userId)
    {
        return User.Create(userId,
            Email.Create("test@email.com").Value,
            "hashedPassword",
            FirstName.Create("John").Value,
            LastName.Create("Doe").Value);
    }

    #endregion
}