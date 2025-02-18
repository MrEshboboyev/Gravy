using FluentAssertions;
using Gravy.Application.Orders.Commands.OrderItems.RemoveOrderItem;
using Gravy.Domain.Entities;
using Gravy.Domain.Enums;
using Gravy.Domain.Errors;
using Gravy.Domain.Repositories;
using Gravy.Domain.ValueObjects;
using Moq;

namespace Gravy.Application.UnitTests.Orders.Commands.OrderItems;

public class RemoveOrderItemCommandHandlerTests
{
    #region Fields and Mocks

    private readonly RemoveOrderItemCommandHandler _handler;

    private readonly Mock<IOrderRepository> _orderRepositoryMock;
    private readonly Mock<IUnitOfWork> _unitOfWorkMock;

    public RemoveOrderItemCommandHandlerTests()
    {
        _orderRepositoryMock = new Mock<IOrderRepository>();
        _unitOfWorkMock = new Mock<IUnitOfWork>();

        _handler = new RemoveOrderItemCommandHandler(
            _orderRepositoryMock.Object,
            _unitOfWorkMock.Object);
    }

    #endregion

    #region Test Methods

    /// <summary>
    /// Successfully removes an order item when the order and item exist.
    /// </summary>
    [Fact]
    public async Task Handle_Should_RemoveOrderItem_WhenOrderAndItemExist()
    {
        // Arrange
        var orderId = Guid.NewGuid();
        var menuItemId = Guid.NewGuid();

        var order = CreateTestOrder(orderId);
        var addOrderItemResult = order.AddOrderItem(
            menuItemId,
            1,
            10m);

        var command = new RemoveOrderItemCommand(
            orderId,
            addOrderItemResult.Value.Id);

        _orderRepositoryMock
            .Setup(repo => repo.GetByIdAsync(
                orderId,
                It.IsAny<CancellationToken>()))
            .ReturnsAsync(order);

        _unitOfWorkMock
            .Setup(uow => uow.SaveChangesAsync(
                It.IsAny<CancellationToken>()))
            .Returns(Task.CompletedTask);

        // Act
        var result = await _handler.Handle(command, CancellationToken.None);

        // Assert
        result.IsSuccess.Should().BeTrue();

        // Ensure the order no longer contains the item
        order.OrderItems.Any(oi => oi.Id == addOrderItemResult.Value.Id)
            .Should().BeFalse();

        _orderRepositoryMock.Verify(
            repo => repo.GetByIdAsync(
                orderId,
                It.IsAny<CancellationToken>()),
            Times.Once);

        _unitOfWorkMock.Verify(
            uow => uow.SaveChangesAsync(
                It.IsAny<CancellationToken>()),
            Times.Once);
    }

    /// <summary>
    /// Returns failure when the order does not exist in the repository.
    /// </summary>
    [Fact]
    public async Task Handle_Should_ReturnFailure_WhenOrderDoesNotExist()
    {
        // Arrange
        var orderId = Guid.NewGuid();
        var command = new RemoveOrderItemCommand(
            orderId,
            Guid.NewGuid());

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

        _unitOfWorkMock.Verify(
            uow => uow.SaveChangesAsync(
                It.IsAny<CancellationToken>()),
            Times.Never);
    }

    /// <summary>
    /// Returns failure when the RemoveOrderItem method fails.
    /// </summary>
    [Fact]
    public async Task Handle_Should_ReturnFailure_WhenOrderItemCannotBeRemoved()
    {
        // Arrange
        var orderId = Guid.NewGuid();
        var orderItemId = Guid.NewGuid();

        var command = new RemoveOrderItemCommand(
            orderId,
            orderItemId);

        var order = CreateTestOrder(orderId);
        var removalError = DomainErrors.OrderItem.NotFound(orderItemId);

        _orderRepositoryMock
            .Setup(repo => repo.GetByIdAsync(
                orderId,
                It.IsAny<CancellationToken>()))
            .ReturnsAsync(order);

        // Act
        var result = await _handler.Handle(command, CancellationToken.None);

        // Assert
        result.IsFailure.Should().BeTrue();
        result.Error.Should().Be(removalError);

        _orderRepositoryMock.Verify(
            repo => repo.GetByIdAsync(orderId,
                It.IsAny<CancellationToken>()),
            Times.Once);

        _unitOfWorkMock.Verify(
            uow => uow.SaveChangesAsync(
                It.IsAny<CancellationToken>()),
            Times.Never);
    }

    /// <summary>
    /// Verifies that SaveChangesAsync is called exactly once on success.
    /// </summary>
    [Fact]
    public async Task Handle_Should_CallUnitOfWorkSaveChanges_Once_OnSuccess()
    {
        // Arrange
        var orderId = Guid.NewGuid();
        var menuItemId = Guid.NewGuid();

        var order = CreateTestOrder(orderId);
        var addOrderItemResult = order.AddOrderItem(
            menuItemId,
            1,
            10m);

        var command = new RemoveOrderItemCommand(
            orderId,
            addOrderItemResult.Value.Id);


        _orderRepositoryMock
            .Setup(repo => repo.GetByIdAsync(
                orderId,
                It.IsAny<CancellationToken>()))
            .ReturnsAsync(order);

        _unitOfWorkMock
            .Setup(uow => uow.SaveChangesAsync(
                It.IsAny<CancellationToken>()))
            .Returns(Task.CompletedTask);

        // Act
        var result = await _handler.Handle(command, CancellationToken.None);

        // Assert
        _unitOfWorkMock.Verify(
            uow => uow.SaveChangesAsync(
                It.IsAny<CancellationToken>()),
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