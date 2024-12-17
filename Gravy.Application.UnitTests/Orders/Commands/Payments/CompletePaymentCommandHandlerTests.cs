using FluentAssertions;
using Gravy.Application.Orders.Commands.Payments.CompletePayment;
using Gravy.Domain.Entities;
using Gravy.Domain.Enums;
using Gravy.Domain.Errors;
using Gravy.Domain.Repositories;
using Gravy.Domain.ValueObjects;
using Moq;

namespace Gravy.Application.UnitTests.Orders.Commands.Payments;

public class CompletePaymentCommandHandlerTests
{
    #region Fields & Mock Setup

    private readonly Mock<IOrderRepository> _orderRepositoryMock;
    private readonly Mock<IUnitOfWork> _unitOfWorkMock;
    private readonly CompletePaymentCommandHandler _handler;

    public CompletePaymentCommandHandlerTests()
    {
        _orderRepositoryMock = new Mock<IOrderRepository>();
        _unitOfWorkMock = new Mock<IUnitOfWork>();
        _handler = new CompletePaymentCommandHandler(
            _orderRepositoryMock.Object, 
            _unitOfWorkMock.Object);
    }

    #endregion

    #region Test Methods

    /// <summary>
    /// Test case: Order is not found.
    /// </summary>
    [Fact]
    public async Task Handle_Should_ReturnFailure_WhenOrderNotFound()
    {
        // Arrange
        var command = new CompletePaymentCommand(Guid.NewGuid());
        _orderRepositoryMock
            .Setup(repo => repo.GetByIdAsync(
                command.OrderId, 
                It.IsAny<CancellationToken>()))
            .ReturnsAsync((Order)null!);

        // Act
        var result = await _handler.Handle(command, CancellationToken.None);

        // Assert
        result.IsSuccess.Should().BeFalse();
        result.Error.Should().Be(DomainErrors.Order.NotFound(command.OrderId));

        _orderRepositoryMock.Verify(repo => repo.GetByIdAsync(
            command.OrderId, 
            It.IsAny<CancellationToken>()),
            Times.Once);

        _unitOfWorkMock.Verify(uow => uow.SaveChangesAsync(
            It.IsAny<CancellationToken>()), 
            Times.Never);
    }

    /// <summary>
    /// Test case: Order exists, but completing payment fails.
    /// </summary>
    [Fact]
    public async Task Handle_Should_ReturnFailure_WhenCompletePaymentFails()
    {
        // Arrange
        var command = new CompletePaymentCommand(Guid.NewGuid());
        var order = CreateValidOrderForPayment(command.OrderId);

        // Simulate failure when CompletePayment is called
        // because payment is not set

        _orderRepositoryMock
            .Setup(repo => repo.GetByIdAsync(
                command.OrderId, 
                It.IsAny<CancellationToken>()))
            .ReturnsAsync(order);

        // Act
        var result = await _handler.Handle(command, CancellationToken.None);

        // Assert
        result.IsSuccess.Should().BeFalse();
        result.Error.Should().NotBeNull();

        _orderRepositoryMock.Verify(repo => repo.GetByIdAsync(
            command.OrderId, 
            It.IsAny<CancellationToken>()),
            Times.Once);

        _unitOfWorkMock.Verify(uow => uow.SaveChangesAsync(
            It.IsAny<CancellationToken>()), 
            Times.Never);
    }

    /// <summary>
    /// Test case: Successfully completes payment for an order.
    /// </summary>
    [Fact]
    public async Task Handle_Should_ReturnSuccess_WhenPaymentIsCompletedSuccessfully()
    {
        // Arrange
        var command = new CompletePaymentCommand(Guid.NewGuid());
        var order = CreateValidOrderForPayment(command.OrderId);

        // Simulate success when CompletePayment is called
        // set payment
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

        _orderRepositoryMock.Verify(repo => repo.GetByIdAsync(
            command.OrderId, 
            It.IsAny<CancellationToken>()),
            Times.Once);

        _unitOfWorkMock.Verify(uow => uow.SaveChangesAsync(
            It.IsAny<CancellationToken>()),
            Times.Once);
    }

    #endregion

    #region Helpers

    /// <summary>
    /// Helper method to create an Order with incomplete payment.
    /// </summary>
    private static Order CreateValidOrderForPayment(Guid orderId)
    {
        return Order.Create(
            orderId, Guid.NewGuid(),
            Guid.NewGuid(),
            DeliveryAddress.Create(
                "street",
                "city",
                "state",
                34.0,
                67.4).Value);
    }

    #endregion
}