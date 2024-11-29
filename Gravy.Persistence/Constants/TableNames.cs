namespace Gravy.Persistence.Constants;

/// <summary> 
/// Provides constants for table names. 
/// </summary>
internal static class TableNames
{
    #region OutboxMessage
    internal const string OutboxMessages = nameof(OutboxMessages);
    internal const string OutboxMessageConsumers = nameof(OutboxMessageConsumers);
    #endregion

    #region User
    internal const string Users = nameof(Users);
    internal const string Customers = nameof(Customers);
    internal const string DeliveryPersons = nameof(DeliveryPersons);
    internal const string Roles = nameof(Roles);
    internal const string Permissions = nameof(Permissions);
    #endregion

    #region Order
    internal const string Orders = nameof(Orders);
    internal const string OrderItems = nameof(OrderItems);
    internal const string Deliveries = nameof(Deliveries);
    internal const string Payments = nameof(Payments);
    #endregion

    #region Restaurant
    internal const string Restaurants = nameof(Restaurants);
    internal const string MenuItems = nameof(MenuItems);
    #endregion
}
