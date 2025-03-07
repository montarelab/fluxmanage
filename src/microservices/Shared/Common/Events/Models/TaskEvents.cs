using Common.Domain.Entities;

namespace Common.Events.Models;

public record TicketCreatedEvent(Guid Id, Guid ProjectId, string Title, Guid CreatedBy) 
    : EntityCreatedEvent<Ticket>(Id);

public record TicketUpdatedEvent(
    Guid Id,
    string? Title,
    string? Description,
    DateTime? StartDate,
    DateTime? DueDate,
    Guid? AssigneeId,
    Guid? ParentTicketId,
    Guid? EpicId,
    int? EstimatedStoryPoints,
    TicketStatus? Status,
    IDictionary<string, string>? CustomFields
) : EntityUpdatedEvent<Ticket>(Id);

public record TicketDeletedEvent(Guid Id) : EntityDeletedEvent<Ticket>(Id);

public record TicketAssignedEvent(Guid Id, Guid AssigneeId) : DomainEvent<Ticket>(Id);

public record TicketCompletedEvent(Guid Id) : DomainEvent<Ticket>(Id);