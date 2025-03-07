using Common.Domain.Entities;
using Common.Events.Models;

namespace Common.Domain.Aggregates;

public class EpicAggregate : AggregateRoot<Epic>
{
    public EpicAggregate() { }

    public EpicAggregate(Guid id, string name, Guid projectId, Guid createdBy)
    {
        RaiseEvent(new EpicCreatedEvent
        (
            Id: id,
            Name: name,
            ProjectId: projectId,
            CreatedBy: createdBy
        ));
    }
    
    protected void Apply(EpicCreatedEvent @event)
    {
        IsActive = true;
        Entity.Id = @event.Id;
        Id = @event.Id;
        Entity.Title = @event.Name;
        Entity.CreatedBy = @event.CreatedBy;
        Entity.ProjectId = @event.ProjectId;
        Entity.CreatedDate = @event.TriggeredOn;
    }
    
    public void EditName(string title)
    {
        if (!IsActive)
        {
            throw new InvalidOperationException("You cannot edit deleted epic!");
        }
        
        if(string.IsNullOrWhiteSpace(title))
        {
            throw new InvalidOperationException("Epic name cannot be empty.");
        }

        if (string.Equals(title, Entity.Title, StringComparison.Ordinal))
        {
            throw new InvalidOperationException($"You cannot set the same name: {title}.");
        }
        
        RaiseEvent(new EpicUpdatedEvent
        (
            Id: Entity.Id,
            Title: title 
        ));
    }
    
    protected void Apply(EpicUpdatedEvent @event)
    {
        base.Apply(@event);
    }
    
    public void DeleteEpic()
    {
        if (!IsActive)
        {
            throw new InvalidOperationException("You cannot delete already deleted epic!");
        }
        
        RaiseEvent(new EpicDeletedEvent(Entity.Id));
    }
    
    protected void Apply(EpicDeletedEvent @event)
    {
        Entity.Id = @event.Id;
        Id = @event.Id;
        IsActive = false;
    }
}