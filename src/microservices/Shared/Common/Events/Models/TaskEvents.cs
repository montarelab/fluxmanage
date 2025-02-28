using Common.Domain.Models;

namespace Common.Events.Models;

public record TaskCreatedEvent(Guid Id, Guid ProjectId, string Title, Guid CreatedBy) 
    : EntityCreatedEvent<TaskAggregate>(Id);

public record TaskUpdatedEvent(Guid Id, IDictionary<string, object> FieldsChanged) 
    : EntityUpdatedEvent<TaskAggregate>(Id, FieldsChanged);

public record TaskDeletedEvent(Guid Id) : EntityDeletedEvent<TaskAggregate>(Id);

public record TaskAssignedEvent(Guid Id, Guid AssigneeId) : DomainEvent<TaskAggregate>(Id);

public record TaskCompletedEvent(Guid Id) : DomainEvent<TaskAggregate>(Id);