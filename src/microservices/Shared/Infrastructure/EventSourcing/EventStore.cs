using Common.Events;
using Common.EventSourcing;

namespace Infrastructure.EventSourcing;

public class EventStore : IEventStore
{
    public Task SaveEventsAsync(Guid aggregateId, IEnumerable<DomainEvent> events, int expectedVersion)
    {
        throw new NotImplementedException();
    }

    public Task<List<DomainEvent>> GetEventsAsync(Guid aggregateId)
    {
        throw new NotImplementedException();
    }

    public Task<List<Guid>> GetAggregateIdsAsync()
    {
        throw new NotImplementedException();
    }
}
