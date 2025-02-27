using Common.Events;

namespace Common.EventSourcing;

public interface IEventProducer
{
    // <summary> Produces event to message broker <summary>
    Task ProduceAsync<T>(string topic, T @event) where T : DomainEvent;
}