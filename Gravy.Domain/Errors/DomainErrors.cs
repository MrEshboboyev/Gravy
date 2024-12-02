using Gravy.Domain.Shared;

namespace Gravy.Domain.Errors;

/// <summary> 
/// Defines and organizes domain-specific errors. 
/// </summary>
public static class DomainErrors
{
    #region User Related
    public static class User
    {
        public static readonly Error EmailAlreadyInUse = new(
            "User.EmailAlreadyInUse",
            "The specified email is already in use");

        public static readonly Func<Guid, Error> NotFound = id => new(
                "User.NotFound",
                $"The user with the identifier {id} was not found.");

        public static readonly Error NotExist = new(
                "Users.NotExist",
                $"There is no users");

        public static readonly Error InvalidCredentials = new(
               "User.InvalidCredentials",
               "The provided credentials are invalid");
    }

    public static class Customer
    {
        public static readonly Func<Guid, Error> NotFound = id => new(
                "Customer.NotFound",
                $"The customer with the identifier {id} was not found.");

        public static readonly Func<Guid, Guid, Error> AlreadyExist = (customerId, userId) => new(
                "Customer.AlreadyExist",
                $"The customer details with the identifier {customerId} " +
                    $"already exist for this user with Id {userId}.");
    }

    public static class DeliveryPerson
    {
        public static readonly Func<Guid, Error> NotFound = id => new(
                "DeliveryPerson.NotFound",
                $"The delivery person with the identifier {id} was not found.");

        public static readonly Func<Guid, Guid, Error> AlreadyExist = (deliveryPersonId, userId) 
            => new( 
                "DeliveryPerson.AlreadyExist",
                $"The delivery person details with the identifier {deliveryPersonId} " +
                    $"already exist for this user with Id {userId}.");
    }
    #endregion

    public static class Restaurant
    {
        public static readonly Func<Guid, Error> NotFound = id => new(
                "Restaurant.NotFound",
                $"The restaurant with the identifier {id} was not found.");

        public static readonly Func<Guid, Guid, Error> MenuItemNotFound =
            (restaurantId, menuItemId) => new(
                "Restaurant.MenuItemNotFound", 
                $"The menu item with the identifier {menuItemId} in the restaurant " +
                $"with the identifier {restaurantId} was not found.");

        public static readonly Func<string, Error> NoRestaurantsFound = searchTerm => new(
                "Restaurant.NoRestaurantsFound",
                $"No restaurants were found matching this {searchTerm}.");

        public static readonly Error NotExist = new(
                "Restaurant.NotExist",
                $"There is no restaurants");


        public static readonly Error Concurrency = new(
                "Restaurant.Concurrency",
                $"DbUpdateConcurrencyException");
    }

    public static class Order
    {
        public static readonly Func<Guid, Error> NotFound = id => new(
                "Order.NotFound",
                $"The Order with the identifier {id} was not found.");

        public static readonly Error NotExist = new(
                "Order.NotExist",
                $"There is no orders");
    }

    public static class Email
    {
        public static readonly Error Empty = new(
            "Email.Empty",
            "Email is empty");
        public static readonly Error InvalidFormat = new(
            "Email.InvalidFormat",
            "Email format is invalid");
    }

    public static class FirstName
    {
        public static readonly Error Empty = new(
            "FirstName.Empty",
            "First name is empty");
        public static readonly Error TooLong = new(
            "LastName.TooLong",
            "FirstName name is too long");
    }

    public static class LastName
    {
        public static readonly Error Empty = new(
            "LastName.Empty",
            "Last name is empty");
        public static readonly Error TooLong = new(
            "LastName.TooLong",
            "Last name is too long");
    }

    public static class Address
    {
        public static readonly Error Empty = new(
            "Address.Empty",
            "Address is empty");
        public static readonly Error TooLong = new(
            "Address.TooLong",
            "Address is too long");
    }

    public static class OpeningHours
    {
        public static readonly Error InvalidTimeRange = new(
            "OpeningHours.InvalidTimeRange",
            "Opening time must be earlier than closing time.");
    }

    public static class DeliveryAddress
    {
        public static readonly Error StreetEmpty = new(
            "DeliveryAddress.StreetEmpty",
            "The street cannot be empty.");

        public static readonly Error CityEmpty = new(
            "DeliveryAddress.CityEmpty",
            "The city cannot be empty.");

        public static readonly Error StateEmpty = new(
            "DeliveryAddress.StateEmpty",
            "The state cannot be empty.");

        public static readonly Error PostalCodeEmpty = new(
            "DeliveryAddress.PostalCodeEmpty",
            "The postal code cannot be empty.");
    }

    public static class Vehicle
    {
        public static readonly Error TypeEmpty = new(
            "Vehicle.TypeEmpty",
            "Vehicle type cannot be empty.");

        public static readonly Error LicensePlateEmpty = new(
            "Vehicle.LicensePlateEmpty",
            "License plate cannot be empty.");
    }

    public static class Payment
    {
        public static readonly Error InvalidAmount = new(
            "Payment.InvalidAmount", 
            "The payment amount must be greater than zero.");
        
        public static readonly Error TransactionIdEmpty = new(
            "Payment.TransactionIdEmpty", 
            "The transaction ID cannot be empty.");

        public static readonly Func<Guid, Error> AlreadySet = id => new(
            "Payment.AlreadySet",
            $"The payment already set to this order. Setted payment Id : {id}.");
    }

    public static class Money
    {
        public static readonly Error InvalidAmount = new(
            "Money.InvalidAmount", 
            "The monetary amount must be greater than zero.");
        
        public static readonly Error CurrencyEmpty = new(
            "Money.CurrencyEmpty", 
            "The currency cannot be empty.");
    }
}

