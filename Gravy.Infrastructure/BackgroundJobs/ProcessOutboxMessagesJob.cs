using Gravy.Domain.Primitives;
using Gravy.Persistence.Outbox;
using Gravy.Persistence;
using MediatR;
using Newtonsoft.Json;
using Quartz;
using Microsoft.EntityFrameworkCore;

namespace Gravy.Infrastructure.BackgroundJobs;


/// <summary> 
/// Background job for processing outbox messages. This job retrieves unprocessed outbox messages, 
/// deserializes the domain events, publishes them, and marks the messages as processed. 
/// </summary>
[DisallowConcurrentExecution]
public class ProcessOutboxMessagesJob : IJob
{
    private readonly ApplicationDbContext _dbContext;
    private readonly IPublisher _publisher;

    /// <summary> 
    /// Initializes a new instance of the <see cref="ProcessOutboxMessagesJob"/> class. 
    /// </summary> 
    /// <param name="dbContext">The application database context.</param> 
    /// <param name="publisher">The publisher for domain events.</param>
    public ProcessOutboxMessagesJob(ApplicationDbContext dbContext, IPublisher publisher)
    {
        _dbContext = dbContext;
        _publisher = publisher;
    }

    /// <summary> 
    /// Executes the job to process outbox messages. 
    /// </summary> 
    /// <param name="context">The job execution context.</param>
    public async Task Execute(IJobExecutionContext context)
    {
        // Retrieve unprocessed outbox messages
        var messages = await _dbContext
            .Set<OutboxMessage>()
            .Where(m => m.ProcessedOnUtc == null)
            .Take(20)
            .ToListAsync(context.CancellationToken);

        // Process each outbox message
        foreach (OutboxMessage outboxMessage in messages)
        {
            var domainEvent = JsonConvert
                .DeserializeObject<IDomainEvent>(outboxMessage.Content);
            if (domainEvent is null)
            {
                continue;
            }

            // Publish the domain event
            await _publisher.Publish(domainEvent, context.CancellationToken);

            // Mark the outbox message as processed
            outboxMessage.ProcessedOnUtc = DateTime.Now;
        }

        // Save changes to the database
        await _dbContext.SaveChangesAsync();
    }
}