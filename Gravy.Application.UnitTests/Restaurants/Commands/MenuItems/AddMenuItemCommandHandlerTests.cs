using FluentAssertions;
using Gravy.Application.Restaurants.Commands.MenuItems.AddMenuItem;
using Gravy.Domain.Entities;
using Gravy.Domain.Enums;
using Gravy.Domain.Errors;
using Gravy.Domain.Repositories;
using Gravy.Domain.ValueObjects;
using Moq;

namespace Gravy.Application.UnitTests.Restaurants.Commands.MenuItems;

public class AddMenuItemCommandHandlerTests
{
    #region Fields & Mock Setup

    private readonly AddMenuItemCommandHandler _handler;

    // Mock dependencies
    private readonly Mock<IRestaurantRepository> _restaurantRepositoryMock;
    private readonly Mock<IMenuItemRepository> _menuItemRepositoryMock;
    private readonly Mock<IUnitOfWork> _unitOfWorkMock;

    public AddMenuItemCommandHandlerTests()
    {
        // Initialize mocks
        _restaurantRepositoryMock = new Mock<IRestaurantRepository>();
        _menuItemRepositoryMock = new Mock<IMenuItemRepository>();
        _unitOfWorkMock = new Mock<IUnitOfWork>();

        // Initialize the handler with mocks
        _handler = new AddMenuItemCommandHandler(
            _restaurantRepositoryMock.Object,
            _menuItemRepositoryMock.Object,
            _unitOfWorkMock.Object);
    }

    #endregion

    #region Test Methods

    /// <summary>
    /// Test Case: Successfully adds a menu item when inputs are valid.
    /// </summary>
    [Fact]
    public async Task Handle_Should_AddMenuItem_WhenInputsAreValid()
    {
        // Arrange
        var restaurantId = Guid.NewGuid();
        var command = new AddMenuItemCommand(
            restaurantId,
            "Pasta",
            "Delicious creamy pasta",
            12.99m,
            Category.MainCourse);

        // Mock existing restaurant
        var restaurant = Restaurant.Create(
            restaurantId,
            "Test Restaurant",
            "Best Test Restaurant",
            Email.Create("test@test.com").Value,
            "1234567890",
            Address.Create(("Test Address")).Value,
            Guid.NewGuid(),
            OpeningHours.Create(
                new TimeSpan(9, 0, 0), // 9:00 AM
                new TimeSpan(17, 0, 0) // 5:00 PM
            ).Value);

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
            repo => repo.Add(
                It.Is<MenuItem>(
                    mi => mi.Name == "Pasta" &&
                          mi.Description == "Delicious creamy pasta" &&
                          mi.Price == 12.99m)),
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
        var command = new AddMenuItemCommand(
            restaurantId,
            "Pasta",
            "Delicious creamy pasta",
            12.99m,
            Category.MainCourse);

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

        _menuItemRepositoryMock.Verify(
            repo => repo.Add(
                It.IsAny<MenuItem>()),
            Times.Never);

        _unitOfWorkMock.Verify(
            uow => uow.SaveChangesAsync(
                It.IsAny<CancellationToken>()), 
            Times.Never);
    }

    /// <summary>
    /// Test Case: Returns failure when adding a menu item fails due to business rules.
    /// </summary>
    [Fact]
    public async Task Handle_Should_ReturnFailure_WhenAddingMenuItemFails()
    {
        // Arrange
        var restaurantId = Guid.NewGuid();
        var command = new AddMenuItemCommand(
            restaurantId,
            "Invalid Item",
            "Invalid price",
            -5.00m, // Invalid price
            Category.MainCourse);

        // Mock existing restaurant
        var restaurant = Restaurant.Create(
            restaurantId,
            "Test Restaurant",
            "Best Test Restaurant",
            Email.Create("test@test.com").Value,
            "1234567890",
            Address.Create(("Test Address")).Value,
            Guid.NewGuid(),
            OpeningHours.Create(
                new TimeSpan(9, 0, 0), // 9:00 AM
                new TimeSpan(17, 0, 0) // 5:00 PM
            ).Value);

        _restaurantRepositoryMock
            .Setup(repo => repo.GetByIdAsync(
                restaurantId,
                It.IsAny<CancellationToken>()))
            .ReturnsAsync(restaurant);

        // Act
        var result = await _handler.Handle(command, CancellationToken.None);

        // Assert
        result.IsSuccess.Should().BeFalse();
        result.Error.Should().Be(DomainErrors.MenuItem.InvalidPrice);

        _menuItemRepositoryMock.Verify(
            repo => repo.Add(
                It.IsAny<MenuItem>()), 
            Times.Never);

        _unitOfWorkMock.Verify(
            uow => uow.SaveChangesAsync(
                It.IsAny<CancellationToken>()), 
            Times.Never);
    }

    /// <summary>
    /// Test Case: Ensures the Add method of the MenuItemRepository is called.
    /// </summary>
    [Fact]
    public async Task Handle_Should_CallMenuItemRepositoryAdd_WhenValidMenuItem()
    {
        // Arrange
        var restaurantId = Guid.NewGuid();
        var command = new AddMenuItemCommand(
            restaurantId,
            "Burger",
            "Juicy beef burger",
            10.99m,
            Category.MainCourse);

        // Mock existing restaurant
        var restaurant = Restaurant.Create(
            restaurantId,
            "Burger Joint",
            "Best Test Restaurant",
            Email.Create("test@test.com").Value,
            "1234567890",
            Address.Create(("Test Address")).Value,
            Guid.NewGuid(),
            OpeningHours.Create(
                new TimeSpan(9, 0, 0), // 9:00 AM
                new TimeSpan(17, 0, 0) // 5:00 PM
            ).Value);

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
        _menuItemRepositoryMock.Verify(
            repo => repo.Add(
                It.Is<MenuItem>(
                    mi => mi.Name == "Burger" && 
                          mi.Description == "Juicy beef burger")),
            Times.Once);

        _unitOfWorkMock.Verify(
            uow => uow.SaveChangesAsync(
                It.IsAny<CancellationToken>()),
            Times.Once);
    }

    /// <summary>
    /// Test Case: Fails to add a menu item when a duplicate exists.
    /// </summary>
    [Fact]
    public async Task Handle_Should_ReturnFailure_WhenMenuItemIsDuplicate()
    {
        // Arrange
        var restaurantId = Guid.NewGuid();
        var command = new AddMenuItemCommand(
            restaurantId,
            "Pasta",
            "Delicious creamy pasta",
            12.99m,
            Category.MainCourse);

        // Mock existing restaurant with an already existing menu item
        var restaurant = Restaurant.Create(
            restaurantId,
            "Test Restaurant",
            "Best Test Restaurant",
            Email.Create("test@test.com").Value,
            "1234567890",
            Address.Create(("Test Address")).Value,
            Guid.NewGuid(),
            OpeningHours.Create(
                new TimeSpan(9, 0, 0), // 9:00 AM
                new TimeSpan(17, 0, 0) // 5:00 PM
            ).Value);

        // Simulate existing menu item
        restaurant.AddMenuItem(
            "Pasta",
            "Delicious creamy pasta",
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
        result.Error.Should().Be(
            DomainErrors.General.DuplicateValue("MenuItem", "Pasta"));

        _menuItemRepositoryMock.Verify(
            repo => repo.Add(
                It.IsAny<MenuItem>()), 
            Times.Never);

        _unitOfWorkMock.Verify(
            uow => uow.SaveChangesAsync(
                It.IsAny<CancellationToken>()), 
            Times.Never);
    }

    #endregion
}