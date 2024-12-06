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
    private Customer _customerDetails;
    private DeliveryPerson _deliveryPersonDetails;

    private User(Guid id, Email email, string passwordHash, FirstName firstName, LastName lastName)
     : base(id)
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

    // Properties
    public Email Email { get; private set; }
    public string PasswordHash { get; private set; }
    public FirstName FirstName { get; private set; }
    public LastName LastName { get; private set; }
    public Customer CustomerDetails => _customerDetails;
    public DeliveryPerson DeliveryPersonDetails => _deliveryPersonDetails;
    public ICollection<Role> Roles { get; private set; } = [];
    public DateTime CreatedOnUtc { get; set; }
    public DateTime? ModifiedOnUtc { get; set; }

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

    /// <summary>
    /// Links customer-specific details to the user.
    /// </summary>
    public Result<Customer> AddCustomerDetails(DeliveryAddress deliveryAddress)
    {
        if (_customerDetails is not null)
        {
            return Result.Failure<Customer>(
                DomainErrors.Customer.AlreadyExist(_customerDetails.Id, Id));
        }

        _customerDetails = new Customer(Guid.NewGuid(), Id, deliveryAddress);
        ModifiedOnUtc = DateTime.UtcNow;

        RaiseDomainEvent(new CustomerLinkedToUserDomainEvent(
            Guid.NewGuid(), 
            Id, 
            _customerDetails.Id));

        return _customerDetails;
    }

    #region Delivery Person
    /// <summary>
    /// Links delivery-person-specific details to the user.
    /// </summary>
    public Result<DeliveryPerson> AddDeliveryPersonDetails(Vehicle vehicle)
    {
        if (_deliveryPersonDetails is not null)
        {
            return Result.Failure<DeliveryPerson>(
                DomainErrors.DeliveryPerson.AlreadyExist(_deliveryPersonDetails.Id, Id));
        }

        _deliveryPersonDetails = new DeliveryPerson(Guid.NewGuid(), Id, vehicle);
        ModifiedOnUtc = DateTime.UtcNow;

        RaiseDomainEvent(new DeliveryPersonLinkedToUserDomainEvent(
            Guid.NewGuid(), 
            Id, 
            _deliveryPersonDetails.Id));

        return _deliveryPersonDetails;
    }

    public Result<DeliveryPersonAvailability> AddDeliveryPersonAvailability(
        DateTime startTimeUtc, DateTime endTimeUtc)
    {
        if (DeliveryPersonDetails == null)
        {
            return Result.Failure<DeliveryPersonAvailability>(
                DomainErrors.User.DeliveryPersonDetailsNotExist(Id));
        }

        var deliveryPersonAvailability = DeliveryPersonDetails
            .AddAvailability(startTimeUtc, endTimeUtc);

        ModifiedOnUtc = DateTime.UtcNow;

        // add event
        RaiseDomainEvent(new AvailabilityAddedToDeliveryPersonDomainEvent(
            Guid.NewGuid(),
            DeliveryPersonDetails.Id,
            deliveryPersonAvailability.Id));

        return deliveryPersonAvailability;
    }
    #endregion
}

