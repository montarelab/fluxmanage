using TaskStatus = Common.Domain.Entities.TaskStatus;
using Task = Common.Domain.Entities.Task;
namespace Common.Events.Models;

public record TaskCreatedEvent(Guid Id, Guid ProjectId, string Title, Guid CreatedBy) 
    : EntityCreatedEvent<Task>(Id);

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
) : EntityUpdatedEvent<Task>(Id);

public record TaskDeletedEvent(Guid Id) : EntityDeletedEvent<Task>(Id);

public record TaskAssignedEvent(Guid Id, Guid AssigneeId) : DomainEvent<Task>(Id);

public record TaskCompletedEvent(Guid Id) : DomainEvent<Task>(Id);