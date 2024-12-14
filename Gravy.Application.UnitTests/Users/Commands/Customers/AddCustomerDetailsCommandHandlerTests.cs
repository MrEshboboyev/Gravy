using FluentAssertions;
using Gravy.Application.Users.Commands.Customers.AddCustomerDetails; 
using Gravy.Domain.Errors; 
using Gravy.Domain.Repositories; 
using Gravy.Domain.Entities;
using Gravy.Domain.ValueObjects;
using Moq; 

namespace Gravy.Application.UnitTests.Users.Commands.Customers;

/// <summary>
/// Unit tests for AddCustomerDetailsCommandHandler.
/// Ensures the handler processes commands correctly under different scenarios.
/// </summary>
public class AddCustomerDetailsCommandHandlerTests
{
    #region Fields & Mock Setup

    private readonly Mock<IUserRepository> _userRepositoryMock = new();
    private readonly Mock<ICustomerRepository> _customerRepositoryMock = new();
    private readonly Mock<IUnitOfWork> _unitOfWorkMock = new();

    private readonly User _user = User.Create(
        Guid.NewGuid(),
        Email.Create("email@test.com").Value,
        "hashedPassword",
        FirstName.Create("firstName").Value,
        LastName.Create("lastName").Value);

    #endregion

    #region Test Methods

    /// <summary>
    /// Test case: Handle should return failure if the _user is not found.
    /// </summary>
    [Fact]
    public async Task Handle_Should_ReturnFailure_WhenUserNotFound()
    {
        // Arrange
        var command = new AddCustomerDetailsCommand(
            Guid.NewGuid(), 
            "123 Main St", 
            "Springfield",
            "IL",
            39.7817, 
            -89.6501);

        _userRepositoryMock
            .Setup(x => x.GetByIdWithCustomerDetailsAsync(
                command.UserId, 
                It.IsAny<CancellationToken>()))
            .ReturnsAsync((User)null!); // Simulate _user not found

        var handler = new AddCustomerDetailsCommandHandler(
            _userRepositoryMock.Object,
            _customerRepositoryMock.Object,
            _unitOfWorkMock.Object);

        // Act
        var result = await handler.Handle(command, CancellationToken.None);

        // Assert
        result.IsFailure.Should().BeTrue();
        result.Error.Should().Be(DomainErrors.User.NotFound(command.UserId));
    }

    /// <summary>
    /// Test case: Handle should return failure when delivery address creation fails.
    /// </summary>
    [Fact]
    public async Task Handle_Should_ReturnFailure_WhenDeliveryAddressIsInvalid()
    {
        // Arrange
        _userRepositoryMock
            .Setup(x => x.GetByIdWithCustomerDetailsAsync(
                It.IsAny<Guid>(), 
                It.IsAny<CancellationToken>()))
            .ReturnsAsync(_user);

        var invalidCommand = new AddCustomerDetailsCommand(
            _user.Id, 
            "", 
            "", 
            "", 
            0, 
            0); // Invalid address data

        var handler = new AddCustomerDetailsCommandHandler(
            _userRepositoryMock.Object,
            _customerRepositoryMock.Object,
            _unitOfWorkMock.Object);

        // Act
        var result = await handler.Handle(invalidCommand, CancellationToken.None);

        // Assert
        result.IsFailure.Should().BeTrue();
        result.Error.Should().NotBeNull(); // Ensure it fails due to invalid address
    }

    /// <summary>
    /// Test case: Handle should return success if customer details are added successfully.
    /// </summary>
    [Fact]
    public async Task Handle_Should_ReturnSuccess_WhenCustomerDetailsAreAddedSuccessfully()
    {
        // Arrange
        var command = new AddCustomerDetailsCommand(
            _user.Id, 
            "123 Main St",
            "Springfield", 
            "IL", 
            39.7817, 
            -89.6501);

        _userRepositoryMock
            .Setup(x => x.GetByIdWithCustomerDetailsAsync(
                command.UserId, 
                It.IsAny<CancellationToken>()))
            .ReturnsAsync(_user);

        _customerRepositoryMock
            .Setup(x => x.Add(
                It.IsAny<Customer>()));

        var handler = new AddCustomerDetailsCommandHandler(
            _userRepositoryMock.Object,
            _customerRepositoryMock.Object,
            _unitOfWorkMock.Object);

        // Act
        var result = await handler.Handle(command, CancellationToken.None);

        // Assert
        result.IsSuccess.Should().BeTrue();
        _customerRepositoryMock.Verify(x => x.Add(It.IsAny<Customer>()), 
            Times.Once);
        _unitOfWorkMock.Verify(x => x.SaveChangesAsync(It.IsAny<CancellationToken>()), 
            Times.Once);
    }

    /// <summary>
    /// Test case: Handle should not call Add or SaveChangesAsync when address creation fails.
    /// </summary>
    [Fact]
    public async Task Handle_Should_NotCallAddOrSave_WhenDeliveryAddressCreationFails()
    {
        // Arrange
        var invalidCommand = new AddCustomerDetailsCommand(
            _user.Id, 
            "", 
            "",
            "", 
            0, 
            0); // Invalid address

        _userRepositoryMock
            .Setup(x => x.GetByIdWithCustomerDetailsAsync(
                It.IsAny<Guid>(), 
                It.IsAny<CancellationToken>()))
            .ReturnsAsync(_user);

        var handler = new AddCustomerDetailsCommandHandler(
            _userRepositoryMock.Object,
            _customerRepositoryMock.Object,
            _unitOfWorkMock.Object);

        // Act
        var result = await handler.Handle(invalidCommand, CancellationToken.None);

        // Assert
        _customerRepositoryMock.Verify(x => x.Add(It.IsAny<Customer>()), 
            Times.Never);
        _unitOfWorkMock.Verify(x => x.SaveChangesAsync(It.IsAny<CancellationToken>()),
            Times.Never);
        result.IsFailure.Should().BeTrue();
    }

    #endregion
}
