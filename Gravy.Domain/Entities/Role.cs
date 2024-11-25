using Gravy.Domain.Primitives;

namespace Gravy.Domain.Entities;

/// <summary> 
/// Represents a user role in the system. 
/// </summary>
public sealed class Role : Enumeration<Role>
{
    public static readonly Role Registered = new(1, "Registered");

    public Role(int id, string name)
        : base(id, name)
    {
    }

    public ICollection<Permission> Permissions { get; set; }
    public ICollection<User> Users { get; set; }
}