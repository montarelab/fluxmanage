using Common.Events;
using Common.EventSourcing;
using Common.Exceptions;

namespace Infrastructure.EventSourcing;

public class EventStore(IEventStoreRepository eventStoreRepository, IEventProducer eventProducer) : IEventStore
{
    public async Task SaveEventsAsync(Guid aggregateId, IEnumerable<DomainEvent> events, int expectedVersion)
    {
        var eventStream = await eventStoreRepository.FindByAggregateId(aggregateId);

        if (expectedVersion != -1 && eventStream[^1].Version != expectedVersion)// ^1 means the last element
        {
            throw new ConcurrencyException();
        }
        
        int version = expectedVersion;
        foreach (var @event in events)
        {
            version++;
            string eventType = @event.GetType().Name;

            var eventModel = new EventModel{
                TimeStamp = DateTime.Now,
                AggregateIdentifier = aggregateId,
                AggregateType = @event.AggregateType,
                Version = version,
                EventType = eventType,
                EventData = @event
            };
			
            await eventStoreRepository.SaveAsync(eventModel);

            string topic = Environment.GetEnvironmentVariable("KAFKA_TOPIC")!;
            await eventProducer.ProduceAsync(topic!, @event with { Version = version });
        }
    }

    public async Task<List<DomainEvent>> GetEventsAsync(Guid aggregateId)
    {
        var eventStream = await eventStoreRepository.FindByAggregateId(aggregateId);

        if (eventStream == null || eventStream.Count == 0)
        {
            throw new AggregateNotFoundException("Incorrect post Id provided! "+aggregateId);
        }

        return eventStream
            .OrderBy(x => x.Version)
            .Select(x => x.EventData)
            .ToList();
    }

    public async Task<List<Guid>> GetAggregateIdsAsync()
    {
        List<EventModel> eventStream = await eventStoreRepository.FindAllAsync();
		
        if (eventStream.Count == 0) return[];
		
        return eventStream
            .Select(x => x.AggregateIdentifier)
            .Distinct()
            .ToList();
    }
}
