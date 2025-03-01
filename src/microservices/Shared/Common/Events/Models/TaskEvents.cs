using Common.Domain.Models;
using TaskStatus = Common.Domain.Models.TaskStatus;

namespace Common.Events.Models;

public record TaskCreatedEvent(Guid Id, Guid ProjectId, string Title, Guid CreatedBy) 
    : EntityCreatedEvent<TaskAggregate>(Id);

public record TaskUpdatedEvent(
    Guid Id,
    string? Title,
    string? Description,
    DateTime? StartDate,
    DateTime? DueDate,
    Guid? AssigneeId,
    Guid? ParentTaskId,
    Guid? EpicId,
    int? EstimatedStoryPoints,
    TaskStatus? Status,
    IDictionary<string, string>? CustomFields
) : EntityUpdatedEvent<TaskAggregate>(Id);

public record TaskDeletedEvent(Guid Id) : EntityDeletedEvent<TaskAggregate>(Id);

public record TaskAssignedEvent(Guid Id, Guid AssigneeId) : DomainEvent<TaskAggregate>(Id);

public record TaskCompletedEvent(Guid Id) : DomainEvent<TaskAggregate>(Id);