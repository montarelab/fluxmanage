namespace Common.Events.Project;

public class ProjectCreatedEvent(Guid projectId, string name, Guid userId, DateTime createdDate) 
    : BaseEvent(projectId)
{
    public string Name { get; set; } = name;
    public Guid UserId { get; set; } = userId;
    public DateTime CreatedDate { get; set; } = createdDate;
}
    
public class ProjectDeletedEvent(Guid projectId) : BaseEvent(projectId);

public abstract class ProjectUpdatedEvent(Guid projectId) : BaseEvent(projectId);

public class ProjectRenamedEvent(Guid projectId, string title) : ProjectUpdatedEvent(projectId)
{
    public string Title { get; set; } = title;
}


public class ProjectPermissionsUpdatedEvent(Guid projectId) : ProjectUpdatedEvent(projectId);

// todo think how we can improve it and add further details
