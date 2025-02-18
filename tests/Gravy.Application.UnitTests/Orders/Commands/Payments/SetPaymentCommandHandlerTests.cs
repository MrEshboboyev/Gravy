using FluentAssertions;
using Gravy.Application.Orders.Commands.Payments.SetPayment;
using Gravy.Application.Services.Orders.Interfaces;
using Gravy.Domain.Entities;
using Gravy.Domain.Enums;
using Gravy.Domain.Errors;
using Gravy.Domain.Repositories;
using Gravy.Domain.Shared;
using Gravy.Domain.ValueObjects;
using Moq;

namespace Gravy.Application.UnitTests.Orders.Commands.Payments;

public class SetPaymentCommandHandlerTests
{
    #region Fields & Mock Setup

    private readonly Mock<IOrderRepository> _orderRepositoryMock;
    private readonly Mock<IPaymentRepository> _paymentRepositoryMock;
    private readonly Mock<IOrderPricingService> _orderPricingServiceMock;
    private readonly Mock<IUnitOfWork> _unitOfWorkMock;
    private readonly SetPaymentCommandHandler _handler;

    public SetPaymentCommandHandlerTests()
    {
        _orderRepositoryMock = new Mock<IOrderRepository>();
        _paymentRepositoryMock = new Mock<IPaymentRepository>();
        _orderPricingServiceMock = new Mock<IOrderPricingService>();
        _unitOfWorkMock = new Mock<IUnitOfWork>();

        _handler = new SetPaymentCommandHandler(
            _orderRepositoryMock.Object,
            _paymentRepositoryMock.Object,
            _orderPricingServiceMock.Object,
            _unitOfWorkMock.Object);
    }

    #endregion

    #region Test Methods

    [Fact]
    public async Task Handle_Should_ReturnFailure_WhenOrderNotFound()
    {
        // Arrange
        var command = new SetPaymentCommand(
            Guid.NewGuid(), 
            PaymentMethod.Card, 
            "TXN12345");

        _orderRepositoryMock
            .Setup(repo => repo.GetByIdAsync(
                command.OrderId, 
                It.IsAny<CancellationToken>()))
            .ReturnsAsync((Order)null!);

        // Act
        var result = await _handler.Handle(command, CancellationToken.None);

        // Assert
        result.IsFailure.Should().BeTrue();
        result.Error.Should().Be(DomainErrors.Order.NotFound(command.OrderId));

        _orderRepositoryMock.Verify(repo => repo.GetByIdAsync(
            command.OrderId, 
            It.IsAny<CancellationToken>()),
            Times.Once);
    }

    [Fact]
    public async Task Handle_Should_ReturnFailure_WhenPricingServiceFails()
    {
        // Arrange
        var command = new SetPaymentCommand(
            Guid.NewGuid(), 
            PaymentMethod.Card,
            "TXN12345");
        var order = CreateValidOrder(command.OrderId);

        _orderRepositoryMock
            .Setup(repo => repo.GetByIdAsync(
                command.OrderId, 
                It.IsAny<CancellationToken>()))
            .ReturnsAsync(order);

        _orderPricingServiceMock
            .Setup(service => service.CalculateOrderTotalAsync(
                order.Id, 
                It.IsAny<CancellationToken>()))
            .ReturnsAsync(Result.Failure<decimal>(DomainErrors.Order.NoOrderItems));

        // Act
        var result = await _handler.Handle(command, CancellationToken.None);

        // Assert
        result.IsFailure.Should().BeTrue();
        result.Error.Should().Be(DomainErrors.Order.NoOrderItems);
    }

    [Fact]
    public async Task Handle_Should_ReturnFailure_WhenSetPaymentFails()
    {
        // Arrange
        var command = new SetPaymentCommand(
            Guid.NewGuid(), 
            PaymentMethod.Card,
            "TXN12345");
        var order = CreateValidOrder(command.OrderId);

        // prepare failure because payment already set error
        order.SetPayment(
            10m,
            PaymentMethod.Card,
            "testId");

        _orderRepositoryMock
            .Setup(repo => repo.GetByIdAsync(
                command.OrderId, 
                It.IsAny<CancellationToken>()))
            .ReturnsAsync(order);

        _orderPricingServiceMock
            .Setup(service => service.CalculateOrderTotalAsync(
                order.Id, 
                It.IsAny<CancellationToken>()))
            .ReturnsAsync(Result.Success(100m));

        // Act
        var result = await _handler.Handle(command, CancellationToken.None);

        // Assert
        result.IsFailure.Should().BeTrue();

        // Simulate failure inside SetPayment
        order.SetPayment(100m, command.Method, command.TransactionId)
            .Error.Should().NotBeNull();
    }

    [Fact]
    public async Task Handle_Should_ReturnSuccess_WhenAllStepsSucceed()
    {
        // Arrange
        var command = new SetPaymentCommand(
            Guid.NewGuid(),
            PaymentMethod.Card, 
            "TXN12345");
        var order = CreateValidOrder(command.OrderId);

        _orderRepositoryMock
            .Setup(repo => repo.GetByIdAsync(
                command.OrderId, 
                It.IsAny<CancellationToken>()))
            .ReturnsAsync(order);

        _orderPricingServiceMock
            .Setup(service => service.CalculateOrderTotalAsync(
                order.Id, 
                It.IsAny<CancellationToken>()))
            .ReturnsAsync(Result.Success(100m));

        _paymentRepositoryMock
            .Setup(repo => repo.Add(
                It.IsAny<Payment>()));

        _unitOfWorkMock
            .Setup(uow => uow.SaveChangesAsync(
                It.IsAny<CancellationToken>()))
            .Returns(Task.CompletedTask);

        // Act
        var result = await _handler.Handle(command, CancellationToken.None);

        // Assert
        result.IsSuccess.Should().BeTrue();

        _orderPricingServiceMock.Verify(service =>
            service.CalculateOrderTotalAsync(
                order.Id, 
                It.IsAny<CancellationToken>()), 
            Times.Once);

        _paymentRepositoryMock.Verify(repo =>
            repo.Add(It.Is<Payment>(p =>
                p.TransactionId == command.TransactionId && 
                p.Amount == 100m && 
                p.Method == command.Method)), 
            Times.Once);

        _orderRepositoryMock.Verify(repo =>
            repo.Update(order), 
            Times.Once);

        _unitOfWorkMock.Verify(uow =>
            uow.SaveChangesAsync(
                It.IsAny<CancellationToken>()), 
            Times.Once);
    }

    #endregion

    #region Helpers

    /// <summary>
    /// Creates a valid Order instance with basic required fields.
    /// </summary>
    private static Order CreateValidOrder(Guid orderId)
    {
        var order = Order.Create(
            orderId, 
            Guid.NewGuid(),
            Guid.NewGuid(),
            DeliveryAddress.Create(
                "street",
                "city",
                "state",
                34.0,
                67.4).Value);

        // create delivery for this order
        order.CreateDelivery();

        return order;
    }

    #endregion
}
