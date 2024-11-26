using Gravy.Domain.Events;
using Gravy.Domain.Primitives;
using Gravy.Domain.Repositories;
using Gravy.Domain.ValueObjects;

namespace Gravy.Domain.Entities;

/// <summary> 
/// Represents a user in the system. 
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
        LastName = lastName;
    }

    private User()
    {
    }

    public string PasswordHash { get; set; }
    public FirstName FirstName { get; set; }
    public LastName LastName { get; set; }
    public Email Email { get; set; }
    public DateTime CreatedOnUtc { get; set; }
    public DateTime? ModifiedOnUtc { get; set; }
    public ICollection<Role> Roles { get; set; } = [];

    /// <summary> 
    /// Creates a new user instance. 
    /// </summary>
    public static async Task<User> CreateAsync(
        Guid id,
        Email email,
        string passwordHash,
        FirstName firstName,
        LastName lastName,
        IRoleRepository roleRepository,
        CancellationToken cancellationToken
        )
    {
        var user = new User(
            id,
            email,
            passwordHash,
            firstName,
            lastName);
        
        user.RaiseDomainEvent(new UserRegisteredDomainEvent(
            Guid.NewGuid(),
            user.Id));

        var registeredRole = await roleRepository.GetByNameAsync("Registered", cancellationToken);
        user.AssignRole(registeredRole);    

        return user;
    }

    /// <summary> 
    /// Changes the user's name and raises a domain event if the name has changed. 
    /// </summary>
    public void ChangeName(FirstName firstName, LastName lastName)
    {
        if (!FirstName.Equals(firstName) || !LastName.Equals(lastName))
        {
            RaiseDomainEvent(new UserNameChangedDomainEvent(
                Guid.NewGuid(), Id));
        }

        FirstName = firstName;
        LastName = lastName;
    }

    public void AssignRole(Role role)
    {
        if (!Roles.Contains(role))
            Roles.Add(role);
    }
}

