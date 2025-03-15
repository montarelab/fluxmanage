using System.Linq.Expressions;
using Common.Config;
using Common.Domain;
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
    private readonly ILogger<MongoEntityRepository<T>> _logger;

    public MongoEntityRepository(IOptions<MongoDbConfig> config, ILogger<MongoEntityRepository<T>> logger)
    {
        _logger = logger;
        var mongoClient = new MongoClient(config.Value.ConnectionString);
        var mongoDatabase = mongoClient.GetDatabase(config.Value.Database);
        _eventStoreCollection = mongoDatabase.GetCollection<T>(typeof(T).Name);
    }

    public async Task<T> GetByIdAsync(Guid id, CancellationToken ct)
    {
        _logger.LogInformation("Getting entity by id {Id}", id);
        return await _eventStoreCollection
            .Find(entity => entity.Id == id)
            .FirstOrDefaultAsync(ct);
    }

    public async Task<IEnumerable<T>> GetAllAsync(Expression<Func<T, bool>>? predicate, CancellationToken ct)
    {
        _logger.LogInformation("Getting all entities");
        return await _eventStoreCollection.Find(predicate ?? (entity => true)).ToListAsync(ct);
    }

    public async Task<Guid> AddAsync(T entity, CancellationToken ct)
    {
        await _eventStoreCollection.InsertOneAsync(entity, cancellationToken: ct);
        _logger.LogInformation("Entity with id {Id} was inserted", entity.Id);
        return entity.Id;
    }

    public async Task<Guid> UpdateAsync(T entity, CancellationToken ct)
    {
        var filter = Builders<T>.Filter.Eq(e => e.Id, entity.Id);
        var update = Builders<T>.Update.Set(e => e, entity);
        await _eventStoreCollection.ReplaceOneAsync(filter, entity, cancellationToken: ct);        _logger.LogInformation("Entity with id {Id} was updated", entity.Id);
        return entity.Id;
    }

    public async Task DeleteAsync(Guid id, CancellationToken ct)
    {
        var result = await _eventStoreCollection.DeleteOneAsync(entity => entity.Id == id, cancellationToken: ct);
        if (result.DeletedCount == 0)
        {
            _logger.LogError("Entity with id {Id} was not found", id);
            throw new Exception($"Entity with id {id} was not found");
        }

        _logger.LogInformation("Entity with id {Id} was deleted", id);
    }
}