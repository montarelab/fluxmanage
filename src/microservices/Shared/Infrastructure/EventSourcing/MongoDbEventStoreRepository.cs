using Common.Events;
using Common.EventSourcing;
using Infrastructure.Config;
using Microsoft.Extensions.Options;
using MongoDB.Driver;

namespace Infrastructure.EventSourcing;

public class MongoDbEventStoreRepository : IEventStoreRepository
{
    private readonly IMongoCollection<EventModel> _eventStoreCollection;

    public MongoDbEventStoreRepository(IOptions<MongoDbConfig> config)
    {
        var mongoClient = new MongoClient(config.Value.ConnectionString);
        var mongoDatabase = mongoClient.GetDatabase(config.Value.Database);

        _eventStoreCollection = mongoDatabase.GetCollection<EventModel>(config.Value.Collection);
    }

    public async Task SaveAsync(EventModel @event)
    {
        await _eventStoreCollection.InsertOneAsync(@event).ConfigureAwait(false);
    }

    public async Task<List<EventModel>> FindByAggregateId(Guid aggregateId)
    {
        return await _eventStoreCollection
            .Find(x => x.AggregateIdentifier.Equals(aggregateId))
            .ToListAsync()
            .ConfigureAwait(false);
    }

    public async Task<List<EventModel>> FindAllAsync()
    {
        return await _eventStoreCollection
            .Find(_ => true)
            .ToListAsync()
            .ConfigureAwait(false);
    }
}
