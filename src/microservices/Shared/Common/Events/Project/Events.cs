namespace Common.Events.Project;

public class ProjectCreatedEvent(Guid projectId, string name, Guid createdBy) 
    : DomainEvent(projectId)
{
    public string Name { get; set; } = name;
    public Guid CreatedBy { get; set; } = createdBy;
}
    
public class ProjectDeletedEvent(Guid projectId) : DomainEvent(projectId);

public class ProjectUpdatedEvent(Guid projectId, string title) : DomainEvent(projectId)
{
    public string Title { get; set; } = title;
}


