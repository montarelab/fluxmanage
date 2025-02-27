using Common.Domain;

namespace Common.EventSourcing;

public interface IEventSourcingHandler<T>
{
    Task SaveAsync(AggregateRoot aggregateRoot);

    Task<T> GetByIdAsync(Guid aggregateId);
	
    Task RepublishEventsAsync();
}