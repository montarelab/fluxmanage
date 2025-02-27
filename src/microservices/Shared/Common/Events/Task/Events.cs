namespace Common.Events.Task;

public class TaskAssignedEvent(Guid taskId, Guid assigneeId) : DomainEvent(taskId)
{
    public Guid AssigneeId { get; set; } = assigneeId;
}

public class TaskCompletedEvent(Guid taskId) : DomainEvent(taskId);

public class TaskDeletedEvent(Guid taskId) : DomainEvent(taskId);

public class TaskCreatedEvent(Guid taskId, Guid projectId, string title) : DomainEvent(taskId)
{
    public string Title { get; set; } = title;
    public Guid ProjectId { get; set; } = projectId;
}


public class TaskUpdatedEvent(Guid taskId) : DomainEvent(taskId);

// todo add further info 