namespace Common.Events.Task;

public record TaskAssignedEvent(Guid Id, Guid AssigneeId) : DomainEvent(Id);

public record TaskCompletedEvent(Guid Id) : DomainEvent(Id);

public record TaskDeletedEvent(Guid Id) : DomainEvent(Id);

public record TaskCreatedEvent(Guid Id, Guid ProjectId, string Title, Guid CreatedBy) : DomainEvent(Id);

public record TaskUpdatedEvent(Guid Id) : DomainEvent(Id);

// todo add further info 