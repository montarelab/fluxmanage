using Common.Domain;

namespace Common.EventSourcing;

public interface IEventSourcingHandler<TAggregate> where TAggregate : IAggregateRoot  
{
    Task SaveAggregateAsync(TAggregate aggregateRoot);

    Task<TAggregate?> GetAggregateByIdAsync(Guid aggregateId);
}