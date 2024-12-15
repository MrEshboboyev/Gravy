using FluentAssertions;
using Gravy.Application.Users.Commands.DeliveryPersons.Availabilities.DeleteAvailability;
using Gravy.Domain.Entities;
using Gravy.Domain.Errors;
using Gravy.Domain.Repositories;
using Gravy.Domain.Shared;
using Gravy.Domain.ValueObjects;
using Moq;

namespace Gravy.Application.UnitTests.Users.Commands.DeliveryPersons.Availabilities;

public class DeleteAvailabilityCommandHandlerTests
{
    #region Fields & Mock Setup

    private readonly Mock<IUserRepository> _userRepositoryMock;
    private readonly Mock<IUnitOfWork> _unitOfWorkMock;
    private readonly DeleteAvailabilityCommandHandler _handler;

    public DeleteAvailabilityCommandHandlerTests()
    {
        _userRepositoryMock = new Mock<IUserRepository>();
        _unitOfWorkMock = new Mock<IUnitOfWork>();

        _handler = new DeleteAvailabilityCommandHandler(
            _userRepositoryMock.Object,
            _unitOfWorkMock.Object);
    }

    #endregion

    #region Test Methods

    /// <summary>
    /// Test case: Successfully delete availability when all conditions are met.
    /// </summary>
    [Fact]
    public async Task Handle_Should_DeleteAvailabilityAndSaveChanges_WhenAllConditionsAreMet()
    {
        // Arrange: Define test data
        var userId = Guid.NewGuid(); // Unique identifier for the test user
        var startTimeUtc = DateTime.UtcNow.AddHours(1); // Start time for availability
        var endTimeUtc = DateTime.UtcNow.AddHours(3);   // End time for availability

        // Create a user with valid delivery person details
        var user = CreateTestUser(userId);
        user.AddDeliveryPersonDetails(
            Vehicle.Create("Car", "XYZ123").Value, // Vehicle details
            Location.Create(10, 20).Value);       // Location details

        // Add an availability to the user
        var availability = user.AddDeliveryPersonAvailability(
            startTimeUtc,
            endTimeUtc).Value;

        // Command to delete the specific availability
        var command = new DeleteAvailabilityCommand(
            userId,
            availability.Id);

        // Mock: Simulate repository returning the user with delivery person details
        _userRepositoryMock
            .Setup(repo => repo.GetByIdWithDeliveryPersonDetailsAsync(
                userId,
                It.IsAny<CancellationToken>()))
            .ReturnsAsync(user);

        // Mock: Simulate SaveChangesAsync to verify database updates
        _unitOfWorkMock
            .Setup(uow => uow.SaveChangesAsync(
                It.IsAny<CancellationToken>()))
            .Returns(Task.CompletedTask); // Returns a completed task to mimic successful save

        // Act: Call the handler to process the command
        var result = await _handler.Handle(command, CancellationToken.None);

        // Assert: Verify the result is successful
        result.IsSuccess.Should().BeTrue();

        // Verify that SaveChangesAsync was called exactly once
        _unitOfWorkMock.Verify(
            uow => uow.SaveChangesAsync(It.IsAny<CancellationToken>()),
            Times.Once);

        // Verify that the availability was deleted from the user
        user.DeliveryPersonDetails.Availabilities.Should()
            .NotContain(a => a.Id == availability.Id); // Ensure the availability no longer exists
    }

    /// <summary>
    /// Test case: Returns failure when user is not found.
    /// </summary>
    [Fact]
    public async Task Handle_Should_ReturnFailure_WhenUserIsNotFound()
    {
        // Arrange
        var userId = Guid.NewGuid();
        var command = new DeleteAvailabilityCommand(
            userId,
            Guid.NewGuid());

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
    /// Test case: Returns failure when deleting availability fails due to invalid data.
    /// </summary>
    [Fact]
    public async Task Handle_Should_ReturnFailure_WhenAvailabilityDeleteFails()
    {
        // Arrange: Define test data
        var userId = Guid.NewGuid(); // Unique identifier for the user
        var availabilityId = Guid.NewGuid(); // Unique identifier for the availability to delete

        // Command to delete availability
        var command = new DeleteAvailabilityCommand(
            userId,
            availabilityId);

        // Create a user and add delivery person details
        var user = CreateTestUser(userId);
        user.AddDeliveryPersonDetails(
            Vehicle.Create("Car", "XYZ123").Value, // Valid vehicle details
            Location.Create(10, 20).Value);       // Valid location details

        // Mock: Simulate repository returning the user
        _userRepositoryMock
            .Setup(repo => repo.GetByIdWithDeliveryPersonDetailsAsync(
                userId,
                It.IsAny<CancellationToken>()))
            .ReturnsAsync(user);

        // Simulate failure in deleting the availability
        // Use a mock for the domain method `DeleteDeliveryPersonAvailability` to return failure
        user.DeleteDeliveryPersonAvailability(availabilityId);
        var deleteAvailabilityResult = Result.Failure(
            DomainErrors.DeliveryPersonAvailability.NotFound(availabilityId));

        // Act: Call the handler to process the command
        var result = await _handler.Handle(command, CancellationToken.None);

        // Assert: Verify that the result indicates failure
        result.IsSuccess.Should().BeFalse();        // Ensure the result is a failure
        result.Error.Should().NotBeNull();          // Ensure an error message exists
        result.Error.Should().Be(
            DomainErrors.DeliveryPersonAvailability.NotFound(availabilityId)); // Confirm the expected error message

        // Verify that SaveChangesAsync was never called, as the operation failed
        _unitOfWorkMock.Verify(
            uow => uow.SaveChangesAsync(It.IsAny<CancellationToken>()),
            Times.Never);
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