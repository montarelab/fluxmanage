using Common.Events;
using Common.EventSourcing;
using Infrastructure.Config;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using MongoDB.Bson;
using MongoDB.Driver;

namespace Infrastructure.EventSourcing;

public class MongoDbEventStoreRepository : IEventStoreRepository
{
    private readonly IMongoCollection<EventModel> _eventStoreCollection;
    private readonly ILogger<MongoDbEventStoreRepository> _logger;

    public MongoDbEventStoreRepository(IOptions<MongoDbConfig> config, ILogger<MongoDbEventStoreRepository> logger)
    {
        _logger = logger;
        var mongoClient = new MongoClient(config.Value.ConnectionString);
        var mongoDatabase = mongoClient.GetDatabase(config.Value.Database);
        _eventStoreCollection = mongoDatabase.GetCollection<EventModel>(config.Value.Collection);
    }

    public async Task SaveAsync(EventModel @event)
    {
        await _eventStoreCollection.InsertOneAsync(@event).ConfigureAwait(false);
        _logger.LogInformation($"Event with Id {@event.Id} was saved to the event store");
    }

    public async Task<List<EventModel>> FindEventsByAggregateId(Guid aggregateId)
    {
        var filter = Builders<EventModel>
            .Filter.Eq("AggregateIdentifier", new BsonBinaryData(aggregateId, GuidRepresentation.Standard));
       
        // todo it's not a problem
        
        return await _eventStoreCollection
            .Find(filter)
            .ToListAsync()
            .ConfigureAwait(false);
    }

    public async Task<List<EventModel>> FindAllEventsAsync()
    {
        return await _eventStoreCollection
            .Find(_ => true)
            .ToListAsync()
            .ConfigureAwait(false);
    }
}
