using FluentAssertions;
using Gravy.Application.Orders.Commands.OrderItems.AddOrderItem;
using Gravy.Application.Services.Orders.Interfaces;
using Gravy.Domain.Entities;
using Gravy.Domain.Enums;
using Gravy.Domain.Errors;
using Gravy.Domain.Repositories;
using Gravy.Domain.Shared;
using Gravy.Domain.ValueObjects;
using Moq;

namespace Gravy.Application.UnitTests.Orders.OrderItems;

public class AddOrderItemCommandHandlerTests
{
    #region Fields & Mocks

    private readonly AddOrderItemCommandHandler _handler;

    private readonly Mock<IOrderRepository> _orderRepositoryMock;
    private readonly Mock<IOrderItemRepository> _orderItemRepositoryMock;
    private readonly Mock<IMenuItemRepository> _menuItemRepositoryMock;
    private readonly Mock<IUnitOfWork> _unitOfWorkMock;
    private readonly Mock<IPricingService> _pricingServiceMock;

    public AddOrderItemCommandHandlerTests()
    {
        _orderRepositoryMock = new Mock<IOrderRepository>();
        _orderItemRepositoryMock = new Mock<IOrderItemRepository>();
        _menuItemRepositoryMock = new Mock<IMenuItemRepository>();
        _unitOfWorkMock = new Mock<IUnitOfWork>();
        _pricingServiceMock = new Mock<IPricingService>();

        _handler = new AddOrderItemCommandHandler(
            _orderRepositoryMock.Object,
            _orderItemRepositoryMock.Object,
            _menuItemRepositoryMock.Object,
            _unitOfWorkMock.Object,
            _pricingServiceMock.Object);
    }

    #endregion

    #region Test Methods

    /// <summary>
    /// Test Case: Successfully adds an order item when all inputs are valid.
    /// </summary>
    [Fact]
    public async Task Handle_Should_AddOrderItem_WhenEverythingIsValid()
    {
        // Arrange
        var orderId = Guid.NewGuid();
        var menuItemId = Guid.NewGuid();
        var quantity = 2;

        var command = new AddOrderItemCommand(orderId, menuItemId, quantity);
        var order = CreateTestOrder(orderId);
        var menuItem = CreateTestMenuItem(
            menuItemId, 
            10.0m); // Price = 10.0
        var calculatedPrice = 20.0m;

        order.AddOrderItem(
            menuItemId, 
            quantity, 
            calculatedPrice);

        _orderRepositoryMock
            .Setup(repo => repo.GetByIdAsync(
                orderId, 
                It.IsAny<CancellationToken>()))
            .ReturnsAsync(order);

        _menuItemRepositoryMock
            .Setup(repo => repo.GetByIdAsync(
                menuItemId, 
                It.IsAny<CancellationToken>()))
            .ReturnsAsync(menuItem);

        _pricingServiceMock
            .Setup(service => service.CalculatePrice(
                menuItem.Price, 
                quantity))
            .Returns(Result.Success(calculatedPrice));

        _orderItemRepositoryMock
            .Setup(repo => repo.Add(
                It.IsAny<OrderItem>()))
            .Verifiable();

        _unitOfWorkMock
            .Setup(uow => uow.SaveChangesAsync(
                It.IsAny<CancellationToken>()))
            .Returns(Task.CompletedTask);

        // Act
        var result = await _handler.Handle(command, CancellationToken.None);

        // Assert
        result.IsSuccess.Should().BeTrue();

        _orderRepositoryMock.Verify(
            repo => repo.GetByIdAsync(
                orderId, 
                It.IsAny<CancellationToken>()),
            Times.Once);

        _menuItemRepositoryMock.Verify(
            repo => repo.GetByIdAsync(
                menuItemId, 
                It.IsAny<CancellationToken>()),
            Times.Once);

        _pricingServiceMock.Verify(
            service => service.CalculatePrice(
                menuItem.Price, 
                quantity),
            Times.Once);

        _orderItemRepositoryMock.Verify(
            repo => repo.Add(
                It.IsAny<OrderItem>()),
            Times.Once);

        _unitOfWorkMock.Verify(
            uow => uow.SaveChangesAsync(
                It.IsAny<CancellationToken>()),
            Times.Once);
    }

    /// <summary>
    /// Test Case: Returns failure when the order does not exist.
    /// </summary>
    [Fact]
    public async Task Handle_Should_ReturnFailure_WhenOrderDoesNotExist()
    {
        // Arrange
        var orderId = Guid.NewGuid();
        var command = new AddOrderItemCommand(
            orderId, 
            Guid.NewGuid(), 
            2);

        _orderRepositoryMock
            .Setup(repo => repo.GetByIdAsync(
                orderId, 
                It.IsAny<CancellationToken>()))
            .ReturnsAsync((Order)null!);

        // Act
        var result = await _handler.Handle(command, CancellationToken.None);

        // Assert
        result.IsFailure.Should().BeTrue();
        result.Error.Should().Be(DomainErrors.Order.NotFound(orderId));

        _orderRepositoryMock.Verify(
            repo => repo.GetByIdAsync(
                orderId, 
                It.IsAny<CancellationToken>()),
            Times.Once);

        _menuItemRepositoryMock.VerifyNoOtherCalls();
        _pricingServiceMock.VerifyNoOtherCalls();
    }

    /// <summary>
    /// Test Case: Returns failure when the menu item does not exist.
    /// </summary>
    [Fact]
    public async Task Handle_Should_ReturnFailure_WhenMenuItemDoesNotExist()
    {
        // Arrange
        var orderId = Guid.NewGuid();
        var menuItemId = Guid.NewGuid();
        var command = new AddOrderItemCommand(
            orderId,
            menuItemId, 
            2);

        var order = CreateTestOrder(orderId);

        _orderRepositoryMock
            .Setup(repo => repo.GetByIdAsync(
                orderId, 
                It.IsAny<CancellationToken>()))
            .ReturnsAsync(order);

        _menuItemRepositoryMock
            .Setup(repo => repo.GetByIdAsync(
                menuItemId, 
                It.IsAny<CancellationToken>()))
            .ReturnsAsync((MenuItem)null!);

        // Act
        var result = await _handler.Handle(command, CancellationToken.None);

        // Assert
        result.IsFailure.Should().BeTrue();
        result.Error.Should().Be(DomainErrors.MenuItem.NotFound(menuItemId));

        _menuItemRepositoryMock.Verify(
            repo => repo.GetByIdAsync(
                menuItemId, 
                It.IsAny<CancellationToken>()),
            Times.Once);

        _pricingServiceMock.VerifyNoOtherCalls();
    }

    /// <summary>
    /// Test Case: Returns failure when the pricing service fails.
    /// </summary>
    [Fact]
    public async Task Handle_Should_ReturnFailure_WhenPricingServiceFails()
    {
        // Arrange
        var orderId = Guid.NewGuid();
        var menuItemId = Guid.NewGuid();
        var quantity = 2;

        var command = new AddOrderItemCommand(
            orderId, 
            menuItemId, 
            quantity);

        var order = CreateTestOrder(orderId);
        var menuItem = CreateTestMenuItem(
            menuItemId,
            10.0m);

        _orderRepositoryMock
            .Setup(repo => repo.GetByIdAsync(
                orderId, 
                It.IsAny<CancellationToken>()))
            .ReturnsAsync(order);

        _menuItemRepositoryMock
            .Setup(repo => repo.GetByIdAsync(
                menuItemId, 
                It.IsAny<CancellationToken>()))
            .ReturnsAsync(menuItem);

        _pricingServiceMock
            .Setup(service => service.CalculatePrice(
                menuItem.Price, 
                quantity))
            .Returns(Result.Failure<decimal>(DomainErrors.Price.InvalidQuantity));

        // Act
        var result = await _handler.Handle(command, CancellationToken.None);

        // Assert
        result.IsFailure.Should().BeTrue();
        result.Error.Should().Be(DomainErrors.Price.InvalidQuantity);

        _pricingServiceMock.Verify(
            service => service.CalculatePrice(
                menuItem.Price, 
                quantity),
            Times.Once);
    }

    #endregion

    #region Helpers

    /// <summary>
    /// Helper method to create a test order with specified parameters.
    /// </summary>
    private static Order CreateTestOrder(Guid orderId)
    {
        var order = Order.Create(
            orderId, 
            Guid.NewGuid(),
            Guid.NewGuid(),
            DeliveryAddress.Create(
                "street",
                "city",
                "state",
                34.0, 35.0).Value);

        return order;
    }

    /// <summary>
    /// Helper method to create a test menu item with specified parameters.
    /// </summary>
    private static MenuItem CreateTestMenuItem(Guid menuItemId, decimal price)
    {
        var restaurant = Restaurant.Create(
            Guid.NewGuid(),
            "test",
            "test",
            Email.Create("email@test.com").Value,
            "1982234",
            Address.Create("test").Value,
            Guid.NewGuid(),
            OpeningHours.Create(
                new TimeSpan(9, 0, 0),
                new TimeSpan(18, 0, 0))
                .Value);

        var menuItemResult = restaurant.AddMenuItem(
            "test",
            "test",
            price,
            Category.MainCourse);

        return menuItemResult.Value;
    }

    #endregion
}