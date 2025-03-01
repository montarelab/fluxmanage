using Common.Events.Models;

namespace Common.Domain.Models;

public class ProjectAggregate : AggregateRoot
{
    private string Title { get; set; } = string.Empty;
    private Guid CreatedBy { get; set; } = Guid.Empty;
    private DateTime CreatedDate { get; set; } = DateTime.Now;
    
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
        Id = @event.Id;
        Title = @event.Name;
        CreatedBy = @event.CreatedBy;
        CreatedDate = @event.TriggeredOn;
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

        if (string.Equals(title, Title, StringComparison.Ordinal))
        {
            throw new InvalidOperationException($"You cannot set the same name: {title}.");
        }
        
        RaiseEvent(new ProjectUpdatedEvent
        (
            Id: Id,
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
        
        RaiseEvent(new ProjectDeletedEvent(Id));
    }
    
    protected void Apply(ProjectDeletedEvent @event)
    {
        Id = @event.Id;
        IsActive = false;
    }
}