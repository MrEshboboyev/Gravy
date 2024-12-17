using FluentAssertions;
using Gravy.Application.Orders.Commands.DeleteOrder;
using Gravy.Domain.Entities;
using Gravy.Domain.Enums;
using Gravy.Domain.Errors;
using Gravy.Domain.Repositories;
using Gravy.Domain.ValueObjects;
using Moq;

namespace Gravy.Application.UnitTests.Orders;

public class DeleteOrderCommandHandlerTests
{
    #region Fields & Mocks

    private readonly DeleteOrderCommandHandler _handler;

    private readonly Mock<IOrderRepository> _orderRepositoryMock;
    private readonly Mock<IUnitOfWork> _unitOfWorkMock;

    public DeleteOrderCommandHandlerTests()
    {
        _orderRepositoryMock = new Mock<IOrderRepository>();
        _unitOfWorkMock = new Mock<IUnitOfWork>();

        _handler = new DeleteOrderCommandHandler(
            _orderRepositoryMock.Object,
            _unitOfWorkMock.Object);
    }

    #endregion

    #region Test Methods

    /// <summary>
    /// Test Case: Successfully deletes an order when it exists and is not locked.
    /// </summary>
    [Fact]
    public async Task Handle_Should_DeleteOrder_WhenOrderExistsAndNotLocked()
    {
        // Arrange
        var orderId = Guid.NewGuid();
        var command = new DeleteOrderCommand(orderId);

        var order = Order.Create(
            orderId,
            Guid.NewGuid(), 
            Guid.NewGuid(), 
            DeliveryAddress.Create(
                "Street",
                "City",
                "State",
                12.34, 
                56.78).Value);

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

        _orderRepositoryMock.Verify(
            repo => repo.GetByIdAsync(
                orderId,
                It.IsAny<CancellationToken>()),
            Times.Once);

        _orderRepositoryMock.Verify(
            repo => repo.Remove(
                It.Is<Order>(o => o == order)),
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
        var command = new DeleteOrderCommand(orderId);

        _orderRepositoryMock
            .Setup(repo => repo.GetByIdAsync(
                orderId,
                It.IsAny<CancellationToken>()))
            .ReturnsAsync((Order)null!); // Simulate order not found

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

        _orderRepositoryMock.Verify(
            repo => repo.Remove(
                It.IsAny<Order>()),
            Times.Never);

        _unitOfWorkMock.Verify(
            uow => uow.SaveChangesAsync(
                It.IsAny<CancellationToken>()),
            Times.Never);
    }

    /// <summary>
    /// Test Case: Returns failure when the order exists but is locked.
    /// </summary>
    [Fact]
    public async Task Handle_Should_ReturnFailure_WhenOrderIsLocked()
    {
        // Arrange
        var orderId = Guid.NewGuid();
        var command = new DeleteOrderCommand(orderId);

        var order = Order.Create(
            orderId, 
            Guid.NewGuid(),
            Guid.NewGuid(),
            DeliveryAddress.Create(
                "Street",
                "City",
                "State",
                12.34,
                56.78).Value);

        // Lock this order using SetPayment() method
        order.SetPayment(
            10.99m,
            PaymentMethod.Card,
            "transactionId");

        _orderRepositoryMock
            .Setup(repo => repo.GetByIdAsync(
                orderId,
                It.IsAny<CancellationToken>()))
            .ReturnsAsync(order);

        // Act
        var result = await _handler.Handle(command, CancellationToken.None);

        // Assert
        result.IsFailure.Should().BeTrue();
        result.Error.Should().Be(DomainErrors.Order.OrderIsLocked);

        _orderRepositoryMock.Verify(
            repo => repo.GetByIdAsync(
                orderId, 
                It.IsAny<CancellationToken>()),
            Times.Once);

        _orderRepositoryMock.Verify(
            repo => repo.Remove(
                It.IsAny<Order>()),
            Times.Never);

        _unitOfWorkMock.Verify(
            uow => uow.SaveChangesAsync(
                It.IsAny<CancellationToken>()),
            Times.Never);
    }

    /// <summary>
    /// Test Case: Ensures Remove and SaveChanges are called exactly once.
    /// </summary>
    [Fact]
    public async Task Handle_Should_CallRemoveAndSaveChanges_ExactlyOnce()
    {
        // Arrange
        var orderId = Guid.NewGuid();
        var command = new DeleteOrderCommand(orderId);

        var order = Order.Create(
            orderId,
            Guid.NewGuid(),
            Guid.NewGuid(),
            DeliveryAddress.Create(
                "Street",
                "City",
                "State",
                12.34,
                56.78).Value);

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
        await _handler.Handle(command, CancellationToken.None);

        // Assert
        _orderRepositoryMock.Verify(
            repo => repo.Remove(
                It.Is<Order>(o => o == order)),
            Times.Once);

        _unitOfWorkMock.Verify(
            uow => uow.SaveChangesAsync(
                It.IsAny<CancellationToken>()),
            Times.Once);
    }

    #endregion
}