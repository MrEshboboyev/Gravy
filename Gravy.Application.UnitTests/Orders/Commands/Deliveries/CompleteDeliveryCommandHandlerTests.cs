using FluentAssertions;
using Gravy.Application.Orders.Commands.Deliveries.CompleteDelivery;
using Gravy.Domain.Entities;
using Gravy.Domain.Enums;
using Gravy.Domain.Errors;
using Gravy.Domain.Repositories;
using Gravy.Domain.ValueObjects;
using Moq;

namespace Gravy.Application.UnitTests.Orders.Commands.Deliveries;

public class CompleteDeliveryCommandHandlerTests
{
    #region Fields & Mock Setup

    private readonly Mock<IOrderRepository> _orderRepositoryMock;
    private readonly Mock<IUnitOfWork> _unitOfWorkMock;
    private readonly CompleteDeliveryCommandHandler _handler;

    public CompleteDeliveryCommandHandlerTests()
    {
        _orderRepositoryMock = new Mock<IOrderRepository>();
        _unitOfWorkMock = new Mock<IUnitOfWork>();

        _handler = new CompleteDeliveryCommandHandler(
            _orderRepositoryMock.Object,
            _unitOfWorkMock.Object);
    }

    #endregion

    #region Test Methods

    [Fact]
    public async Task Handle_Should_ReturnFailure_WhenOrderNotFound()
    {
        // Arrange
        var command = new CompleteDeliveryCommand(Guid.NewGuid());

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

        _orderRepositoryMock.Verify(
            repo => repo.GetByIdAsync(
                command.OrderId, 
                It.IsAny<CancellationToken>()),
            Times.Once);

        _unitOfWorkMock.Verify(
            uow => uow.SaveChangesAsync(
                It.IsAny<CancellationToken>()), 
            Times.Never);
    }

    [Fact]
    public async Task Handle_Should_ReturnFailure_WhenCompleteDeliveryFails()
    {
        // Arrange
        var command = new CompleteDeliveryCommand(Guid.NewGuid());
        var order = CreateTestOrder(command.OrderId);

        _orderRepositoryMock
            .Setup(repo => repo.GetByIdAsync(
                command.OrderId, 
                It.IsAny<CancellationToken>()))
            .ReturnsAsync(order);

        // Simulate failure in CompleteDelivery
        // return failure, because delivery not created yet
        order.CompleteDelivery();

        // Act
        var result = await _handler.Handle(command, CancellationToken.None);

        // Assert
        result.IsFailure.Should().BeTrue();
        result.Error.Should().NotBeNull();

        _orderRepositoryMock.Verify(
            repo => repo.GetByIdAsync(
                command.OrderId, 
                It.IsAny<CancellationToken>()), 
            Times.Once);

        _unitOfWorkMock.Verify(
            uow => uow.SaveChangesAsync(
                It.IsAny<CancellationToken>()),
            Times.Never);
    }

    [Fact]
    public async Task Handle_Should_ReturnSuccess_WhenDeliveryIsCompletedSuccessfully()
    {
        // Arrange
        var command = new CompleteDeliveryCommand(Guid.NewGuid());
        var order = CreateTestOrder(command.OrderId);

        _orderRepositoryMock
            .Setup(repo => repo.GetByIdAsync(
                command.OrderId, 
                It.IsAny<CancellationToken>()))
            .ReturnsAsync(order);

        // Simulate success in CompleteDelivery
        #region Set Payment, create delivery and assign delivery before complete delivery 

        order.SetPayment(
            10m,
            PaymentMethod.Card,
            "testId");

        order.CreateDelivery();

        order.AssignDelivery(
            Guid.NewGuid(),
            new TimeSpan(1, 0, 0));

        #endregion

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
                command.OrderId, 
                It.IsAny<CancellationToken>()),
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
        var command = new CompleteDeliveryCommand(Guid.NewGuid());

        _orderRepositoryMock
            .Setup(repo => repo.GetByIdAsync(
                command.OrderId, 
                It.IsAny<CancellationToken>()))
            .ThrowsAsync(new Exception("Repository error"));

        // Act
        Func<Task> act = async () => await _handler.Handle(command, CancellationToken.None);

        // Assert
        await act.Should().ThrowAsync<Exception>()
            .WithMessage("Repository error");

        _unitOfWorkMock.Verify(
            uow => uow.SaveChangesAsync(
                It.IsAny<CancellationToken>()),
            Times.Never);
    }

    [Fact]
    public async Task Handle_Should_NotCallSaveChanges_WhenCompleteDeliveryFails()
    {
        // Arrange
        var command = new CompleteDeliveryCommand(Guid.NewGuid());
        var order = CreateTestOrder(command.OrderId);

        _orderRepositoryMock
            .Setup(repo => repo.GetByIdAsync(
                command.OrderId, 
                It.IsAny<CancellationToken>()))
            .ReturnsAsync(order);

        // return failure, because delivery not created yet
        order.CompleteDelivery(); 

        // Act
        var result = await _handler.Handle(command, CancellationToken.None);

        // Assert
        result.IsFailure.Should().BeTrue();

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
