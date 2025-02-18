using Gravy.Domain.Primitives;
using Gravy.Persistence.Outbox;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Diagnostics;
using Newtonsoft.Json;

namespace Gravy.Persistence.Interceptors;

/// <summary>
/// Intercepts save changes operations to convert domain events into outbox messages.
/// </summary>
public sealed class ConvertDomainEventsToOutboxMessagesInterceptor
    : SaveChangesInterceptor
{
    /// <summary>
    /// Overrides the async saving changes method to convert domain events into outbox messages.
    /// </summary>
    /// <param name="eventData">The event data containing information about the DbContext.</param> 
    /// <param name="result">The result of the interception.</param> 
    /// <param name="cancellationToken">A token to monitor for cancellation requests.</param>
    /// <returns></returns>
    public override ValueTask<InterceptionResult<int>> SavingChangesAsync(
        DbContextEventData eventData,
        InterceptionResult<int> result,
        CancellationToken cancellationToken = default)
    {
        // Get the current DbContext
        DbContext dbContext = eventData.Context;
        if (dbContext is null)
        {
            return base.SavingChangesAsync(eventData, result, cancellationToken);
        }

        // Convert domain events to outbox messages
        var outboxMessages = dbContext.ChangeTracker
            .Entries<AggregateRoot>() // Get entries of AggregateRoot type 
            .Select(x => x.Entity) // Select the AggregateRoot entities
            .SelectMany(aggregateRoot =>
            {
                var domainEvents = aggregateRoot.GetDomainEvents(); // Get domain events
                aggregateRoot.ClearDomainEvents(); // Clear domain events from aggregate
                return domainEvents; // Return domain events
            })
            .Select(domainEvent => new OutboxMessage // Create new OutBoxMessage for adding dbContext
            {
                Id = Guid.NewGuid(),
                OccurredOnUtc = DateTime.UtcNow,
                Type = domainEvent.GetType().Name,
                Content = JsonConvert.SerializeObject(
                    domainEvent,
                    new JsonSerializerSettings
                    {
                        TypeNameHandling = TypeNameHandling.All,
                    })
            })
            .ToList();

        // Add outbox messages to the DbContext
        dbContext.Set<OutboxMessage>().AddRange(outboxMessages);

        // Continue with the base saving changes operation
        return base.SavingChangesAsync(eventData, result, cancellationToken);
    }
}