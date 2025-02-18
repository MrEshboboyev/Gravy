using FluentAssertions;
using Gravy.Application.Users.Commands.DeliveryPersons.AddDeliveryPersonDetails;
using Gravy.Domain.Entities;
using Gravy.Domain.Errors;
using Gravy.Domain.Repositories;
using Gravy.Domain.ValueObjects;
using Moq;

namespace Gravy.Application.UnitTests.Users.Commands.DeliveryPersons;

public class AddDeliveryPersonCommandHandlerTests
{
    #region Fields & Mock Setup

    private readonly Mock<IUserRepository> _userRepositoryMock = new();
    private readonly Mock<IDeliveryPersonRepository> _deliveryPersonRepositoryMock = new();
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
        var command = new AddDeliveryPersonDetailsCommand(
            Guid.NewGuid(),
            "type",
            "license plate",
            0.0,
            0.0);

        _userRepositoryMock
            .Setup(x => x.GetByIdWithDeliveryPersonDetailsAsync(
                command.UserId,
                It.IsAny<CancellationToken>()))
            .ReturnsAsync((User)null!); // Simulate user not found

        var handler = new AddDeliveryPersonDetailsCommandHandler(
            _userRepositoryMock.Object,
            _deliveryPersonRepositoryMock.Object,
            _unitOfWorkMock.Object);

        // Act
        var result = await handler.Handle(command, CancellationToken.None);

        // Assert
        result.IsFailure.Should().BeTrue();
        result.Error.Should().Be(DomainErrors.User.NotFound(command.UserId));
    }

    /// <summary>
    /// Test case: Handle should return failure when vehicle creation fails
    /// </summary>
    [Fact]
    public async Task Handle_Should_ReturnFailure_WhenVehicleIsInvalid()
    {
        // Arrange
        _userRepositoryMock
            .Setup(x => x.GetByIdWithDeliveryPersonDetailsAsync(
                It.IsAny<Guid>(),
                It.IsAny<CancellationToken>()))
            .ReturnsAsync(_user); // returns user 

        var invalidCommand = new AddDeliveryPersonDetailsCommand(
            _user.Id,
            "",
            "",
            34.23645,
            -87.5673);

        var handler = new AddDeliveryPersonDetailsCommandHandler(
            _userRepositoryMock.Object,
            _deliveryPersonRepositoryMock.Object,
            _unitOfWorkMock.Object);

        // Act
        var result = await handler.Handle(invalidCommand, CancellationToken.None);

        // Assert
        result.IsFailure.Should().BeTrue();
        result.Error.Should().NotBeNull();
    }

    /// <summary>
    /// Test case: Handle should return failure when location creation fails
    /// </summary>
    [Fact]
    public async Task Handle_Should_ReturnFailure_WhenLocationIsInvalid()
    {
        // Arrange
        _userRepositoryMock
            .Setup(x => x.GetByIdWithDeliveryPersonDetailsAsync(
                It.IsAny<Guid>(),
                It.IsAny<CancellationToken>()))
            .ReturnsAsync(_user); // returns user 

        var invalidCommand = new AddDeliveryPersonDetailsCommand(
            _user.Id,
            "Car",
            "license plate",
            100,
            200);

        var handler = new AddDeliveryPersonDetailsCommandHandler(
            _userRepositoryMock.Object,
            _deliveryPersonRepositoryMock.Object,
            _unitOfWorkMock.Object);

        // Act
        var result = await handler.Handle(invalidCommand, CancellationToken.None);

        // Assert
        result.IsFailure.Should().BeTrue();
        result.Error.Should().NotBeNull();
    }

    /// <summary>
    /// Test case: Handle should return success if delivery person details are added successfully
    /// </summary>
    [Fact]
    public async Task Handle_Should_ReturnSuccess_WhenDeliveryPersonDetailsAreAddedSuccessfully()
    {
        // Arrange
        var command = new AddDeliveryPersonDetailsCommand(
            _user.Id,
            "Car",
            "license plate",
            18,
            26);

        _userRepositoryMock
            .Setup(x => x.GetByIdWithDeliveryPersonDetailsAsync(
                command.UserId,
                It.IsAny<CancellationToken>()))
            .ReturnsAsync(_user);

        _deliveryPersonRepositoryMock
            .Setup(x => x.Add(
                It.IsAny<DeliveryPerson>()));

        var handler = new AddDeliveryPersonDetailsCommandHandler(
            _userRepositoryMock.Object,
            _deliveryPersonRepositoryMock.Object,
            _unitOfWorkMock.Object);

        // Act
        var result = await handler.Handle(command, CancellationToken.None);

        // Assert
        result.IsSuccess.Should().BeTrue();
        _deliveryPersonRepositoryMock
            .Verify(x => x.Add(
                It.IsAny<DeliveryPerson>()),
                Times.Once);
        _unitOfWorkMock
            .Verify(x => x.SaveChangesAsync(
                It.IsAny<CancellationToken>()),
                Times.Once);
    }

    /// <summary>
    /// Test case: Handle should not call Add or SaveChangesAsync when vehicle or location creation fails.
    /// </summary>
    [Fact]
    public async Task Handle_Should_NotCallAddOrSave_WhenVehicleOrLocationCreationFails()
    {
        // Arrange
        var invalidCommand = new AddDeliveryPersonDetailsCommand(
            _user.Id,
            "",
            "",
            100,
            200);

        _userRepositoryMock
            .Setup(x => x.GetByIdWithDeliveryPersonDetailsAsync(
                It.IsAny<Guid>(),
                It.IsAny<CancellationToken>()))
            .ReturnsAsync(_user);

        var handler = new AddDeliveryPersonDetailsCommandHandler(
            _userRepositoryMock.Object,
            _deliveryPersonRepositoryMock.Object,
            _unitOfWorkMock.Object);

        // Act
        var result = await handler.Handle(invalidCommand, CancellationToken.None);

        // Assert
        _deliveryPersonRepositoryMock
            .Verify(x => x.Add(
                It.IsAny<DeliveryPerson>()),
                Times.Never);
        _unitOfWorkMock
            .Verify(x => x.SaveChangesAsync(
                It.IsAny<CancellationToken>()),
                Times.Never);
        result.IsFailure.Should().BeTrue();
    }
    #endregion
}