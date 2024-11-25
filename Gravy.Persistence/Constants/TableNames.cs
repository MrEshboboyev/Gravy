namespace Gravy.Persistence.Constants;

/// <summary> 
/// Provides constants for table names. 
/// </summary>
internal static class TableNames
{
    internal const string Users = nameof(Users);
    internal const string OutboxMessages = nameof(OutboxMessages);
    internal const string OutboxMessageConsumers = nameof(OutboxMessageConsumers);
    internal const string Roles = nameof(Roles);
    internal const string Permissions = nameof(Permissions);
}
