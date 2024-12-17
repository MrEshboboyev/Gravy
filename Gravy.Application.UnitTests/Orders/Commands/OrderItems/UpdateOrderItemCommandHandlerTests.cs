using FluentAssertions;
using Gravy.Application.Orders.Commands.OrderItems.UpdateOrderItem;
using Gravy.Application.Services.Orders.Interfaces;
using Gravy.Domain.Entities;
using Gravy.Domain.Enums;
using Gravy.Domain.Errors;
using Gravy.Domain.Repositories;
using Gravy.Domain.Shared;
using Gravy.Domain.ValueObjects;
using Moq;

namespace Gravy.Application.UnitTests.Orders.Commands.OrderItems;

public class UpdateOrderItemCommandHandlerTests
{
    #region Fields and Mocks

    private readonly UpdateOrderItemCommandHandler _handler;

    private readonly Mock<IOrderRepository> _orderRepositoryMock;
    private readonly Mock<IOrderItemRepository> _orderItemRepositoryMock;
    private readonly Mock<IUnitOfWork> _unitOfWorkMock;
    private readonly Mock<IPricingService> _pricingServiceMock;
    private readonly Mock<IMenuItemRepository> _menuItemRepositoryMock;

    public UpdateOrderItemCommandHandlerTests()
    {
        _orderRepositoryMock = new Mock<IOrderRepository>();
        _orderItemRepositoryMock = new Mock<IOrderItemRepository>();
        _unitOfWorkMock = new Mock<IUnitOfWork>();
        _pricingServiceMock = new Mock<IPricingService>();
        _menuItemRepositoryMock = new Mock<IMenuItemRepository>();

        _handler = new UpdateOrderItemCommandHandler(
            _orderRepositoryMock.Object,
            _orderItemRepositoryMock.Object,
            _unitOfWorkMock.Object,
            _pricingServiceMock.Object,
            _menuItemRepositoryMock.Object);
    }

    #endregion

    #region Test Methods

    /// <summary>
    /// Successfully updates an order item when all inputs are valid.
    /// </summary>
    [Fact]
    public async Task Handle_Should_UpdateOrderItem_WhenAllInputsAreValid()
    {
        // Arrange
        var orderId = Guid.NewGuid();
        var menuItemId = Guid.NewGuid();
        var quantity = 3;
        var menuItemPrice = 10.0m;
        var finalPrice = 30.0m;

        var order = CreateTestOrder(orderId);

        var addOrderItemResult = order.AddOrderItem(
            menuItemId,
            2,
            20.0m);

        var menuItem = CreateTestMenuItem(
            menuItemId,
            menuItemPrice);

        var command = new UpdateOrderItemCommand(
            orderId,
            addOrderItemResult.Value.Id,
            quantity);

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
            .Returns(Result.Success(finalPrice));

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
            repo => repo.Update(
                It.IsAny<OrderItem>()),
            Times.Once);

        _unitOfWorkMock.Verify(
            uow => uow.SaveChangesAsync(
                It.IsAny<CancellationToken>()),
            Times.Once);
    }

    /// <summary>
    /// Returns failure when the order does not exist.
    /// </summary>
    [Fact]
    public async Task Handle_Should_ReturnFailure_WhenOrderDoesNotExist()
    {
        // Arrange
        var orderId = Guid.NewGuid();
        var command = new UpdateOrderItemCommand(
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
        result.Error.Should().Be(DomainErrors.Order.NotFound(command.OrderItemId));
    }

    /// <summary>
    /// Returns failure when the order item does not exist in the order.
    /// </summary>
    [Fact]
    public async Task Handle_Should_ReturnFailure_WhenOrderItemDoesNotExist()
    {
        // Arrange
        var orderId = Guid.NewGuid();
        var orderItemId = Guid.NewGuid();

        var order = CreateTestOrder(orderId);

        var command = new UpdateOrderItemCommand(
            orderId,
            orderItemId,
            2);

        _orderRepositoryMock
            .Setup(repo => repo.GetByIdAsync(
                orderId,
                It.IsAny<CancellationToken>()))
            .ReturnsAsync(order);

        // Act
        var result = await _handler.Handle(command, CancellationToken.None);

        // Assert
        result.IsFailure.Should().BeTrue();
        result.Error.Should().Be(DomainErrors.OrderItem.NotFound(orderItemId));
    }

    /// <summary>
    /// Returns failure when the menu item does not exist.
    /// </summary>
    [Fact]
    public async Task Handle_Should_ReturnFailure_WhenMenuItemDoesNotExist()
    {
        // Arrange
        var orderId = Guid.NewGuid();
        var menuItemId = Guid.NewGuid();

        var order = CreateTestOrder(orderId);

        var addOrderItemResult = order.AddOrderItem(
            menuItemId,
            2,
            20.0m);

        var command = new UpdateOrderItemCommand(
            orderId,
            addOrderItemResult.Value.Id,
            3);

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
    }

    /// <summary>
    /// Returns failure when the pricing service fails.
    /// </summary>
    [Fact]
    public async Task Handle_Should_ReturnFailure_WhenPricingServiceFails()
    {
        // Arrange
        var orderId = Guid.NewGuid();
        var menuItemId = Guid.NewGuid();

        var order = CreateTestOrder(orderId);

        var addOrderItemResult = order.AddOrderItem(
            menuItemId,
            2,
            20.0m);

        var menuItem = CreateTestMenuItem(
            menuItemId,
            10.0m);

        var command = new UpdateOrderItemCommand(
            orderId,
            addOrderItemResult.Value.Id,
            3);

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
                3))
            .Returns(Result.Failure<decimal>(DomainErrors.Price.InvalidQuantity));

        // Act
        var result = await _handler.Handle(command, CancellationToken.None);

        // Assert
        result.IsFailure.Should().BeTrue();
        result.Error.Should().Be(DomainErrors.Price.InvalidQuantity);
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