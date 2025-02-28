using System.Reflection;
using Common.DTO;
using Common.Events.Task;
using TaskStatus = TaskCommon.Domain.TaskStatus;

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
    
    private void Apply(TaskCreatedEvent @event)
    {
        IsActive = true;
        Id = @event.Id;
        Title = @event.Title;
        ProjectId = @event.ProjectId;
        CreatedDate = @event.CreatedDate;
    }

    public void Update(TaskUpdateData updatedData)
    {
        if (!IsActive)
        {
            throw new InvalidOperationException("You cannot edit deleted project!");
        }
        
        RaiseEvent(new TaskUpdatedEvent
        (
            Id: Id,
            updatedData.Changes
        ));
    }
    
    private void Apply(TaskUpdatedEvent @event)
    {
        base.Apply(@event);
    }
    
    public void DeleteTask()
    {
        RaiseEvent(new TaskDeletedEvent(Id));
    }
    
    private void Apply(TaskDeletedEvent @event)
    {
        Id = @event.Id;
        IsActive = false;
    }
    
    public void CompleteTask()
    {
        RaiseEvent(new TaskCompletedEvent(Id));
    }
    
    private void Apply(TaskCompletedEvent @event)
    {
        Id = @event.Id;
        Status = TaskStatus.Completed;
    }
    
    public void AssignTask(Guid assigneeId)
    {
        RaiseEvent(new TaskAssignedEvent(Id, assigneeId));
    }
    
    private void Apply(TaskAssignedEvent @event)
    {
        Id = @event.Id;
        AssigneeId = @event.AssigneeId;
    }
}