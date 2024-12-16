using FluentAssertions;
using Gravy.Application.Restaurants.Commands.UpdateRestaurant;
using Gravy.Domain.Entities;
using Gravy.Domain.Errors;
using Gravy.Domain.Repositories;
using Gravy.Domain.ValueObjects;
using Moq;

namespace Gravy.Application.UnitTests.Restaurants.Commands;

public class UpdateRestaurantCommandHandlerTests
{
    #region Fields & Mock Setup

    private readonly UpdateRestaurantCommandHandler _handler;

    // Mock dependencies
    private readonly Mock<IRestaurantRepository> _restaurantRepositoryMock;
    private readonly Mock<IUnitOfWork> _unitOfWorkMock;

    public UpdateRestaurantCommandHandlerTests()
    {
        // Initialize mocks
        _restaurantRepositoryMock = new Mock<IRestaurantRepository>();
        _unitOfWorkMock = new Mock<IUnitOfWork>();

        // Initialize the command handler with mocks
        _handler = new UpdateRestaurantCommandHandler(
            _restaurantRepositoryMock.Object,
            _unitOfWorkMock.Object);
    }

    #endregion

    #region Test Methods

    /// <summary>
    /// Test Case: Successfully updates a restaurant when inputs are valid.
    /// </summary>
    [Fact]
    public async Task Handle_Should_UpdateRestaurant_WhenInputsAreValid()
    {
        // Arrange
        var restaurantId = Guid.NewGuid();
        var command = new UpdateRestaurantCommand(
            restaurantId,
            "Updated Name",
            "Updated Description",
            "updated@example.com",
            "1234567890",
            "Updated Address");

        // Mock existing restaurant
        var restaurant = Restaurant.Create(
            restaurantId,
            "Old Name",
            "Old Description",
            Email.Create("old@example.com").Value,
            "0987654321",
            Address.Create("Old Address").Value,
            ownerId: Guid.NewGuid(),
            OpeningHours.Create(new TimeSpan(9, 0, 0), 
                new TimeSpan(19,0,0)).Value
        );

        _restaurantRepositoryMock
            .Setup(repo => repo.GetByIdAsync(
                restaurantId, 
                It.IsAny<CancellationToken>()))
            .ReturnsAsync(restaurant);

        _unitOfWorkMock
            .Setup(uow => uow.SaveChangesAsync(
                It.IsAny<CancellationToken>()))
            .Returns(Task.CompletedTask);

        // Act
        var result = await _handler.Handle(command, CancellationToken.None);

        // Assert
        result.IsSuccess.Should().BeTrue();

        restaurant.Name.Should().Be("Updated Name");
        restaurant.Description.Should().Be("Updated Description");
        restaurant.Email.Value.Should().Be("updated@example.com");
        restaurant.PhoneNumber.Should().Be("1234567890");
        restaurant.Address.Value.Should().Be("Updated Address");

        _restaurantRepositoryMock.Verify(
            repo => repo.Update(
                It.IsAny<Restaurant>()), 
            Times.Once);

        _unitOfWorkMock.Verify(
            uow => uow.SaveChangesAsync(
                It.IsAny<CancellationToken>()), 
            Times.Once);
    }

    /// <summary>
    /// Test Case: Returns failure when the restaurant is not found.
    /// </summary>
    [Fact]
    public async Task Handle_Should_ReturnFailure_WhenRestaurantNotFound()
    {
        // Arrange
        var restaurantId = Guid.NewGuid();
        var command = new UpdateRestaurantCommand(
            restaurantId,
            "Updated Name",
            "Updated Description",
            "updated@example.com",
            "1234567890",
            "Updated Address");

        _restaurantRepositoryMock
            .Setup(repo => repo.GetByIdAsync(
                restaurantId, 
                It.IsAny<CancellationToken>()))
            .ReturnsAsync((Restaurant)null!); // Simulate restaurant not found

        // Act
        var result = await _handler.Handle(command, CancellationToken.None);

        // Assert
        result.IsSuccess.Should().BeFalse();
        result.Error.Should().Be(DomainErrors.Restaurant.NotFound(restaurantId));

        _restaurantRepositoryMock.Verify(
            repo => repo.Update(
                It.IsAny<Restaurant>()), 
            Times.Never);

        _unitOfWorkMock.Verify(
            uow => uow.SaveChangesAsync(
                It.IsAny<CancellationToken>()),
            Times.Never);
    }

    /// <summary>
    /// Test Case: Returns failure when email creation fails.
    /// </summary>
    [Fact]
    public async Task Handle_Should_ReturnFailure_WhenEmailIsInvalid()
    {
        // Arrange
        var restaurantId = Guid.NewGuid();
        var command = new UpdateRestaurantCommand(
            restaurantId,
            "Updated Name",
            "Updated Description",
            "invalid-email", // Invalid email
            "1234567890",
            "Updated Address");

        var restaurant = Restaurant.Create(
            restaurantId,
            "Old Name",
            "Old Description",
            Email.Create("old@example.com").Value,
            "0987654321",
            Address.Create("Old Address").Value,
            ownerId: Guid.NewGuid(),
            OpeningHours.Create(new TimeSpan(9, 0, 0),
                new TimeSpan(19, 0, 0)).Value
        );

        _restaurantRepositoryMock
            .Setup(repo => repo.GetByIdAsync(
                restaurantId, 
                It.IsAny<CancellationToken>()))
            .ReturnsAsync(restaurant);

        // Act
        var result = await _handler.Handle(command, CancellationToken.None);

        // Assert
        result.IsSuccess.Should().BeFalse();
        result.Error.Should().Be(DomainErrors.Email.InvalidFormat);

        _restaurantRepositoryMock.Verify(
            repo => repo.Update(
                It.IsAny<Restaurant>()), 
            Times.Never);

        _unitOfWorkMock.Verify(
            uow => uow.SaveChangesAsync(
                It.IsAny<CancellationToken>()),
            Times.Never);
    }

    /// <summary>
    /// Test Case: Returns failure when address creation fails.
    /// </summary>
    [Fact]
    public async Task Handle_Should_ReturnFailure_WhenAddressIsInvalid()
    {
        // Arrange
        var restaurantId = Guid.NewGuid();
        var command = new UpdateRestaurantCommand(
            restaurantId,
            "Updated Name",
            "Updated Description",
            "updated@example.com",
            "1234567890",
            "" // Invalid address
        );

        var restaurant = Restaurant.Create(
            restaurantId,
            "Old Name",
            "Old Description",
            Email.Create("old@example.com").Value,
            "0987654321",
            Address.Create("Old Address").Value,
            ownerId: Guid.NewGuid(),
            OpeningHours.Create(new TimeSpan(9, 0, 0),
                new TimeSpan(19, 0, 0)).Value
        );

        _restaurantRepositoryMock
            .Setup(repo => repo.GetByIdAsync(
                restaurantId, 
                It.IsAny<CancellationToken>()))
            .ReturnsAsync(restaurant);

        // Act
        var result = await _handler.Handle(command, CancellationToken.None);

        // Assert
        result.IsSuccess.Should().BeFalse();
        result.Error.Should().Be(DomainErrors.Address.Empty);

        _restaurantRepositoryMock.Verify(
            repo => repo.Update(
                It.IsAny<Restaurant>()),
            Times.Never);

        _unitOfWorkMock.Verify(
            uow => uow.SaveChangesAsync(
                It.IsAny<CancellationToken>()), 
            Times.Never);
    }

    /// <summary>
    /// Test Case: Ensures repository update and unit of work save are called.
    /// </summary>
    [Fact]
    public async Task Handle_Should_CallUpdateAndSave_WhenRestaurantIsValid()
    {
        // Arrange
        var restaurantId = Guid.NewGuid();
        var command = new UpdateRestaurantCommand(
            restaurantId,
            "Updated Name",
            "Updated Description",
            "updated@example.com",
            "1234567890",
            "Updated Address");

        var restaurant = Restaurant.Create(
            restaurantId,
            "Old Name",
            "Old Description",
            Email.Create("old@example.com").Value,
            "0987654321",
            Address.Create("Old Address").Value,
            ownerId: Guid.NewGuid(),
            OpeningHours.Create(new TimeSpan(9, 0, 0),
                new TimeSpan(19, 0, 0)).Value
        );

        _restaurantRepositoryMock
            .Setup(repo => repo.GetByIdAsync(
                restaurantId, 
                It.IsAny<CancellationToken>()))
            .ReturnsAsync(restaurant);

        _unitOfWorkMock
            .Setup(uow => uow.SaveChangesAsync(
                It.IsAny<CancellationToken>()))
            .Returns(Task.CompletedTask);

        // Act
        await _handler.Handle(command, CancellationToken.None);

        // Assert
        _restaurantRepositoryMock.Verify(repo => repo.Update(restaurant), 
            Times.Once);
        _unitOfWorkMock.Verify(uow => uow.SaveChangesAsync(
            It.IsAny<CancellationToken>()),
            Times.Once);
    }

    #endregion
}