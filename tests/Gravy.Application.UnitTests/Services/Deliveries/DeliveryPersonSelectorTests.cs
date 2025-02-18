using FluentAssertions;
using Gravy.Application.Services.Deliveries;
using Gravy.Domain.Entities;
using Gravy.Domain.Repositories;
using Gravy.Domain.ValueObjects;
using Moq;

namespace Gravy.Application.UnitTests.Services.Deliveries;

public class DeliveryPersonSelectorTests
{
    #region Fields & Mock Setup

    private readonly Mock<IDeliveryPersonRepository> _deliveryPersonRepositoryMock;
    private readonly DeliveryPersonSelector _selector;

    public DeliveryPersonSelectorTests()
    {
        _deliveryPersonRepositoryMock = new Mock<IDeliveryPersonRepository>();
        _selector = new DeliveryPersonSelector(
            _deliveryPersonRepositoryMock.Object);
    }

    #endregion

    #region Test Methods

    /// <summary>
    /// Test case: No delivery persons are available at all.
    /// </summary>
    [Fact]
    public async Task SelectBestDeliveryPersonAsync_Should_ReturnNull_WhenNoDeliveryPersonsExist()
    {
        // Arrange
        var order = CreateOrderWithDeliveryAddress();
        _deliveryPersonRepositoryMock
            .Setup(repo => repo.GetAllAsync(
                It.IsAny<CancellationToken>()))
            .ReturnsAsync([]);

        // Act
        var result = await _selector.SelectBestDeliveryPersonAsync(
            order, CancellationToken.None);

        // Assert
        result.Should().BeNull();

        _deliveryPersonRepositoryMock.Verify(repo =>
            repo.GetAllAsync(
                It.IsAny<CancellationToken>()), 
            Times.Once);

        _deliveryPersonRepositoryMock.Verify(repo =>
            repo.Update(
                It.IsAny<DeliveryPerson>()), 
            Times.Never);
    }

    /// <summary>
    /// Test case: No delivery persons match location/distance requirements.
    /// </summary>
    [Fact]
    public async Task SelectBestDeliveryPersonAsync_Should_ReturnNull_WhenNoDeliveryPersonsMatchLocation()
    {
        // Arrange
        var order = CreateOrderWithDeliveryAddress();
        var deliveryPersons = new List<DeliveryPerson>
        {
            CreateDeliveryPerson(Guid.NewGuid()),
            CreateDeliveryPerson(Guid.NewGuid())
        };

        _deliveryPersonRepositoryMock
            .Setup(repo => repo.GetAllAsync(
                It.IsAny<CancellationToken>()))
            .ReturnsAsync(deliveryPersons);

        // Act
        var result = await _selector.SelectBestDeliveryPersonAsync(
            order, CancellationToken.None);

        // Assert
        result.Should().BeNull();

        _deliveryPersonRepositoryMock.Verify(repo =>
            repo.GetAllAsync(
                It.IsAny<CancellationToken>()), 
            Times.Once);

        _deliveryPersonRepositoryMock.Verify(repo =>
            repo.Update(
                It.IsAny<DeliveryPerson>()),
            Times.Never);
    }

    /// <summary>
    /// Test case: No delivery persons are available at the current time.
    /// </summary>
    [Fact]
    public async Task SelectBestDeliveryPersonAsync_Should_ReturnNull_WhenNoPersonsAreAvailableAtCurrentTime()
    {
        // Arrange
        var order = CreateOrderWithDeliveryAddress();
        var deliveryPersons = new List<DeliveryPerson>
        {
            CreateDeliveryPerson(Guid.NewGuid()),
            CreateDeliveryPerson(Guid.NewGuid())
        };

        _deliveryPersonRepositoryMock
            .Setup(repo => repo.GetAllAsync(
                It.IsAny<CancellationToken>()))
            .ReturnsAsync(deliveryPersons);

        // Act
        var result = await _selector.SelectBestDeliveryPersonAsync(
            order, CancellationToken.None);

        // Assert
        result.Should().BeNull();

        _deliveryPersonRepositoryMock.Verify(repo =>
            repo.GetAllAsync(
                It.IsAny<CancellationToken>()),
            Times.Once);

        _deliveryPersonRepositoryMock.Verify(repo =>
            repo.Update(
                It.IsAny<DeliveryPerson>()), 
            Times.Never);
    }

    /// <summary>
    /// Test case: The closest delivery person is selected successfully.
    /// </summary>
    [Fact]
    public async Task SelectBestDeliveryPersonAsync_Should_ReturnClosestDeliveryPerson_WhenAvailable()
    {
        // Arrange
        var order = CreateOrderWithDeliveryAddress();
        var deliveryPersons = new List<DeliveryPerson>
        {
            CreateDeliveryPerson(Guid.NewGuid()),
            CreateDeliveryPerson(Guid.NewGuid(), 10, 10), // Closest
            CreateDeliveryPerson(Guid.NewGuid())
        };

        _deliveryPersonRepositoryMock
            .Setup(repo => repo.GetAllAsync(
                It.IsAny<CancellationToken>()))
            .ReturnsAsync(deliveryPersons);

        // Act
        var result = await _selector.SelectBestDeliveryPersonAsync(
            order, CancellationToken.None);

        // Assert
        result.Should().NotBeNull();
        result.Id.Should().Be(deliveryPersons[1].Id);

        result.IsAvailable.Should().BeFalse(); // Availability should be set to false

        _deliveryPersonRepositoryMock.Verify(repo =>
            repo.GetAllAsync(
                It.IsAny<CancellationToken>()), 
            Times.Once);

        _deliveryPersonRepositoryMock.Verify(repo =>
            repo.Update(It.Is<DeliveryPerson>(
                dp => dp.Id == deliveryPersons[1].Id)), 
            Times.Once);
    }

    #endregion

    #region Helpers

    /// <summary>
    /// Helper method to create a test Order with DeliveryAddress.
    /// </summary>
    private static Order CreateOrderWithDeliveryAddress()
    {
        return Order.Create(
            Guid.NewGuid(),
            Guid.NewGuid(),
            Guid.NewGuid(),
            DeliveryAddress.Create(
                "street",
                "city",
                "state",
                10.0,
                10.0).Value);
    }

    private static DeliveryPerson CreateDeliveryPerson(
        Guid deliveryPersonId,
        double latitude = 0.0,
        double longitude = 0.0)
    {
        var user = User.Create(
            Guid.NewGuid(),
            Email.Create("email@test.com").Value,
            "hashedPassword",
            FirstName.Create("firstName").Value,
            LastName.Create("lastName").Value);

        return user.AddDeliveryPersonDetails(
            Vehicle.Create("Car", "XXX123").Value,
            Location.Create(34.0, 73.0).Value)
            .Value;
    }

    #endregion
}