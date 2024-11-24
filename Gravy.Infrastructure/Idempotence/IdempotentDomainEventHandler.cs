using Gravy.Application.Abstractions.Messaging;
using Gravy.Domain.Primitives;
using Gravy.Persistence;
using Gravy.Persistence.Outbox;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace Gravy.Infrastructure.Idempotence;

/// <summary> 
/// Handles domain events in an idempotent manner, ensuring that an event is processed only once. 
/// </summary> 
/// <typeparam name="TDomainEvent">The type of the domain event.</typeparam>
public class IdempotentDomainEventHandler<TDomainEvent> : IDomainEventHandler<TDomainEvent>
    where TDomainEvent : IDomainEvent
{
    private readonly INotificationHandler<TDomainEvent> _decorated;
    private readonly ApplicationDbContext _dbContext;
    public IdempotentDomainEventHandler(INotificationHandler<TDomainEvent> decorated,
        ApplicationDbContext dbContext)
    {
        _decorated = decorated;
        _dbContext = dbContext;
    }

    /// <summary> 
    /// Handles the domain event. 
    /// </summary> 
    /// <param name="notification">The domain event notification.</param> 
    /// <param name="cancellationToken">Cancellation token.</param>
    public async Task Handle(TDomainEvent notification, CancellationToken cancellationToken)
    {
        string consumer = _decorated.GetType().Name;

        // Check if the domain event has already been processed by querying the database.
        if (await _dbContext.Set<OutboxMessageConsumer>()
            .AnyAsync(o => o.Id == notification.Id &&
                           o.Name == consumer, cancellationToken: cancellationToken))
        {
            return; // Event already processed, exit method.
        }

        // Handle the domain event.
        await _decorated.Handle(notification, cancellationToken);

        // Record the processing of the event.
        _dbContext.Set<OutboxMessageConsumer>()
            .Add(new OutboxMessageConsumer
            {
                Id = notification.Id,
                Name = consumer
            });

        // Save changes to the database.
        await _dbContext.SaveChangesAsync(cancellationToken);
    }
}