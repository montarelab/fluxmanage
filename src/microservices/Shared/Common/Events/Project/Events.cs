namespace Common.Events.Project;

public class ProjectCreatedEvent(Guid projectId, string name, Guid userId, DateTime createdDate) 
    : DomainEvent(projectId)
{
    public string Name { get; set; } = name;
    public Guid UserId { get; set; } = userId;
    public DateTime CreatedDate { get; set; } = createdDate;
}
    
public class ProjectDeletedEvent(Guid projectId) : DomainEvent(projectId);

public class ProjectUpdatedEvent(Guid projectId, string title) : DomainEvent(projectId)
{
    public string Title { get; set; } = title;
}


