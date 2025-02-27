using Common.Events;
using Common.EventSourcing;

namespace Infrastructure.EventSourcing;

public class EventProducer : IEventProducer
{
    public Task ProduceAsync<T>(string topic, T @event) where T : DomainEvent
    {
        throw new NotImplementedException();
    }
}