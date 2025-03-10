using Common.Events;
using Common.EventSourcing;
using Common.Exceptions;
using Microsoft.Extensions.Logging;

namespace Infrastructure.EventSourcing;

public class EventStore(
    IEventStoreRepository eventStoreRepository, 
    IEventProducer eventProducer, 
    ILogger<EventStore> logger) 
: IEventStore
{
    public async Task SaveEventsAsync(Guid aggregateId, IEnumerable<DomainEvent> events, int expectedVersion)
    {
        var eventStream = await eventStoreRepository.FindEventsByAggregateId(aggregateId);

        if (expectedVersion != -1 && eventStream[^1].Version != expectedVersion) // ^1 means the last element
        {
            logger.LogError("Concurrency exception for aggregate id {Id}", aggregateId);
            throw new ConcurrencyException();
        }
        
        int version = expectedVersion;
        foreach (var @event in events)
        {
            version++;
            var versionedEvent = @event with { Version = version };

            var eventModel = new EventModel{
                TimeStamp = DateTime.Now,
                AggregateIdentifier = aggregateId,
                AggregateType = @event.AggregateType,
                Version = version,
                EventType = @event.EventType,
                EventData = versionedEvent
            };
            
            await eventStoreRepository.SaveAsync(eventModel);
            logger.LogInformation("Event with id {Id} saved for aggregate id {AggregateId}", eventModel.Id, aggregateId);

            string topic = Environment.GetEnvironmentVariable("KAFKA_TOPIC")!;
            await eventProducer.ProduceAsync(topic, versionedEvent);
        }
    }

    public async Task<List<DomainEvent>> GetEventsByAggregateIdAsync(Guid aggregateId)
    {
        logger.LogInformation("Getting events by aggregate id {Id}", aggregateId);
        var eventStream = await eventStoreRepository.FindEventsByAggregateId(aggregateId);

        if (eventStream == null || eventStream.Count == 0)
        {
            logger.LogError("Aggregate not found for id {Id}", aggregateId);
            throw new AggregateNotFoundException("Incorrect aggregate Id provided! " + aggregateId);
        }

        return eventStream
            .OrderBy(x => x.Version)
            .Select(x => x.EventData)
            .ToList();
    }

    public async Task<List<Guid>> GetAggregateIdsAsync()
    {
        logger.LogInformation("Getting all aggregate ids");
        List<EventModel> eventStream = await eventStoreRepository.FindAllEventsAsync();
        
        if (eventStream.Count == 0) return [];
        
        return eventStream
            .Select(x => x.AggregateIdentifier)
            .Distinct()
            .ToList();
    }
}