using Common.Domain.Entities;
using Common.Events.Models;

namespace Common.Domain.Aggregates;

public class ProjectAggregate : AggregateRoot<Project>
{
    public ProjectAggregate() { }

    public ProjectAggregate(Guid id, string name, Guid createdBy)
    {
        RaiseEvent(new ProjectCreatedEvent
        (
            Id: id,
            Name: name,
            CreatedBy: createdBy
        ));
    }
    
    protected void Apply(ProjectCreatedEvent @event)
    {
        IsActive = true;
        Entity.Id = @event.Id;
        Id = @event.Id;
        Entity.Title = @event.Name;
        Entity.CreatedBy = @event.CreatedBy;
        Entity.CreatedDate = @event.TriggeredOn;
    }
    
    public void EditName(string title)
    {
        if (!IsActive)
        {
            throw new InvalidOperationException("You cannot edit deleted project!");
        }
        
        if(string.IsNullOrWhiteSpace(title))
        {
            throw new InvalidOperationException("Project name cannot be empty.");
        }

        if (string.Equals(title, Entity.Title, StringComparison.Ordinal))
        {
            throw new InvalidOperationException($"You cannot set the same name: {title}.");
        }
        
        RaiseEvent(new ProjectUpdatedEvent
        (
            Id: Entity.Id,
            Title: title
        ));
    }
    
    protected void Apply(ProjectUpdatedEvent @event)
    {
        base.Apply(@event);
    }
    
    public void DeleteProject()
    {
        if (!IsActive)
        {
            throw new InvalidOperationException("You cannot delete already deleted project!");
        }
        
        RaiseEvent(new ProjectDeletedEvent(Entity.Id));
    }
    
    protected void Apply(ProjectDeletedEvent @event)
    {
        Entity.Id = @event.Id;
        Id = @event.Id;
        IsActive = false;
    }
}