using Common.Events.Models;

namespace Common.Domain.Models;

public class EpicAggregate : AggregateRoot
{
    private string Title { get; set; } = string.Empty;
    private Guid CreatedBy { get; set; } = Guid.Empty;
    private Guid ProjectId { get; set; }
    private DateTime CreatedDate { get; set; } = DateTime.Now;
    
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
        Id = @event.Id;
        Title = @event.Name;
        CreatedBy = @event.CreatedBy;
        ProjectId = @event.ProjectId;
        CreatedDate = @event.TriggeredOn;
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

        if (string.Equals(title, Title, StringComparison.Ordinal))
        {
            throw new InvalidOperationException($"You cannot set the same name: {title}.");
        }
        
        RaiseEvent(new EpicUpdatedEvent
        (
            Id: Id,
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
        
        RaiseEvent(new EpicDeletedEvent(Id));
    }
    
    protected void Apply(EpicDeletedEvent @event)
    {
        Id = @event.Id;
        IsActive = false;
    }
}