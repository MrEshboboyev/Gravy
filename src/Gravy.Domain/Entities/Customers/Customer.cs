using Gravy.Domain.Enums;
using Gravy.Domain.Errors;
using Gravy.Domain.Events;
using Gravy.Domain.Primitives;
using Gravy.Domain.Shared;
using Gravy.Domain.ValueObjects;
using System.Dynamic;
using Gravy.Domain.Enums.Customers;

namespace Gravy.Domain.Entities.Customers;

/// <summary>
/// Represents a customer in the system.
/// Acts as the aggregate root for customer-related data.
/// </summary>
public sealed class Customer : AggregateRoot, IAuditableEntity
{
    #region Private fields
    private readonly List<Address> _addresses = [];
    private readonly List<PaymentMethod> _paymentMethods = [];
    #endregion

    #region Constructor
    private Customer(
        Guid id,
        CustomerName name,
        Email email,
        CustomerPhoneNumber phoneNumber,
        DateTime registrationDate,
        CustomerStatus status) : base(id)
    {
        Name = name;
        Email = email;
        PhoneNumber = phoneNumber;
        RegistrationDate = registrationDate;
        Status = status;

        RaiseDomainEvent(new CustomerCreatedDomainEvent(
            Guid.NewGuid(),
            Id,
            Name.FullName));
    }

    private Customer()
    {
    }
    #endregion

    #region Properties
    public CustomerName Name { get; private set; }
    public Email Email { get; private set; }
    public CustomerPhoneNumber PhoneNumber { get; private set; }
    public DateTime RegistrationDate { get; private set; }
    public CustomerStatus Status { get; private set; }
    public IReadOnlyCollection<Address> Addresses => _addresses.AsReadOnly();
    public IReadOnlyCollection<PaymentMethod> PaymentMethods => _paymentMethods.AsReadOnly();
    public DateTime CreatedOnUtc { get; set; }
    public DateTime? ModifiedOnUtc { get; set; }

    #endregion

    #region Factory methods
    /// <summary>
    /// Factory method to create a new customer.
    /// </summary>
    public static Customer Create(
        Guid id,
        CustomerName name,
        Email email,
        CustomerPhoneNumber phoneNumber)
    {
        return new Customer(
            id,
            name,
            email,
            phoneNumber,
            DateTime.UtcNow,
            CustomerStatus.Active);
    }
    #endregion

    #region Own Methods
    /// <summary>
    /// Updates the customer's details.
    /// </summary>
    public void UpdateDetails(
        CustomerName name,
        Email email,
        CustomerPhoneNumber phoneNumber)
    {
        Name = name;
        Email = email;
        PhoneNumber = phoneNumber;

        RaiseDomainEvent(new CustomerUpdatedDomainEvent(
            Guid.NewGuid(),
            Id,
            Name.FullName));
    }

    /// <summary>
    /// Activates a customer account.
    /// </summary>
    public void Activate()
    {
        Status = CustomerStatus.Active;

        RaiseDomainEvent(new CustomerActivatedDomainEvent(
            Guid.NewGuid(),
            Id,
            Name.FullName));
    }

    /// <summary>
    /// Deactivates a customer account.
    /// </summary>
    public void Deactivate()
    {
        Status = CustomerStatus.Inactive;

        RaiseDomainEvent(new CustomerDeactivatedDomainEvent(
            Guid.NewGuid(),
            Id,
            Name.FullName));
    }
    #endregion

    #region Address Related
    /// <summary>
    /// Adds a new address for the customer.
    /// </summary>
    public Result<Address> AddAddress(
        string street,
        string city,
        string state,
        string postalCode,
        string country,
        CustomerAddressType addressType,
        bool isDefault)
    {
        var address = Address.Create(
            Guid.NewGuid(),
            Id,
            street,
            city,
            state,
            postalCode,
            country,
            addressType,
            isDefault);

        if (isDefault)
        {
            foreach (var existingAddress in _addresses.Where(a => a.IsDefault))
            {
                existingAddress.UnsetDefault();
            }
        }

        _addresses.Add(address);

        RaiseDomainEvent(new AddressAddedDomainEvent(
            Guid.NewGuid(),
            Id,
            address.Id,
            addressType,
            isDefault));

        return Result.Success(address);
    }

    /// <summary>
    /// Updates an existing address.
    /// </summary>
    public Result<Address> UpdateAddress(
        Guid addressId,
        string street,
        string city,
        string state,
        string postalCode,
        string country,
        CustomerAddressType addressType,
        bool isDefault)
    {
        var address = _addresses.SingleOrDefault(a => a.Id == addressId);
        if (address is null)
        {
            return Result.Failure<Address>(
                DomainErrors.Customer.AddressNotFound(Id, addressId));
        }

        if (isDefault)
        {
            foreach (var existingAddress in _addresses.Where(a => a.IsDefault && a.Id != addressId))
            {
                existingAddress.UnsetDefault();
            }
        }

        address.Update(street, city, state, postalCode, country, addressType, isDefault);

        RaiseDomainEvent(new AddressUpdatedDomainEvent(
            Guid.NewGuid(),
            Id,
            address.Id,
            addressType,
            isDefault));

        return Result.Success(address);
    }

    /// <summary>
    /// Removes an address from the customer.
    /// </summary>
    public Result RemoveAddress(Guid addressId)
    {
        var address = _addresses.SingleOrDefault(a => a.Id == addressId);
        if (address is null)
        {
            return Result.Failure(
                DomainErrors.Customer.AddressNotFound(Id, addressId));
        }

        if (address.IsDefault && _addresses.Count > 1)
        {
            return Result.Failure(
                DomainErrors.Customer.CannotRemoveDefaultAddress);
        }

        _addresses.Remove(address);

        RaiseDomainEvent(new AddressRemovedDomainEvent(
            Guid.NewGuid(),
            Id,
            addressId));

        return Result.Success();
    }
    #endregion

    #region Payment Method Related
    /// <summary>
    /// Adds a new payment method for the customer.
    /// </summary>
    public Result<PaymentMethod> AddPaymentMethod(
        PaymentType type,
        CardInfo cardInfo,
        bool isDefault)
    {
        var paymentMethod = PaymentMethod.Create(
            Guid.NewGuid(),
            Id,
            type,
            cardInfo,
            isDefault,
            PaymentMethodStatus.Active);

        if (isDefault)
        {
            foreach (var existingPaymentMethod in _paymentMethods.Where(p => p.IsDefault))
            {
                existingPaymentMethod.UnsetDefault();
            }
        }

        _paymentMethods.Add(paymentMethod);

        RaiseDomainEvent(new PaymentMethodAddedDomainEvent(
            Guid.NewGuid(),
            Id,
            paymentMethod.Id,
            type,
            isDefault));

        return Result.Success(paymentMethod);
    }

    /// <summary>
    /// Updates an existing payment method.
    /// </summary>
    public Result<PaymentMethod> UpdatePaymentMethod(
        Guid paymentMethodId,
        CardInfo cardInfo,
        bool isDefault)
    {
        var paymentMethod = _paymentMethods.SingleOrDefault(p => p.Id == paymentMethodId);
        if (paymentMethod is null)
        {
            return Result.Failure<PaymentMethod>(
                DomainErrors.Customer.PaymentMethodNotFound(Id, paymentMethodId));
        }

        if (isDefault)
        {
            foreach (var existingPaymentMethod in _paymentMethods.Where(p => p.IsDefault && p.Id != paymentMethodId))
            {
                existingPaymentMethod.UnsetDefault();
            }
        }

        paymentMethod.Update(cardInfo, isDefault);

        RaiseDomainEvent(new PaymentMethodUpdatedDomainEvent(
            Guid.NewGuid(),
            Id,
            paymentMethod.Id,
            isDefault));

        return Result.Success(paymentMethod);
    }

    /// <summary>
    /// Removes a payment method from the customer.
    /// </summary>
    public Result RemovePaymentMethod(Guid paymentMethodId)
    {
        var paymentMethod = _paymentMethods.SingleOrDefault(p => p.Id == paymentMethodId);
        if (paymentMethod is null)
        {
            return Result.Failure(
                DomainErrors.Customer.PaymentMethodNotFound(Id, paymentMethodId));
        }

        if (paymentMethod.IsDefault && _paymentMethods.Count > 1)
        {
            return Result.Failure(
                DomainErrors.Customer.CannotRemoveDefaultPaymentMethod);
        }

        _paymentMethods.Remove(paymentMethod);

        RaiseDomainEvent(new PaymentMethodRemovedDomainEvent(
            Guid.NewGuid(),
            Id,
            paymentMethodId));

        return Result.Success();
    }

    #endregion
}