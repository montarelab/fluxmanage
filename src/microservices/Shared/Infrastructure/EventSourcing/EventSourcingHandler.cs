using Common.Domain;
using Common.EventSourcing;

namespace Infrastructure.EventSourcing;

public class EventSourcingHandler<T> : IEventSourcingHandler<T> where T : AggregateRoot
{
    public Task SaveAsync(AggregateRoot aggregateRoot)
    {
        throw new NotImplementedException();
    }

    public Task<T> GetByIdAsync(Guid aggregateId)
    {
        throw new NotImplementedException();
    }

    public Task RepublishEventsAsync()
    {
        throw new NotImplementedException();
    }
}
