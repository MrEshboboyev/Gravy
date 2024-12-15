using Gravy.Domain.Errors;
using Gravy.Domain.Events;
using Gravy.Domain.Primitives;
using Gravy.Domain.Shared;
using Gravy.Domain.ValueObjects;

namespace Gravy.Domain.Entities;

/// <summary>
/// Represents a system user and acts as the aggregate root for user-related entities.
/// </summary>
public sealed class User : AggregateRoot, IAuditableEntity
{
    #region Private fields
    private Customer _customerDetails;
    private DeliveryPerson _deliveryPersonDetails;
    #endregion

    #region Constructors
    private User(
        Guid id,
        Email email, 
        string passwordHash, 
        FirstName firstName, 
        LastName lastName) : base(id)
    {
        Email = email;
        PasswordHash = passwordHash;
        FirstName = firstName;
        LastName = lastName;

        RaiseDomainEvent(new UserCreatedDomainEvent(
            Guid.NewGuid(),
            Id, 
            email.Value));
    }

    private User()
    {
    }
    #endregion

    #region Properties
    public Email Email { get; private set; }
    public string PasswordHash { get; private set; }
    public FirstName FirstName { get; private set; }
    public LastName LastName { get; private set; }
    public Customer CustomerDetails => _customerDetails;
    public DeliveryPerson DeliveryPersonDetails => _deliveryPersonDetails;
    public ICollection<Role> Roles { get; private set; } = [];
    public DateTime CreatedOnUtc { get; set; }
    public DateTime? ModifiedOnUtc { get; set; }
    #endregion

    #region Own methods
    /// <summary>
    /// Factory method to create a new user.
    /// </summary>
    public static User Create(Guid id, 
        Email email, 
        string passwordHash, 
        FirstName firstName, 
        LastName lastName)
    {
        return new User(id, 
            email, 
            passwordHash, 
            firstName, 
            lastName);
    }

    /// <summary>
    /// Updates the user's name.
    /// </summary>
    public void UpdateName(FirstName firstName, LastName lastName)
    {
        if (!FirstName.Equals(firstName) || !LastName.Equals(lastName))
        {
            FirstName = firstName;
            LastName = lastName;
            ModifiedOnUtc = DateTime.UtcNow;

            RaiseDomainEvent(new UserNameUpdatedDomainEvent(
                Guid.NewGuid(), 
                Id, 
                firstName.Value, 
                lastName.Value));
        }
    }
    #endregion

    #region Role Related methods
    /// <summary>
    /// Assigns a role to the user.
    /// </summary>
    public void AssignRole(Role role)
    {
        if (!Roles.Contains(role))
        {
            Roles.Add(role);
            ModifiedOnUtc = DateTime.UtcNow;

            RaiseDomainEvent(new RoleAssignedToUserDomainEvent(
                Guid.NewGuid(), 
                Id, 
                role.Id));
        }
    }
    #endregion

    #region Customer Related methods
    /// <summary>
    /// Links customer-specific details to the user.
    /// </summary>
    public Result<Customer> AddCustomerDetails(DeliveryAddress deliveryAddress)
    {
        #region Checking Customer Details already exists 

        if (_customerDetails is not null)
        {
            return Result.Failure<Customer>(
                DomainErrors.Customer.AlreadyExist(_customerDetails.Id, Id));
        }

        #endregion

        #region Create new Customer and assign _customerDetails for this user

        _customerDetails = new Customer(Guid.NewGuid(), Id, deliveryAddress);
        ModifiedOnUtc = DateTime.UtcNow;

        #endregion

        #region Domain Events

        RaiseDomainEvent(new CustomerLinkedToUserDomainEvent(
            Guid.NewGuid(), 
            Id, 
            _customerDetails.Id));

        #endregion

        return Result.Success(_customerDetails);
    }
    #endregion

    #region Delivery Person related methods
    /// <summary>
    /// Links delivery-person-specific details to the user.
    /// </summary>
    public Result<DeliveryPerson> AddDeliveryPersonDetails(
        Vehicle vehicle,
        Location location)
    {
        if (_deliveryPersonDetails is not null)
        {
            return Result.Failure<DeliveryPerson>(
                DomainErrors.DeliveryPerson.AlreadyExist(_deliveryPersonDetails.Id, Id));
        }

        _deliveryPersonDetails = new DeliveryPerson(
            Guid.NewGuid(), 
            Id, 
            vehicle,
            location);
        ModifiedOnUtc = DateTime.UtcNow;

        RaiseDomainEvent(new DeliveryPersonLinkedToUserDomainEvent(
            Guid.NewGuid(), 
            Id, 
            _deliveryPersonDetails.Id));

        return _deliveryPersonDetails;
    }

    #region Availabilities
    public Result<DeliveryPersonAvailability> AddDeliveryPersonAvailability(
        DateTime startTimeUtc,
        DateTime endTimeUtc)
    {
        #region Checking delivery person details exist

        if (DeliveryPersonDetails is null)
        {
            return Result.Failure<DeliveryPersonAvailability>(
                DomainErrors.User.DeliveryPersonDetailsNotExist(Id));
        }

        #endregion

        #region adding availability

        var addingAvailabilityResult = DeliveryPersonDetails
            .AddAvailability(startTimeUtc, endTimeUtc);

        if (addingAvailabilityResult.IsFailure)
        {
            return Result.Failure<DeliveryPersonAvailability>(
                addingAvailabilityResult.Error);
        }

        #endregion

        #region Update this user

        ModifiedOnUtc = DateTime.UtcNow;

        #endregion

        #region Domain Events

        RaiseDomainEvent(new AvailabilityAddedToDeliveryPersonDomainEvent(
            Guid.NewGuid(),
            DeliveryPersonDetails.Id,
            addingAvailabilityResult.Value.Id));

        #endregion

        return addingAvailabilityResult;
    }

    public Result<DeliveryPersonAvailability> UpdateDeliveryPersonAvailability(
        Guid availabilityId,
        DateTime startTimeUtc, 
        DateTime endTimeUtc)
    {
        #region Checking delivery person details exist

        if (DeliveryPersonDetails is null)
        {
            return Result.Failure<DeliveryPersonAvailability>(
                DomainErrors.User.DeliveryPersonDetailsNotExist(Id));
        }

        #endregion

        #region Update availability

        var updateAvailabilityResult = DeliveryPersonDetails.UpdateAvailability(
            availabilityId,
            startTimeUtc, 
            endTimeUtc);
        if (updateAvailabilityResult.IsFailure)
        {
            return Result.Failure<DeliveryPersonAvailability>(
                updateAvailabilityResult.Error);
        }

        #endregion

        #region Update this user

        ModifiedOnUtc = DateTime.UtcNow;

        #endregion

        #region Domain Events

        RaiseDomainEvent(new DeliveryPersonAvailabilityUpdatedDomainEvent(
            Guid.NewGuid(),
            DeliveryPersonDetails.Id,
            updateAvailabilityResult.Value.Id));

        #endregion

        return updateAvailabilityResult;
    }

    public Result DeleteDeliveryPersonAvailability(
        Guid availabilityId)
    {
        #region Checking delivery person details exist

        if (DeliveryPersonDetails is null)
        {
            return Result.Failure<DeliveryPersonAvailability>(
                DomainErrors.User.DeliveryPersonDetailsNotExist(Id));
        }

        #endregion

        #region Delete availability

        var deleteAvailabilityResult = DeliveryPersonDetails.DeleteAvailability(
            availabilityId);
        if (deleteAvailabilityResult.IsFailure)
        {
            return Result.Failure<DeliveryPersonAvailability>(
                deleteAvailabilityResult.Error);
        }

        #endregion

        #region Update this user

        ModifiedOnUtc = DateTime.UtcNow;

        #endregion

        #region Domain Events

        RaiseDomainEvent(new DeliveryPersonAvailabilityDeletedDomainEvent(
            Guid.NewGuid(),
            DeliveryPersonDetails.Id,
            availabilityId));

        #endregion

        return Result.Success();
    }
    
    #endregion

    #endregion
}

