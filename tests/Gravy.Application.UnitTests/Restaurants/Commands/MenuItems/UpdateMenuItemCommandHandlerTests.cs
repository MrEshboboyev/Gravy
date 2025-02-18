using FluentAssertions;
using Gravy.Application.Restaurants.Commands.MenuItems.UpdateMenuItem;
using Gravy.Domain.Entities;
using Gravy.Domain.Enums;
using Gravy.Domain.Errors;
using Gravy.Domain.Repositories;
using Gravy.Domain.ValueObjects;
using Moq;

namespace Gravy.Application.UnitTests.Restaurants.Commands.MenuItems;

public class UpdateMenuItemCommandHandlerTests
{
    #region Fields & Mock Setup

    private readonly UpdateMenuItemCommandHandler _handler;

    // Mock dependencies
    private readonly Mock<IRestaurantRepository> _restaurantRepositoryMock;
    private readonly Mock<IMenuItemRepository> _menuItemRepositoryMock;
    private readonly Mock<IUnitOfWork> _unitOfWorkMock;

    public UpdateMenuItemCommandHandlerTests()
    {
        // Initialize mocks
        _restaurantRepositoryMock = new Mock<IRestaurantRepository>();
        _menuItemRepositoryMock = new Mock<IMenuItemRepository>();
        _unitOfWorkMock = new Mock<IUnitOfWork>();

        // Initialize the handler with mocks
        _handler = new UpdateMenuItemCommandHandler(
            _restaurantRepositoryMock.Object,
            _menuItemRepositoryMock.Object,
            _unitOfWorkMock.Object);
    }

    #endregion

    #region Test Methods

    /// <summary>
    /// Test Case: Successfully updates a menu item when inputs are valid.
    /// </summary>
    [Fact]
    public async Task Handle_Should_UpdateMenuItem_WhenInputsAreValid()
    {
        // Arrange
        var restaurantId = Guid.NewGuid();

        // Mock existing restaurant with an existing menu item
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

        // Add an existing menu item to the restaurant
        var addMenuItemResult = restaurant.AddMenuItem(
            "Old Pasta",
            "Old description",
            12.99m,
            Category.MainCourse);

        var command = new UpdateMenuItemCommand(
            restaurantId,
            addMenuItemResult.Value.Id,
            "Updated Pasta",
            "Creamy Alfredo pasta",
            15.99m,
            Category.MainCourse,
            true);

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

        _menuItemRepositoryMock.Verify(
            repo => repo.Update(It.Is<MenuItem>(
                mi => mi.Name == "Updated Pasta" &&
                      mi.Description == "Creamy Alfredo pasta" &&
                      mi.Price == 15.99m)),
            Times.Once);

        _unitOfWorkMock.Verify(
            uow => uow.SaveChangesAsync(
                It.IsAny<CancellationToken>()),
            Times.Once);
    }

    /// <summary>
    /// Test Case: Fails when the restaurant is not found.
    /// </summary>
    [Fact]
    public async Task Handle_Should_ReturnFailure_WhenRestaurantNotFound()
    {
        // Arrange
        var restaurantId = Guid.NewGuid();
        var menuItemId = Guid.NewGuid();

        var command = new UpdateMenuItemCommand(
            restaurantId,
            menuItemId,
            "Updated Pasta",
            "Creamy Alfredo pasta",
            15.99m,
            Category.MainCourse,
            true);

        _restaurantRepositoryMock
            .Setup(repo => repo.GetByIdAsync(
                restaurantId, 
                It.IsAny<CancellationToken>()))
            .ReturnsAsync((Restaurant)null!);

        // Act
        var result = await _handler.Handle(command, CancellationToken.None);

        // Assert
        result.IsSuccess.Should().BeFalse();
        result.Error.Should().Be(DomainErrors.Restaurant.NotFound(restaurantId));

        _menuItemRepositoryMock.Verify(repo => repo.Update(
            It.IsAny<MenuItem>()), 
            Times.Never);
        _unitOfWorkMock.Verify(uow => uow.SaveChangesAsync(
            It.IsAny<CancellationToken>()), 
            Times.Never);
    }

    /// <summary>
    /// Test Case: Fails to update a menu item due to domain validation errors.
    /// </summary>
    [Fact]
    public async Task Handle_Should_ReturnFailure_WhenMenuItemUpdateFails()
    {
        // Arrange
        var restaurantId = Guid.NewGuid();
        var menuItemId = Guid.NewGuid();

        var command = new UpdateMenuItemCommand(
            restaurantId,
            menuItemId,
            "", // Invalid Name
            "Invalid Description",
            -10.99m, // Invalid Price
            Category.MainCourse,
            true);

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

        _restaurantRepositoryMock
            .Setup(repo => repo.GetByIdAsync(
                restaurantId, 
                It.IsAny<CancellationToken>()))
            .ReturnsAsync(restaurant);

        // Simulate failure in UpdateMenuItem
        restaurant.UpdateMenuItem(
            menuItemId,
            "", // Invalid name to force failure
            "Invalid Description",
            -10.99m,
            Category.MainCourse,
            true);

        // Act
        var result = await _handler.Handle(command, CancellationToken.None);

        // Assert
        result.IsSuccess.Should().BeFalse();
        result.Error.Should().NotBeNull();

        _menuItemRepositoryMock.Verify(repo => repo.Update(
            It.IsAny<MenuItem>()), 
            Times.Never);
        _unitOfWorkMock.Verify(uow => uow.SaveChangesAsync(
            It.IsAny<CancellationToken>()), 
            Times.Never);
    }

    #endregion
}