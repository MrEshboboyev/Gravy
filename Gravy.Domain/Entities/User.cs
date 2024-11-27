using Gravy.Domain.Events;
using Gravy.Domain.Primitives;
using Gravy.Domain.ValueObjects;

namespace Gravy.Domain.Entities;

/// <summary>
/// Represents a system user and acts as the aggregate root for user-related entities.
/// </summary>
public sealed class User : AggregateRoot, IAuditableEntity
{
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
    public Customer? CustomerDetails { get; private set; }
    public DeliveryPerson? DeliveryPersonDetails { get; private set; }
    public ICollection<Role> Roles { get; private set; } = [];
    public DateTime CreatedOnUtc { get; set; }
    public DateTime? ModifiedOnUtc { get; set; }

    ///// <summary> 
    ///// Creates a new user instance. 
    ///// </summary>
    //public static async Task<User> CreateAsync(
    //    Guid id,
    //    Email email,
    //    string passwordHash,
    //    FirstName firstName,
    //    LastName lastName,
    //    IRoleRepository roleRepository,
    //    CancellationToken cancellationToken
    //    )
    //{
    //    var user = new User(
    //        id,
    //        email,
    //        passwordHash,
    //        firstName,
    //        lastName);

    //    user.RaiseDomainEvent(new UserRegisteredDomainEvent(
    //        Guid.NewGuid(),
    //        user.Id));

    //    var registeredRole = await roleRepository.GetByNameAsync("Registered", cancellationToken);
    //    user.AssignRole(registeredRole);    

    //    return user;
    //}

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
    public void AddCustomerDetails(Customer customer)
    {
        CustomerDetails = customer;
        ModifiedOnUtc = DateTime.UtcNow;

        RaiseDomainEvent(new CustomerLinkedToUserDomainEvent(
            Guid.NewGuid(), 
            Id, 
            customer.Id));
    }

    /// <summary>
    /// Links delivery-person-specific details to the user.
    /// </summary>
    public void AddDeliveryPersonDetails(DeliveryPerson deliveryPerson)
    {
        DeliveryPersonDetails = deliveryPerson;
        ModifiedOnUtc = DateTime.UtcNow;

        RaiseDomainEvent(new DeliveryPersonLinkedToUserDomainEvent(
            Guid.NewGuid(), 
            Id, 
            deliveryPerson.Id));
    }
}

