using Common.Events;

namespace Common.EventSourcing;

public interface IEventProducer
{
    Task ProduceAsync<T>(string topic, T @event) where T : DomainEvent;
}