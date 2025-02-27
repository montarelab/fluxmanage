using Common.Domain;

namespace Common.EventSourcing;

public interface IEventSourcingHandler<TAggregateRoot> where TAggregateRoot : AggregateRoot
{
    Task SaveAsync(TAggregateRoot aggregateRoot);

    Task<TAggregateRoot?> GetByIdAsync(Guid aggregateId);
}