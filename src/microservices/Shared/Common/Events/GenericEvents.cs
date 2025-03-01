using Common.Domain;

namespace Common.Events;

public abstract record EntityCreatedEvent<TEntity>(Guid Id) : DomainEvent<TEntity>(Id) 
    where TEntity : IEntity;

public abstract record EntityDeletedEvent<TEntity>(Guid Id) : DomainEvent<TEntity>(Id)
    where TEntity : IEntity;

public abstract record EntityUpdatedEvent<TEntity>(Guid Id) : DomainEvent<TEntity>(Id) 
    where TEntity : IEntity;
