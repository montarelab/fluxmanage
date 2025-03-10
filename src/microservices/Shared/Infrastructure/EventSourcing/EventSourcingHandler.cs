using Common.Domain;
using Common.EventSourcing;
using Microsoft.Extensions.Logging;

namespace Infrastructure.EventSourcing;

public class EventSourcingHandler<TAggregateRoot>(
    IEventStore eventStore,
    ILogger<EventSourcingHandler<TAggregateRoot>> logger
) : IEventSourcingHandler<TAggregateRoot> where TAggregateRoot : class, IAggregateRoot, new()
{
    public async Task SaveAggregateAsync(TAggregateRoot aggregateRoot)
    {
        logger.LogInformation("Saving aggregate with id {Id}", aggregateRoot.Id);
        await eventStore.SaveEventsAsync(aggregateRoot.Id, aggregateRoot.GetUncommittedChanges(), aggregateRoot.Version);
        aggregateRoot.MarkChangesAsCommitted();
    }

    public async Task<TAggregateRoot?> GetAggregateByIdAsync(Guid aggregateId)
    {
        logger.LogInformation("Getting aggregate by id {Id}", aggregateId);
        var events = await eventStore.GetEventsByAggregateIdAsync(aggregateId);
        if (events is not { Count: > 0 })
        {
            logger.LogWarning("No events found for aggregate id {Id}", aggregateId);
            return null;
        }
        
        var aggregate = new TAggregateRoot();
        aggregate.ReplayEvents(events);
        aggregate.Version = events.Select(x => x.Version).Max();
        return aggregate;
    }
}