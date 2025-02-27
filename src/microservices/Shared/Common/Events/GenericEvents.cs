using Common.Domain;

namespace Common.Events;

public record EntityCreatedEvent<T>(T Entity) where T : IEntity;

public record EntityDeletedEvent<T>(Guid Id) where T : IEntity;

public class EntityUpdatedEvent<T> where T : IEntity
{
    public IDictionary<string, object>? FieldsChanged { get; set; }
}