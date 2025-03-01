using Common.DTO;
using Common.Events.Models;

namespace Common.Domain.Models;

public class TaskAggregate : AggregateRoot
{
    private string Title { get; set; } = string.Empty;
    private string Description { get; set; } = string.Empty;
    private Guid CreatedBy { get; set; } = Guid.Empty;
    private Guid? EpicId { get; set; } 
    private Guid? ParentTaskId { get; set; }
    private int EstimatedStoryPoints { get; set; } = 0;
    private DateTime CreatedDate { get; set; } = DateTime.Now;
    private DateTime StartDate { get; set; } = DateTime.Now; 
    private DateTime DueDate { get; set; } = DateTime.Now.AddDays(7);
    private IDictionary<string, string>? CustomFields { get; set; }
    
    private Guid AssigneeId { get; set; }
    public Guid ProjectId { get; set; }
    public TaskStatus Status { get; set; } = TaskStatus.Created;
    
    public TaskAggregate() { }

    public TaskAggregate(Guid id, Guid projectId, string title, Guid createdBy)
    {
        RaiseEvent(new TaskCreatedEvent
        (
            Id: id,
            ProjectId: projectId,
            Title: title,
            CreatedBy: createdBy
        ));
    }
    
    protected void Apply(TaskCreatedEvent @event)
    {
        IsActive = true;
        Id = @event.Id;
        Title = @event.Title;
        ProjectId = @event.ProjectId;
        CreatedBy = @event.CreatedBy;
        CreatedDate = @event.TriggeredOn;
    }

    public void Update(TaskUpdateData updatedData)
    {
        if (!IsActive)
        {
            throw new InvalidOperationException("You cannot edit deleted task!");
        }
        
        RaiseEvent(new TaskUpdatedEvent(
            updatedData.Id,
            updatedData.Title,
            updatedData.Description,
            updatedData.StartDate,
            updatedData.DueDate,
            updatedData.AssigneeId,
            updatedData.ParentTaskId,
            updatedData.EpicId,
            updatedData.EstimatedStoryPoints,
            updatedData.Status,
            updatedData.CustomFields));
    }
    
    protected void Apply(TaskUpdatedEvent @event)
    {
        base.Apply(@event);
    }
    
    public void DeleteTask()
    {
        if (!IsActive)
        {
            throw new InvalidOperationException("You cannot delete already deleted task!");
        }
        
        RaiseEvent(new TaskDeletedEvent(Id));
    }
    
    protected void Apply(TaskDeletedEvent @event)
    {
        Id = @event.Id;
        IsActive = false;
    }
    
    public void CompleteTask()
    {
        RaiseEvent(new TaskCompletedEvent(Id));
    }
    
    protected void Apply(TaskCompletedEvent @event)
    {
        Id = @event.Id;
        Status = TaskStatus.Completed;
    }
    
    public void AssignTask(Guid assigneeId)
    {
        RaiseEvent(new TaskAssignedEvent(Id, assigneeId));
    }
    
    protected void Apply(TaskAssignedEvent @event)
    {
        Id = @event.Id;
        AssigneeId = @event.AssigneeId;
    }
}