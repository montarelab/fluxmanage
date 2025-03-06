using Common.DTO;
using Common.Events.Models;
using TaskStatus = Common.Domain.Entities.TaskStatus;
using Task = Common.Domain.Entities.Task;
namespace Common.Domain.Aggregates;

public class TaskAggregate : IEntity<Task>
{
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
        Entity.Id = @event.Id;
        Id = @event.Id;
        Entity.Title = @event.Title;
        Entity.ProjectId = @event.ProjectId;
        Entity.CreatedBy = @event.CreatedBy;
        Entity.CreatedDate = @event.TriggeredOn;
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
        
        RaiseEvent(new TaskDeletedEvent(Entity.Id));
    }
    
    protected void Apply(TaskDeletedEvent @event)
    {
        Entity.Id = @event.Id;
        IsActive = false;
    }
    
    public void CompleteTask()
    {
        RaiseEvent(new TaskCompletedEvent(Entity.Id));
    }
    
    protected void Apply(TaskCompletedEvent @event)
    {
        Entity.Id = @event.Id;
        Entity.Status = TaskStatus.Completed;
    }
    
    public void AssignTask(Guid assigneeId)
    {
        RaiseEvent(new TaskAssignedEvent(Entity.Id, assigneeId));
    }
    
    protected void Apply(TaskAssignedEvent @event)
    {
        Entity.Id = @event.Id;
        Id = @event.Id;
        Entity.AssigneeId = @event.AssigneeId;
    }
}