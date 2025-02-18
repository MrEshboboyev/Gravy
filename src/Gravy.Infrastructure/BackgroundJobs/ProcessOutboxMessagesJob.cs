using Gravy.Domain.Primitives;
using Gravy.Persistence.Outbox;
using Gravy.Persistence;
using MediatR;
using Newtonsoft.Json;
using Quartz;
using Microsoft.EntityFrameworkCore;
using Polly.Retry;
using Polly;

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
            // Deserialize the domain event from the message content
            var domainEvent = JsonConvert
                .DeserializeObject<IDomainEvent>(
                    outboxMessage.Content,
                    new JsonSerializerSettings 
                    {
                        TypeNameHandling = TypeNameHandling.All
                    });

            if (domainEvent is null)
            {
                continue;
            }

            // Define a retry policy to handle transient exceptions
            AsyncRetryPolicy policy = Policy
                    .Handle<Exception>()
                    .WaitAndRetryAsync(3, attempt => TimeSpan.FromMilliseconds(50 * attempt));

            // Execute the publish operation with retry policy
            PolicyResult result = await policy.ExecuteAndCaptureAsync(() =>
                _publisher.Publish(
                    domainEvent,
                    context.CancellationToken));

            // Record any errors that occurred during publishing
            outboxMessage.Error = result.FinalException?.ToString();

            // Mark the outbox message as processed
            outboxMessage.ProcessedOnUtc = DateTime.UtcNow;
        }

        // Save changes to the database
        await _dbContext.SaveChangesAsync();
    }
}