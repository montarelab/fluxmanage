using Common.Events;

namespace Common.EventSourcing;

public interface IEventStore
{
    Task SaveEventsAsync(Guid aggregateId, IEnumerable<DomainEvent> events, int expectedVersion);

    Task<List<DomainEvent>> GetEventsAsync(Guid aggregateId);
	
    Task<List<Guid>> GetAggregateIdsAsync();
}
