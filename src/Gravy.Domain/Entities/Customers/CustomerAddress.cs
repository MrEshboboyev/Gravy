using Gravy.Domain.Enums.Customers;
using Gravy.Domain.Primitives;

namespace Gravy.Domain.Entities.Customers;

/// <summary>
/// Represents a customer address.
/// </summary>
public sealed class CustomerAddress : Entity
{
    #region Constructors

    private CustomerAddress(
        Guid id,
        Guid customerId,
        string street,
        string city,
        string state,
        string postalCode,
        string country,
        CustomerAddressType addressType,
        bool isDefault) : base(id)
    {
        CustomerId = customerId;
        Street = street;
        City = city;
        State = state;
        PostalCode = postalCode;
        Country = country;
        AddressType = addressType;
        IsDefault = isDefault;
        CreatedOnUtc = DateTime.UtcNow;
    }

    // ORM
    private CustomerAddress()
    {
    }

    #endregion

    #region Properties
    
    public Guid CustomerId { get; private set; }
    public string Street { get; private set; }
    public string City { get; private set; }
    public string State { get; private set; }
    public string PostalCode { get; private set; }
    public string Country { get; private set; }
    public CustomerAddressType AddressType { get; private set; }
    public bool IsDefault { get; private set; }
    public DateTime CreatedOnUtc { get; private set; }
    public DateTime? ModifiedOnUtc { get; private set; }
    
    #endregion

    #region Factory Methods
    
    public static CustomerAddress Create(
        Guid id,
        Guid customerId,
        string street,
        string city,
        string state,
        string postalCode,
        string country,
        CustomerAddressType addressType,
        bool isDefault)
    {
        return new CustomerAddress(
            id,
            customerId,
            street,
            city,
            state,
            postalCode,
            country,
            addressType,
            isDefault);
    }

    #endregion

    #region Own Methods
    
    public void Update(
        string street,
        string city,
        string state,
        string postalCode,
        string country,
        CustomerAddressType addressType,
        bool isDefault)
    {
        Street = street;
        City = city;
        State = state;
        PostalCode = postalCode;
        Country = country;
        AddressType = addressType;
        IsDefault = isDefault;
        ModifiedOnUtc = DateTime.UtcNow;
    }

    public void UnsetDefault()
    {
        IsDefault = false;
        ModifiedOnUtc = DateTime.UtcNow;
    }

    #endregion
}