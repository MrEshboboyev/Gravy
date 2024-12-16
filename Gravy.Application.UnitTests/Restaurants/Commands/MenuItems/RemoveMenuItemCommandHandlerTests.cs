using FluentAssertions;
using Gravy.Application.Restaurants.Commands.MenuItems.RemoveMenuItem;
using Gravy.Domain.Entities;
using Gravy.Domain.Enums;
using Gravy.Domain.Errors;
using Gravy.Domain.Repositories;
using Gravy.Domain.ValueObjects;
using Moq;

namespace Gravy.Application.UnitTests.Restaurants.Commands.MenuItems;

public class RemoveMenuItemCommandHandlerTests
{
    #region Fields & Mocks

    private readonly RemoveMenuItemCommandHandler _handler;

    // Mock dependencies
    private readonly Mock<IRestaurantRepository> _restaurantRepositoryMock;
    private readonly Mock<IUnitOfWork> _unitOfWorkMock;

    public RemoveMenuItemCommandHandlerTests()
    {
        _restaurantRepositoryMock = new Mock<IRestaurantRepository>();
        _unitOfWorkMock = new Mock<IUnitOfWork>();

        _handler = new RemoveMenuItemCommandHandler(
            _restaurantRepositoryMock.Object,
            _unitOfWorkMock.Object);
    }

    #endregion

    #region Test Methods

    /// <summary>
    /// Test Case: Successfully removes a menu item when inputs are valid.
    /// </summary>
    [Fact]
    public async Task Handle_Should_RemoveMenuItem_WhenInputsAreValid()
    {
        // Arrange
        var restaurantId = Guid.NewGuid();

        // Mock an existing restaurant with the menu item
        var restaurant = Restaurant.Create(
            restaurantId,
            "Test Restaurant",
            "Best Restaurant",
            Email.Create("test@test.com").Value,
            "1234567890",
            Address.Create("Test Address").Value,
            Guid.NewGuid(),
            OpeningHours.Create(
                new TimeSpan(9, 0, 0), // 9:00 AM
                new TimeSpan(17, 0, 0) // 5:00 PM
            ).Value);

        // Simulate the menu item being removed successfully
        var addMenuItemResult = restaurant.AddMenuItem(
            "Pizza",
            "Cheese Pizza",
            12.99m, 
            Category.MainCourse);

        var command = new RemoveMenuItemCommand(restaurantId, addMenuItemResult.Value.Id);

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

        _restaurantRepositoryMock.Verify(
            repo => repo.GetByIdAsync(
                restaurantId, 
                It.IsAny<CancellationToken>()),
            Times.Once);

        _unitOfWorkMock.Verify(
            uow => uow.SaveChangesAsync(
                It.IsAny<CancellationToken>()),
            Times.Once);
    }

    /// <summary>
    /// Test Case: Returns failure when the restaurant does not exist.
    /// </summary>
    [Fact]
    public async Task Handle_Should_ReturnFailure_WhenRestaurantNotFound()
    {
        // Arrange
        var restaurantId = Guid.NewGuid();
        var menuItemId = Guid.NewGuid();

        var command = new RemoveMenuItemCommand(restaurantId, menuItemId);

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
            repo => repo.GetByIdAsync(
                restaurantId,
                It.IsAny<CancellationToken>()),
            Times.Once);

        _unitOfWorkMock.Verify(
            uow => uow.SaveChangesAsync(
                It.IsAny<CancellationToken>()),
            Times.Never);
    }

    /// <summary>
    /// Test Case: Returns failure when removing the menu item fails (invalid menu item ID).
    /// </summary>
    [Fact]
    public async Task Handle_Should_ReturnFailure_WhenMenuItemDoesNotExist()
    {
        // Arrange
        var restaurantId = Guid.NewGuid();
        var menuItemId = Guid.NewGuid();

        var command = new RemoveMenuItemCommand(restaurantId, menuItemId);

        var restaurant = Restaurant.Create(
            restaurantId,
            "Test Restaurant",
            "Best Restaurant",
            Email.Create("test@test.com").Value,
            "1234567890",
            Address.Create("Test Address").Value,
            Guid.NewGuid(),
            OpeningHours.Create(
                new TimeSpan(9, 0, 0),
                new TimeSpan(17, 0, 0)
            ).Value);

        // Simulate failure when attempting to remove a non-existent menu item
        restaurant.AddMenuItem(
            "Pizza",
            "Cheese Pizza",
            12.99m, 
            Category.MainCourse);

        _restaurantRepositoryMock
            .Setup(repo => repo.GetByIdAsync(
                restaurantId, 
                It.IsAny<CancellationToken>()))
            .ReturnsAsync(restaurant);

        // Act
        var result = await _handler.Handle(command, CancellationToken.None);

        // Assert
        result.IsSuccess.Should().BeFalse();
        result.Error.Should().Be(DomainErrors.Restaurant.MenuItemNotFound(
            restaurantId,
            menuItemId));

        _restaurantRepositoryMock.Verify(
            repo => repo.GetByIdAsync(restaurantId, 
                It.IsAny<CancellationToken>()),
            Times.Once);

        _unitOfWorkMock.Verify(
            uow => uow.SaveChangesAsync(
                It.IsAny<CancellationToken>()),
            Times.Never);
    }

    #endregion
}