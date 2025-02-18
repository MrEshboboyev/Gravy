using FluentAssertions;
using Gravy.Application.Orders.Commands.Deliveries.AssignDelivery;
using Gravy.Application.Services.Deliveries;
using Gravy.Domain.Entities;
using Gravy.Domain.Enums;
using Gravy.Domain.Errors;
using Gravy.Domain.Repositories;
using Gravy.Domain.ValueObjects;
using Moq;

namespace Gravy.Application.UnitTests.Orders.Commands.Deliveries;

public class AssignDeliveryCommandHandlerTests
{
    #region Fields & Mock Setup

    private readonly Mock<IOrderRepository> _orderRepositoryMock;
    private readonly Mock<IDeliveryPersonSelector> _deliveryPersonSelectorMock;
    private readonly Mock<IUnitOfWork> _unitOfWorkMock;
    private readonly AssignDeliveryCommandHandler _handler;

    public AssignDeliveryCommandHandlerTests()
    {
        _orderRepositoryMock = new Mock<IOrderRepository>();
        _deliveryPersonSelectorMock = new Mock<IDeliveryPersonSelector>();
        _unitOfWorkMock = new Mock<IUnitOfWork>();

        _handler = new AssignDeliveryCommandHandler(
            _orderRepositoryMock.Object,
            _deliveryPersonSelectorMock.Object,
            _unitOfWorkMock.Object);
    }

    #endregion

    #region Test Methods

    [Fact]
    public async Task Handle_Should_ReturnFailure_WhenOrderNotFound()
    {
        // Arrange
        var command = new AssignDeliveryCommand(Guid.NewGuid());

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

        _deliveryPersonSelectorMock.Verify(
            selector => selector.SelectBestDeliveryPersonAsync(
                It.IsAny<Order>(),
                It.IsAny<CancellationToken>()), 
            Times.Never);

        _unitOfWorkMock.Verify(uow => uow.SaveChangesAsync(
            It.IsAny<CancellationToken>()),
            Times.Never);
    }

    [Fact]
    public async Task Handle_Should_ReturnFailure_WhenNoDeliveryPersonAvailable()
    {
        // Arrange
        var command = new AssignDeliveryCommand(Guid.NewGuid());
        var order = CreateTestOrder(command.OrderId);

        _orderRepositoryMock
            .Setup(repo => repo.GetByIdAsync(
                command.OrderId, 
                It.IsAny<CancellationToken>()))
            .ReturnsAsync(order);

        _deliveryPersonSelectorMock
            .Setup(selector => selector.SelectBestDeliveryPersonAsync(
                order, 
                It.IsAny<CancellationToken>()))
            .ReturnsAsync((DeliveryPerson)null!);

        // Act
        var result = await _handler.Handle(command, CancellationToken.None);

        // Assert
        result.IsFailure.Should().BeTrue();
        result.Error.Should().Be(DomainErrors.Delivery.NoAvailableDeliveryPerson);

        _orderRepositoryMock.Verify(
            repo => repo.GetByIdAsync(
                command.OrderId, 
                It.IsAny<CancellationToken>()), 
            Times.Once);

        _deliveryPersonSelectorMock.Verify(
            selector => selector.SelectBestDeliveryPersonAsync(
                order, 
                It.IsAny<CancellationToken>()), 
            Times.Once);

        _unitOfWorkMock.Verify(uow => uow.SaveChangesAsync(
            It.IsAny<CancellationToken>()), 
            Times.Never);
    }

    [Fact]
    public async Task Handle_Should_ReturnFailure_WhenAssignDeliveryFails()
    {
        // Arrange
        var command = new AssignDeliveryCommand(Guid.NewGuid());
        var order = CreateTestOrder(command.OrderId);
        var deliveryPerson = CreateTestDeliveryPerson(
            Guid.NewGuid());

        _orderRepositoryMock
            .Setup(repo => repo.GetByIdAsync(
                command.OrderId, 
                It.IsAny<CancellationToken>()))
            .ReturnsAsync(order);

        _deliveryPersonSelectorMock
            .Setup(selector => selector.SelectBestDeliveryPersonAsync(
                order, 
                It.IsAny<CancellationToken>()))
            .ReturnsAsync(deliveryPerson);

        // Simulate failure in AssignDelivery
        // return [Delivery not found for this order] error
        order.AssignDelivery(
            Guid.NewGuid(),
            new TimeSpan(1,0,0)); 

        // Act
        var result = await _handler.Handle(command, CancellationToken.None);

        // Assert
        result.IsFailure.Should().BeTrue();
        result.Error.Should().NotBeNull();

        _unitOfWorkMock.Verify(uow => uow.SaveChangesAsync(
            It.IsAny<CancellationToken>()), 
            Times.Never);
    }

    [Fact]
    public async Task Handle_Should_AssignDelivery_WhenSuccessful()
    {
        // Arrange
        var command = new AssignDeliveryCommand(Guid.NewGuid());
        var order = CreateTestOrder(command.OrderId);

        #region Set Payment and create delivery before assigning delivery 

        order.SetPayment(
            10m,
            PaymentMethod.Card,
            "testId");
        var delivery = order.CreateDelivery();

        #endregion

        var deliveryPerson = CreateTestDeliveryPerson(
            Guid.NewGuid());

        _orderRepositoryMock
            .Setup(repo => repo.GetByIdAsync(
                command.OrderId, 
                It.IsAny<CancellationToken>()))
            .ReturnsAsync(order);

        _deliveryPersonSelectorMock
            .Setup(selector => selector.SelectBestDeliveryPersonAsync(
                order, 
                It.IsAny<CancellationToken>()))
            .ReturnsAsync(deliveryPerson);

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

        _deliveryPersonSelectorMock.Verify(
            selector => selector.SelectBestDeliveryPersonAsync(
                order, 
                It.IsAny<CancellationToken>()),
            Times.Once);

        _unitOfWorkMock.Verify(uow => uow.SaveChangesAsync(
            It.IsAny<CancellationToken>()), 
            Times.Once);
    }

    [Fact]
    public async Task Handle_Should_PropagateException_WhenRepositoryThrows()
    {
        // Arrange
        var command = new AssignDeliveryCommand(Guid.NewGuid());

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

        _deliveryPersonSelectorMock.Verify(
            selector => selector.SelectBestDeliveryPersonAsync(
                It.IsAny<Order>(),
                It.IsAny<CancellationToken>()), 
            Times.Never);

        _unitOfWorkMock.Verify(uow => uow.SaveChangesAsync(
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

    /// <summary>
    /// Helper: Creates a test user.
    /// </summary>
    private static User CreateTestUser(Guid userId)
    {
        return User.Create(userId,
            Email.Create("test@email.com").Value,
            "hashedPassword",
            FirstName.Create("John").Value,
            LastName.Create("Doe").Value);
    }

    /// <summary>
    /// Helper: Creates a delivery person.
    /// </summary>
    private static DeliveryPerson CreateTestDeliveryPerson(Guid deliveryPersonId)
    {
        var user = CreateTestUser(Guid.NewGuid());

        return user.AddDeliveryPersonDetails(
            Vehicle.Create("Car","XXX111").Value,
            Location.Create(34.9, 45.7).Value).Value;
    }

    #endregion
}
