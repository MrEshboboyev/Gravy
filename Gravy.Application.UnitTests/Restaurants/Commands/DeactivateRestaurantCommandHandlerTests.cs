﻿using FluentAssertions;
using Gravy.Application.Restaurants.Commands.DeactivateRestaurant;
using Gravy.Domain.Entities;
using Gravy.Domain.Errors;
using Gravy.Domain.Repositories;
using Gravy.Domain.ValueObjects;
using Moq;

namespace Gravy.Application.UnitTests.Restaurants.Commands;

public class DeactivateRestaurantCommandHandlerTests
{
    #region Fields & Mock Setup

    private readonly DeactivateRestaurantCommandHandler _handler;

    // Mock dependencies
    private readonly Mock<IRestaurantRepository> _restaurantRepositoryMock;
    private readonly Mock<IUnitOfWork> _unitOfWorkMock;

    public DeactivateRestaurantCommandHandlerTests()
    {
        // Initialize mocks
        _restaurantRepositoryMock = new Mock<IRestaurantRepository>();
        _unitOfWorkMock = new Mock<IUnitOfWork>();

        // Initialize the command handler with mocks
        _handler = new DeactivateRestaurantCommandHandler(
            _restaurantRepositoryMock.Object,
            _unitOfWorkMock.Object);
    }

    #endregion

    #region Test Methods

    /// <summary>
    /// Test Case: Successfully deactivates a restaurant when all conditions are met.
    /// </summary>
    [Fact]
    public async Task Handle_Should_DeactivateRestaurant_WhenRestaurantExists()
    {
        // Arrange
        var restaurantId = Guid.NewGuid();
        var command = new DeactivateRestaurantCommand(restaurantId);

        var restaurant = CreateTestRestaurant(restaurantId, isActive: true);

        // Mock: Repository returns the restaurant
        _restaurantRepositoryMock
            .Setup(repo => repo.GetByIdAsync(
                restaurantId,
                It.IsAny<CancellationToken>()))
            .ReturnsAsync(restaurant);

        // Act
        var result = await _handler.Handle(command, CancellationToken.None);

        // Assert
        result.IsSuccess.Should().BeTrue();

        restaurant.IsActive.Should().BeFalse(); // Verify restaurant was deactivated

        // Verify repository update was called
        _restaurantRepositoryMock.Verify(repo => repo.Update(restaurant),
            Times.Once);

        // Verify SaveChangesAsync was called once
        _unitOfWorkMock.Verify(uow => uow.SaveChangesAsync(
            It.IsAny<CancellationToken>()),
            Times.Once);
    }

    /// <summary>
    /// Test Case: Returns failure when the restaurant does not exist.
    /// </summary>
    [Fact]
    public async Task Handle_Should_ReturnFailure_WhenRestaurantDoesNotExist()
    {
        // Arrange
        var restaurantId = Guid.NewGuid();
        var command = new DeactivateRestaurantCommand(restaurantId);

        // Mock: Repository returns null (restaurant not found)
        _restaurantRepositoryMock
            .Setup(repo => repo.GetByIdAsync(
                restaurantId,
                It.IsAny<CancellationToken>()))
            .ReturnsAsync((Restaurant)null!);

        // Act
        var result = await _handler.Handle(command, CancellationToken.None);

        // Assert
        result.IsSuccess.Should().BeFalse();
        result.Error.Should().Be(DomainErrors.Restaurant.NotFound(restaurantId));

        // Verify that repository Update and unit of work SaveChangesAsync are NOT called
        _restaurantRepositoryMock.Verify(repo => repo.Update(
            It.IsAny<Restaurant>()),
            Times.Never);
        _unitOfWorkMock.Verify(uow => uow.SaveChangesAsync(
            It.IsAny<CancellationToken>()),
            Times.Never);
    }

    /// <summary>
    /// Test Case: Ensures SaveChangesAsync is called after deactivation.
    /// </summary>
    [Fact]
    public async Task Handle_Should_CallSaveChanges_WhenRestaurantIsDeactivated()
    {
        // Arrange
        var restaurantId = Guid.NewGuid();
        var command = new DeactivateRestaurantCommand(restaurantId);

        var restaurant = CreateTestRestaurant(restaurantId, isActive: true);

        // Mock: Repository returns the restaurant
        _restaurantRepositoryMock
            .Setup(repo => repo.GetByIdAsync(
                restaurantId, It.IsAny<CancellationToken>()))
            .ReturnsAsync(restaurant);

        // Act
        var result = await _handler.Handle(command, CancellationToken.None);

        // Assert
        result.IsSuccess.Should().BeTrue();
        restaurant.IsActive.Should().BeFalse();

        // Verify SaveChangesAsync was called once
        _unitOfWorkMock.Verify(uow => uow.SaveChangesAsync(
            It.IsAny<CancellationToken>()),
            Times.Once);
    }

    #endregion

    #region Helpers

    /// <summary>
    /// Helper method to create a test restaurant with specified parameters.
    /// </summary>
    private static Restaurant CreateTestRestaurant(Guid restaurantId, bool isActive)
    {
        var restaurant = Restaurant.Create(
            restaurantId,
            "Test Restaurant",
            "Test restaurant description",
            Email.Create("email@test.com").Value,
            "+test",
            Address.Create("address").Value,
            Guid.NewGuid(),
            OpeningHours.Create(TimeSpan.FromDays(7), TimeSpan.FromDays(19)).Value);
        if (isActive)
        {
            restaurant.Deactivate();
        }
        return restaurant;
    }

    #endregion
}