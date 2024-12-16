using FluentAssertions;
using Gravy.Application.Restaurants.Commands.CreateRestaurant;
using Gravy.Domain.Entities;
using Gravy.Domain.Repositories;
using Moq;

namespace Gravy.Application.UnitTests.Restaurants.Commands;

public class CreateRestaurantCommandHandlerTests
{
    #region Fields & Mock Setup

    private readonly CreateRestaurantCommandHandler _handler;

    // Mock dependencies
    private readonly Mock<IRestaurantRepository> _restaurantRepositoryMock;
    private readonly Mock<IUnitOfWork> _unitOfWorkMock;

    public CreateRestaurantCommandHandlerTests()
    {
        // Initialize mocks
        _restaurantRepositoryMock = new Mock<IRestaurantRepository>();
        _unitOfWorkMock = new Mock<IUnitOfWork>();

        // Initialize the command handler with mocks
        _handler = new CreateRestaurantCommandHandler(
            _restaurantRepositoryMock.Object,
            _unitOfWorkMock.Object);
    }

    #endregion

    #region Test Methods

    /// <summary>
    /// Test Case: Successfully creates a restaurant when all conditions are met.
    /// </summary>
    [Fact]
    public async Task Handle_Should_CreateRestaurant_WhenAllInputsAreValid()
    {
        // Arrange
        var command = new CreateRestaurantCommand(
            "Test Restaurant",
            "Best Test Restaurant",
            "test@example.com",
            "1234567890",
            "123 Test Street",
            Guid.NewGuid(),
            [
                new TimeSpan(9, 0, 0), // 9:00 AM
                new TimeSpan(17, 0, 0) // 5:00 PM
            ]
        );

        var restaurantId = Guid.NewGuid();

        // Mock the repository to simulate adding a restaurant
        _restaurantRepositoryMock
            .Setup(repo => repo.Add(
                It.IsAny<Restaurant>()));

        _unitOfWorkMock
            .Setup(uow => uow.SaveChangesAsync(
                It.IsAny<CancellationToken>()))
            .Returns(Task.CompletedTask);

        // Act
        var result = await _handler.Handle(command, CancellationToken.None);

        // Assert
        result.IsSuccess.Should().BeTrue();
        result.Value.Should().NotBeEmpty();

        // Verify repository's Add method is called once
        _restaurantRepositoryMock.Verify(repo => repo.Add(
            It.IsAny<Restaurant>()), 
            Times.Once);

        // Verify SaveChangesAsync is called once
        _unitOfWorkMock.Verify(uow => uow.SaveChangesAsync(
            It.IsAny<CancellationToken>()),
            Times.Once);
    }

    /// <summary>
    /// Test Case: Returns failure when email creation fails.
    /// </summary>
    [Fact]
    public async Task Handle_Should_ReturnFailure_WhenEmailIsInvalid()
    {
        // Arrange
        var command = new CreateRestaurantCommand(
            "Test Restaurant",
            "Best Test Restaurant",
            "invalid-email", // Invalid email format
            "1234567890",
            "123 Test Street",
            Guid.NewGuid(),
            [
                new TimeSpan(9, 0, 0), // 9:00 AM
                new TimeSpan(17, 0, 0) // 5:00 PM
            ]
        );

        // Act
        var result = await _handler.Handle(command, CancellationToken.None);

        // Assert
        result.IsSuccess.Should().BeFalse();
        result.Error.Should().NotBeNull();

        // Verify repository and unit of work methods were NOT called
        _restaurantRepositoryMock.Verify(repo => repo.Add(
            It.IsAny<Restaurant>()), 
            Times.Never);
        _unitOfWorkMock.Verify(uow => uow.SaveChangesAsync(
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
        var command = new CreateRestaurantCommand(
            "Test Restaurant",
            "Best Test Restaurant",
            "test@example.com",
            "1234567890",
            "", // Invalid address (empty)
            Guid.NewGuid(),
            [
                new TimeSpan(9, 0, 0), // 9:00 AM
                new TimeSpan(17, 0, 0) // 5:00 PM
            ]
        );

        // Act
        var result = await _handler.Handle(command, CancellationToken.None);

        // Assert
        result.IsSuccess.Should().BeFalse();
        result.Error.Should().NotBeNull();

        // Verify repository and unit of work methods were NOT called
        _restaurantRepositoryMock.Verify(repo => repo.Add(
            It.IsAny<Restaurant>()), 
            Times.Never);
        _unitOfWorkMock.Verify(uow => uow.SaveChangesAsync(
            It.IsAny<CancellationToken>()), 
            Times.Never);
    }

    /// <summary>
    /// Test Case: Returns failure when opening hours creation fails.
    /// </summary>
    [Fact]
    public async Task Handle_Should_ReturnFailure_WhenOpeningHoursAreInvalid()
    {
        // Arrange
        var command = new CreateRestaurantCommand(
            "Test Restaurant",
            "Best Test Restaurant",
            "test@example.com",
            "1234567890",
            "123 Test Street",
            Guid.NewGuid(),
            [
                new TimeSpan(25, 0, 0), 
                new TimeSpan(22, 0, 0) 
            ]// Invalid opening hour
        );

        // Act
        var result = await _handler.Handle(command, CancellationToken.None);

        // Assert
        result.IsSuccess.Should().BeFalse();
        result.Error.Should().NotBeNull();

        // Verify repository and unit of work methods were NOT called
        _restaurantRepositoryMock.Verify(repo => repo.Add(
            It.IsAny<Restaurant>()), 
            Times.Never);
        _unitOfWorkMock.Verify(uow => uow.SaveChangesAsync(
            It.IsAny<CancellationToken>()), 
            Times.Never);
    }

    /// <summary>
    /// Test Case: Verifies that SaveChangesAsync is called after successful creation.
    /// </summary>
    [Fact]
    public async Task Handle_Should_CallSaveChangesAsync_WhenRestaurantIsCreated()
    {
        // Arrange
        var command = new CreateRestaurantCommand(
            "Test Restaurant",
            "Best Test Restaurant",
            "test@example.com",
            "1234567890",
            "123 Test Street",
            Guid.NewGuid(),
            [
                new TimeSpan(9, 0, 0), // 9:00 AM
                new TimeSpan(17, 0, 0) // 5:00 PM
            ]
        );

        // Mock the repository and unit of work
        _restaurantRepositoryMock
            .Setup(repo => repo.Add(
                It.IsAny<Restaurant>()));
        _unitOfWorkMock
            .Setup(uow => uow.SaveChangesAsync(
                It.IsAny<CancellationToken>()))
            .Returns(Task.CompletedTask);

        // Act
        var result = await _handler.Handle(command, CancellationToken.None);

        // Assert
        result.IsSuccess.Should().BeTrue();

        // Verify SaveChangesAsync was called exactly once
        _unitOfWorkMock.Verify(uow => uow.SaveChangesAsync(
            It.IsAny<CancellationToken>()),
            Times.Once);
    }

    #endregion
}