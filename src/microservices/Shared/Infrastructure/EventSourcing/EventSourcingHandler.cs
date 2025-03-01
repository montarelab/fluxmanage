using Common.Domain;
using Common.Events;
using Common.EventSourcing;

namespace Infrastructure.EventSourcing;

public class EventSourcingHandler<TAggregateRoot>(
    IEventStore eventStore
) : IEventSourcingHandler<TAggregateRoot> where TAggregateRoot : AggregateRoot, new()
{
    public async Task SaveAsync(TAggregateRoot aggregateRoot)
    {
        await eventStore.SaveEventsAsync(aggregateRoot.Id, aggregateRoot.GetUncommittedChanges(), aggregateRoot.Version);
        aggregateRoot.MarkChangesAsCommitted();
    }

    public async Task<TAggregateRoot?> GetByIdAsync(Guid aggregateId)
    {
        Console.WriteLine("AggregateId in Sourcing handler: "+aggregateId);
        var events = await eventStore.GetEventsAsync(aggregateId);
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
