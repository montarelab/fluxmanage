namespace Common.Events.Task;

public class TaskAssignedEvent(Guid taskId, Guid assigneeId) : BaseEvent(taskId)
{
    public Guid AssigneeId { get; set; } = assigneeId;
}

public class TaskCompletedEvent(Guid taskId) : BaseEvent(taskId);

public class TaskDeletedEvent(Guid taskId) : BaseEvent(taskId);

public class TaskCreatedEvent(Guid taskId, Guid projectId, string title) : BaseEvent(taskId)
{
    public string Title { get; set; } = title;
    public Guid ProjectId { get; set; } = projectId;
}


public class TaskUpdatedEvent(Guid taskId) : BaseEvent(taskId);

// todo add further info 