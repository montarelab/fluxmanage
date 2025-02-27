namespace Common.Events.Project;

public record ProjectCreatedEvent(Guid Id, string Name, Guid CreatedBy) : DomainEvent(Id);
    
public record ProjectDeletedEvent(Guid Id) : DomainEvent(Id);

public record ProjectUpdatedEvent(Guid Id, string Title) : DomainEvent(Id);


