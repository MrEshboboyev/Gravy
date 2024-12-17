using FluentAssertions;
using Gravy.Application.Orders.Commands.Deliveries.CreateDelivery;
using Gravy.Domain.Entities;
using Gravy.Domain.Enums;
using Gravy.Domain.Errors;
using Gravy.Domain.Repositories;
using Gravy.Domain.ValueObjects;
using Moq;

namespace Gravy.Application.UnitTests.Orders.Commands.Deliveries;

public class CreateDeliveryCommandHandlerTests
{
    #region Fields & Mock Setup

    private readonly Mock<IOrderRepository> _orderRepositoryMock;
    private readonly Mock<IDeliveryRepository> _deliveryRepositoryMock;
    private readonly Mock<IUnitOfWork> _unitOfWorkMock;
    private readonly CreateDeliveryCommandHandler _handler;

    public CreateDeliveryCommandHandlerTests()
    {
        _orderRepositoryMock = new Mock<IOrderRepository>();
        _deliveryRepositoryMock = new Mock<IDeliveryRepository>();
        _unitOfWorkMock = new Mock<IUnitOfWork>();

        _handler = new CreateDeliveryCommandHandler(
            _orderRepositoryMock.Object,
            _deliveryRepositoryMock.Object,
            _unitOfWorkMock.Object);
    }

    #endregion

    #region Test Methods

    [Fact]
    public async Task Handle_Should_ReturnFailure_WhenOrderNotFound()
    {
        // Arrange
        var command = new CreateDeliveryCommand(Guid.NewGuid());

        _orderRepositoryMock
            .Setup(repo => repo.GetByIdAsync(
                command.OrderId, 
                It.IsAny<CancellationToken>()))
            .ReturnsAsync((Order)null!); // Simulate order not found

        // Act
        var result = await _handler.Handle(command, CancellationToken.None);

        // Assert
        result.IsFailure.Should().BeTrue();
        result.Error.Should().Be(DomainErrors.Order.NotFound(command.OrderId));

        _orderRepositoryMock.Verify(
            repo => repo.GetByIdAsync(
                command.OrderId,
                It.IsAny<CancellationToken>()),
            Times.Once);

        _deliveryRepositoryMock.Verify(
            repo => repo.Add(
                It.IsAny<Delivery>()),
            Times.Never);

        _unitOfWorkMock.Verify(
            uow => uow.SaveChangesAsync(
                It.IsAny<CancellationToken>()),
            Times.Never);
    }

    [Fact]
    public async Task Handle_Should_ReturnFailure_WhenDeliveryCreationFails()
    {
        // Arrange
        var command = new CreateDeliveryCommand(Guid.NewGuid());
        var order = CreateTestOrder(command.OrderId);

        // Simulate Delivery Creation Failure (example : payment is not set)
        order.CreateDelivery();

        _orderRepositoryMock
            .Setup(repo => repo.GetByIdAsync(
                command.OrderId, 
                It.IsAny<CancellationToken>()))
            .ReturnsAsync(order);

        // Act
        var result = await _handler.Handle(command, CancellationToken.None);

        // Assert
        result.IsFailure.Should().BeTrue();
        result.Error.Should().NotBeNull();

        _deliveryRepositoryMock.Verify(
            repo => repo.Add(
                It.IsAny<Delivery>()),
            Times.Never);

        _unitOfWorkMock.Verify(
            uow => uow.SaveChangesAsync(
                It.IsAny<CancellationToken>()),
            Times.Never);
    }

    [Fact]
    public async Task Handle_Should_CreateDelivery_WhenOrderAndCreationSucceed()
    {
        // Arrange
        var command = new CreateDeliveryCommand(Guid.NewGuid());
        var order = CreateTestOrder(command.OrderId);

        // set payment for this order, because setting payment
        // before create delivery in real world
        order.SetPayment(
            10m,
            PaymentMethod.Card, 
            "testId");

        _orderRepositoryMock
            .Setup(repo => repo.GetByIdAsync(
                command.OrderId, 
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
        result.Value.Should().NotBeEmpty();

        _orderRepositoryMock.Verify(
            repo => repo.GetByIdAsync(
                command.OrderId, 
                It.IsAny<CancellationToken>()),
            Times.Once);

        _deliveryRepositoryMock.Verify(
            repo => repo.Add(
                It.IsAny<Delivery>()),
            Times.Once);

        _unitOfWorkMock.Verify(
            uow => uow.SaveChangesAsync(
                It.IsAny<CancellationToken>()),
            Times.Once);
    }

    [Fact]
    public async Task Handle_Should_PropagateException_WhenRepositoryThrows()
    {
        // Arrange
        var command = new CreateDeliveryCommand(Guid.NewGuid());

        _orderRepositoryMock
            .Setup(repo => repo.GetByIdAsync(
                It.IsAny<Guid>(),
                It.IsAny<CancellationToken>()))
            .ThrowsAsync(new Exception("Database error"));

        // Act
        Func<Task> act = async () => await _handler.Handle(command, CancellationToken.None);

        // Assert
        await act.Should().ThrowAsync<Exception>()
            .WithMessage("Database error");

        _deliveryRepositoryMock.Verify(
            repo => repo.Add(
                It.IsAny<Delivery>()),
            Times.Never);

        _unitOfWorkMock.Verify(
            uow => uow.SaveChangesAsync(
                It.IsAny<CancellationToken>()),
            Times.Never);
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

    #endregion
}
