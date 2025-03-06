using Common.Domain;
using Common.Events;
using Common.EventSourcing;

namespace Infrastructure.EventSourcing;

public class EventSourcingHandler<TAggregateRoot>(
    IEventStore eventStore
) : IEventSourcingHandler<TAggregateRoot> where TAggregateRoot : class, IAggregateRoot, new()
{
    public async Task SaveAggregateAsync(TAggregateRoot aggregateRoot)
    {
        await eventStore.SaveEventsAsync(aggregateRoot.Id, aggregateRoot.GetUncommittedChanges(), aggregateRoot.Version);
        aggregateRoot.MarkChangesAsCommitted();
    }

    public async Task<TAggregateRoot?> GetAggregateByIdAsync(Guid aggregateId)
    {
        Console.WriteLine("AggregateId in Sourcing handler: "+aggregateId);
        var events = await eventStore.GetEventsByAggregateIdAsync(aggregateId);
        if (events is not { Count: > 0 })
        {
            return null;
        }
        
        var aggregate = new TAggregateRoot();
        aggregate.ReplayEvents(events);
        aggregate.Version = events.Select(x => x.Version).Max();
        return aggregate;
    }
}
