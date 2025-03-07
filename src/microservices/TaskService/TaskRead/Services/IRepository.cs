using System.Linq.Expressions;
using Common.Domain;
using Common.Events;
using Infrastructure.Config;
using Infrastructure.EventSourcing;
using Microsoft.Extensions.Options;
using MongoDB.Driver;

namespace TaskRead.Services;

public interface IRepository<T> where T : IEntity
{
    Task<T> GetByIdAsync(Guid id, CancellationToken ct);
    Task<IEnumerable<T>> GetAllAsync(Expression<Func<T, bool>>? predicate, CancellationToken ct);
    Task<Guid> AddAsync(T entity, CancellationToken ct);
    Task<Guid> UpdateAsync(T entity, CancellationToken ct);
    Task DeleteAsync(Guid id, CancellationToken ct);
}

public abstract class MongoEntityRepository<T> : IRepository<T> where T : IEntity
{
    private readonly IMongoCollection<T> _eventStoreCollection;
    private readonly ILogger<MongoDbEventStoreRepository> _logger;

    public MongoEntityRepository(IOptions<MongoDbConfig> config, ILogger<MongoDbEventStoreRepository> logger)
    {
        _logger = logger;
        var mongoClient = new MongoClient(config.Value.ConnectionString);
        var mongoDatabase = mongoClient.GetDatabase(config.Value.Database);
        _eventStoreCollection = mongoDatabase.GetCollection<T>(typeof(T).Name);
    }

    public Task<T> GetByIdAsync(Guid id, CancellationToken ct)
    {
        return _eventStoreCollection
            .Find(entity => entity.Id == id)
            .FirstOrDefaultAsync(ct);
    }

    public async Task<IEnumerable<T>> GetAllAsync(Expression<Func<T, bool>>? predicate, CancellationToken ct)
    {
        return await _eventStoreCollection.Find(predicate ?? (entity => true)).ToListAsync(ct);
    }

    public async Task<Guid> AddAsync(T entity, CancellationToken ct)
    {
        await _eventStoreCollection.InsertOneAsync(entity, cancellationToken: ct);
        _logger.LogInformation($"Entity with id {entity.Id} was inserted");
        return entity.Id;
    }

    public async Task<Guid> UpdateAsync(T entity, CancellationToken ct)
    {
        var filter = Builders<T>.Filter.Eq(e => e.Id, entity.Id);
        var update = Builders<T>.Update.Set(e => e, entity);
        await _eventStoreCollection.UpdateOneAsync(filter, update, cancellationToken: ct);
        _logger.LogInformation($"Entity with id {entity.Id} was updated");
        return entity.Id;
    }

    public async Task DeleteAsync(Guid id, CancellationToken ct)
    {
        var result = await _eventStoreCollection.DeleteOneAsync(entity => entity.Id == id, cancellationToken: ct);
        if(result.DeletedCount == 0)
        {
            throw new Exception($"Entity with id {id} was not found");
        }
        
        _logger.LogInformation($"Entity with id {id} was deleted");
    }
}