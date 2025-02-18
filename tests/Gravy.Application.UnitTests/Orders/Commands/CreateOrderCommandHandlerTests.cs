using FluentAssertions;
using Gravy.Application.Orders.Commands.CreateOrder;
using Gravy.Domain.Entities;
using Gravy.Domain.Errors;
using Gravy.Domain.Repositories;
using Gravy.Domain.ValueObjects;
using Moq;

namespace Gravy.Application.UnitTests.Orders.Commands;

public class CreateOrderCommandHandlerTests
{
    #region Fields & Mocks

    private readonly CreateOrderCommandHandler _handler;

    private readonly Mock<IUserRepository> _userRepositoryMock;
    private readonly Mock<IRestaurantRepository> _restaurantRepositoryMock;
    private readonly Mock<IOrderRepository> _orderRepositoryMock;
    private readonly Mock<IUnitOfWork> _unitOfWorkMock;

    public CreateOrderCommandHandlerTests()
    {
        _userRepositoryMock = new Mock<IUserRepository>();
        _restaurantRepositoryMock = new Mock<IRestaurantRepository>();
        _orderRepositoryMock = new Mock<IOrderRepository>();
        _unitOfWorkMock = new Mock<IUnitOfWork>();

        _handler = new CreateOrderCommandHandler(
            _orderRepositoryMock.Object,
            _restaurantRepositoryMock.Object,
            _userRepositoryMock.Object,
            _unitOfWorkMock.Object);
    }

    #endregion

    #region Test Methods

    /// <summary>
    /// Test Case: Successfully creates an order when all inputs are valid.
    /// </summary>
    [Fact]
    public async Task Handle_Should_CreateOrder_WhenAllInputsAreValid()
    {
        // Arrange
        var userId = Guid.NewGuid();
        var restaurantId = Guid.NewGuid();
        var command = new CreateOrderCommand(
            userId,
            restaurantId,
            "Street",
            "City",
            "State",
            12.34,
            56.78);

        var user = User.Create(
            userId,
            Email.Create("test@test.com").Value,
            "password-hash",
            FirstName.Create("firstname").Value,
            LastName.Create("lastName").Value);

        var customer = user.AddCustomerDetails(
            DeliveryAddress.Create(
                "street",
                "city",
                "state",
                41.1,
                68.1).Value);

        var restaurant = Restaurant.Create(
            restaurantId,
            "Test Restaurant",
            "Best Test Restaurant",
            Email.Create("test@example.com").Value,
            "1234567890",
            Address.Create("123 Test Street").Value,
            Guid.NewGuid(),
            OpeningHours.Create(
                new TimeSpan(9, 0, 0), // 9:00 AM
                new TimeSpan(17, 0, 0) // 5:00 PM
            ).Value);

        // Mock successful delivery address creation
        var deliveryAddress = DeliveryAddress.Create(
            "Street",
            "City",
            "State",
            12.34,
            56.78).Value;

        _userRepositoryMock
            .Setup(repo => repo.GetByIdWithCustomerDetailsAsync(
                userId,
                It.IsAny<CancellationToken>()))
            .ReturnsAsync(user);

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
        result.Value.Should().NotBe(Guid.Empty);

        _userRepositoryMock.Verify(
            repo => repo.GetByIdWithCustomerDetailsAsync(
                userId,
                It.IsAny<CancellationToken>()),
            Times.Once);

        _restaurantRepositoryMock.Verify(
            repo => repo.GetByIdAsync(
                restaurantId,
                It.IsAny<CancellationToken>()),
            Times.Once);

        _orderRepositoryMock.Verify(
            repo => repo.Add(
                It.IsAny<Order>()),
            Times.Once);

        _unitOfWorkMock.Verify(
            uow => uow.SaveChangesAsync(
                It.IsAny<CancellationToken>()),
            Times.Once);
    }

    /// <summary>
    /// Test Case: Returns failure when the user is not found.
    /// </summary>
    [Fact]
    public async Task Handle_Should_ReturnFailure_WhenUserNotFound()
    {
        // Arrange
        var userId = Guid.NewGuid();
        var restaurantId = Guid.NewGuid();
        var command = new CreateOrderCommand(
            userId,
            restaurantId,
            "Street",
            "City",
            "State",
            12.34,
            56.78);

        _userRepositoryMock
            .Setup(repo => repo.GetByIdWithCustomerDetailsAsync(
                userId,
                It.IsAny<CancellationToken>()))
            .ReturnsAsync((User)null!); // Simulate user not found

        // Act
        var result = await _handler.Handle(command, CancellationToken.None);

        // Assert
        result.IsFailure.Should().BeTrue();
        result.Error.Should().Be(DomainErrors.User.NotFound(userId));

        _userRepositoryMock.Verify(
            repo => repo.GetByIdWithCustomerDetailsAsync(
                userId,
                It.IsAny<CancellationToken>()),
            Times.Once);

        _restaurantRepositoryMock.Verify(
            repo => repo.GetByIdAsync(
                It.IsAny<Guid>(),
                It.IsAny<CancellationToken>()),
            Times.Never);

        _orderRepositoryMock.Verify(
            repo => repo.Add(
                It.IsAny<Order>()),
            Times.Never);

        _unitOfWorkMock.Verify(
            uow => uow.SaveChangesAsync(
                It.IsAny<CancellationToken>()),
            Times.Never);
    }

    /// <summary>
    /// Test Case: Returns failure when restaurant is not found.
    /// </summary>
    [Fact]
    public async Task Handle_Should_ReturnFailure_WhenRestaurantNotFound()
    {
        // Arrange
        var userId = Guid.NewGuid();
        var restaurantId = Guid.NewGuid();
        var command = new CreateOrderCommand(
            userId,
            restaurantId,
            "Street",
            "City",
            "State",
            12.34,
            56.78);

        var user = User.Create(
            userId,
            Email.Create("test@test.com").Value,
            "password-hash",
            FirstName.Create("firstname").Value,
            LastName.Create("lastName").Value);

        var customer = user.AddCustomerDetails(
            DeliveryAddress.Create(
                "street",
                "city",
                "state",
                41.1,
                68.1).Value);

        _userRepositoryMock
            .Setup(repo => repo.GetByIdWithCustomerDetailsAsync(
                userId,
                It.IsAny<CancellationToken>()))
            .ReturnsAsync(user);

        _restaurantRepositoryMock
            .Setup(repo => repo.GetByIdAsync(
                restaurantId,
                It.IsAny<CancellationToken>()))
            .ReturnsAsync((Restaurant)null!); // Simulate restaurant not found

        // Act
        var result = await _handler.Handle(command, CancellationToken.None);

        // Assert
        result.IsFailure.Should().BeTrue();
        result.Error.Should().Be(DomainErrors.Restaurant.NotFound(restaurantId));

        _userRepositoryMock.Verify(
            repo => repo.GetByIdWithCustomerDetailsAsync(
                userId,
                It.IsAny<CancellationToken>()),
            Times.Once);

        _restaurantRepositoryMock.Verify(
            repo => repo.GetByIdAsync(
                restaurantId,
                It.IsAny<CancellationToken>()),
            Times.Once);

        _orderRepositoryMock.Verify(
            repo => repo.Add(
                It.IsAny<Order>()),
            Times.Never);

        _unitOfWorkMock.Verify(
            uow => uow.SaveChangesAsync(
                It.IsAny<CancellationToken>()),
            Times.Never);
    }

    /// <summary>
    /// Test Case: Returns failure when delivery address creation fails.
    /// </summary>
    [Fact]
    public async Task Handle_Should_ReturnFailure_WhenDeliveryAddressIsInvalid()
    {
        // Arrange
        var userId = Guid.NewGuid();
        var restaurantId = Guid.NewGuid();
        var command = new CreateOrderCommand(
            userId,
            restaurantId,
            "",
            "",
            "",
            -999,
            -999);

        var user = User.Create(
            userId,
            Email.Create("test@test.com").Value,
            "password-hash",
            FirstName.Create("firstname").Value,
            LastName.Create("lastName").Value);

        var customer = user.AddCustomerDetails(
            DeliveryAddress.Create(
                "street",
                "city",
                "state",
                41.1,
                68.1).Value);

        var restaurant = Restaurant.Create(
            restaurantId,
            "Test Restaurant",
            "Best Test Restaurant",
            Email.Create("test@example.com").Value,
            "1234567890",
            Address.Create("123 Test Street").Value,
            Guid.NewGuid(),
            OpeningHours.Create(
                new TimeSpan(9, 0, 0), // 9:00 AM
                new TimeSpan(17, 0, 0) // 5:00 PM
            ).Value);

        _userRepositoryMock
            .Setup(repo => repo.GetByIdWithCustomerDetailsAsync(
                userId,
                It.IsAny<CancellationToken>()))
            .ReturnsAsync(user);

        _restaurantRepositoryMock
            .Setup(repo => repo.GetByIdAsync(
                restaurantId,
                It.IsAny<CancellationToken>()))
            .ReturnsAsync(restaurant);

        // Act
        var result = await _handler.Handle(command, CancellationToken.None);

        // Assert
        result.IsFailure.Should().BeTrue();

        _orderRepositoryMock.Verify(repo => repo.Add(
            It.IsAny<Order>()),
            Times.Never);
        _unitOfWorkMock.Verify(uow => uow.SaveChangesAsync(
            It.IsAny<CancellationToken>()),
            Times.Never);
    }

    #endregion
}