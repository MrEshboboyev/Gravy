using FluentAssertions;
using Gravy.Application.Users.Commands.DeliveryPersons.Availabilities.AddAvailability;
using Gravy.Domain.Entities;
using Gravy.Domain.Errors;
using Gravy.Domain.Repositories;
using Gravy.Domain.ValueObjects;
using Moq;

namespace Gravy.Application.UnitTests.Users.Commands.DeliveryPersons.Availabilities;

public class AddAvailabilityCommandHandlerTests
{
    #region Fields & Mock setup

    private readonly Mock<IUserRepository> _userRepositoryMock = new();
    private readonly Mock<IDeliveryPersonAvailabilityRepository> 
        _deliveryPersonAvailabilityRepositoryMock = new();
    private readonly Mock<IUnitOfWork> _unitOfWorkMock = new();

    private readonly User _user = User.Create(
        Guid.NewGuid(),
        Email.Create("email@test.com").Value,
        "hashedPassword",
        FirstName.Create("firstName").Value,
        LastName.Create("lastName").Value);

    #endregion

    #region Test Methods

    /// <summary>
    /// Test case: Handle should return failure if user is not found
    /// </summary>
    [Fact]
    public async Task Handle_Should_ReturnFailure_WhenUserNotFound()
    {
        // Arrange
        var command = new AddAvailabilityCommand(
            Guid.NewGuid(),
            DateTime.UtcNow, 
            DateTime.UtcNow.AddHours(10));

        _userRepositoryMock
            .Setup(x => x.GetByIdWithDeliveryPersonDetailsAsync(
                command.UserId,
                It.IsAny<CancellationToken>()))
            .ReturnsAsync((User)null!); // Simulate user not found

        var handler = new AddAvailabilityCommandHandler(
            _userRepositoryMock.Object,
            _deliveryPersonAvailabilityRepositoryMock.Object,
            _unitOfWorkMock.Object);

        // Act
        var result = await handler.Handle(command, CancellationToken.None);

        // Assert
        result.IsFailure.Should().BeTrue();
        result.Error.Should().Be(DomainErrors.User.NotFound(command.UserId));
    }

    /// <summary>
    /// Test case: Handle should return failure if delivery person details is not exist for user
    /// </summary>
    [Fact]
    public async Task Handle_Should_ReturnFailure_WhenDeliveryPersonDetailsNotExistForUser()
    {
        // Arrange
        var command = new AddAvailabilityCommand(
            _user.Id,
            DateTime.UtcNow,
            DateTime.UtcNow.AddHours(10));

        _userRepositoryMock
            .Setup(x => x.GetByIdWithDeliveryPersonDetailsAsync(
                command.UserId,
                It.IsAny<CancellationToken>()))
            .ReturnsAsync(_user); // user was found

        var handler = new AddAvailabilityCommandHandler(
            _userRepositoryMock.Object,
            _deliveryPersonAvailabilityRepositoryMock.Object,
            _unitOfWorkMock.Object);

        // Act
        var result = await handler.Handle(command, CancellationToken.None);

        // Assert
        result.IsFailure.Should().BeTrue();
        result.Error.Should().Be(DomainErrors.User.DeliveryPersonDetailsNotExist(_user.Id));
    }

    /// <summary>
    /// Test case: Handle should return success when delivery person availability is added successfully.
    /// </summary>
    [Fact]
    public async Task Handle_Should_ReturnSuccess_WhenDeliveryPersonAvailabilityIsAddedSuccessfully()
    {
        // Arrange
        var command = new AddAvailabilityCommand(
            _user.Id,
            DateTime.UtcNow.AddHours(1),
            DateTime.UtcNow.AddHours(10));

        // Add delivery person details for _user
        _user.AddDeliveryPersonDetails(
            Vehicle.Create("Car", "license plate").Value,
            Location.Create(10, 20).Value);

        _userRepositoryMock
            .Setup(x => x.GetByIdWithDeliveryPersonDetailsAsync(
                It.IsAny<Guid>(),
                It.IsAny<CancellationToken>()))
            .ReturnsAsync(_user); // user was found

        _deliveryPersonAvailabilityRepositoryMock
            .Setup(x => x.Add(
                It.IsAny<DeliveryPersonAvailability>()));

        var handler = new AddAvailabilityCommandHandler(
            _userRepositoryMock.Object,
            _deliveryPersonAvailabilityRepositoryMock.Object,
            _unitOfWorkMock.Object);

        // Act: Execute the command handler
        var result = await handler.Handle(command, CancellationToken.None);

        // Assert
        result.IsSuccess.Should().BeTrue();
        _deliveryPersonAvailabilityRepositoryMock
            .Verify(x => x.Add(
                    It.IsAny<DeliveryPersonAvailability>()),
                Times.Once);
        _unitOfWorkMock
            .Verify(x => x.SaveChangesAsync(
                It.IsAny<CancellationToken>()),
                Times.Once);
    }

    /// <summary>
    /// Test case: Handle should add delivery person availability to the repository and save changes when all conditions are met.
    /// </summary>
    [Fact]
    public async Task Handle_Should_AddAvailabilityAndSaveChanges_WhenAllConditionsAreMet()
    {
        // Arrange: Set up the test scenario
        var command = new AddAvailabilityCommand(
            _user.Id,
            DateTime.UtcNow.AddHours(1),
            DateTime.UtcNow.AddHours(10));

        // Add delivery person details for _user
        _user.AddDeliveryPersonDetails(
            Vehicle.Create("Car", "license plate").Value,
            Location.Create(10, 20).Value);

        // Mock the behavior of GetByIdWithDeliveryPersonDetailsAsync to return the user
        _userRepositoryMock
            .Setup(x => x.GetByIdWithDeliveryPersonDetailsAsync(
                It.IsAny<Guid>(),
                It.IsAny<CancellationToken>()))
            .ReturnsAsync(_user); // user was found

        var handler = new AddAvailabilityCommandHandler(
            _userRepositoryMock.Object,
            _deliveryPersonAvailabilityRepositoryMock.Object,
            _unitOfWorkMock.Object);

        // Act: Execute the command handler
        var result = await handler.Handle(command, CancellationToken.None);

        // Assert: Verify that the handler adds the availability and saves changes
        result.IsSuccess.Should().BeTrue();
        _deliveryPersonAvailabilityRepositoryMock
            .Verify(x => x.Add(
                It.IsAny<DeliveryPersonAvailability>()),
                Times.Once);
        _unitOfWorkMock
            .Verify(x => x.SaveChangesAsync(
                It.IsAny<CancellationToken>()),
                Times.Once);
    }

    #endregion
}