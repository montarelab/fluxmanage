using Common.Domain;

namespace Common.Exceptions;

public class NotFoundException<TEntity>(Guid entityId) 
    : Exception($"Entity {typeof(TEntity).Name} with id {entityId} not found")
    where TEntity : IEntity;