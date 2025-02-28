using Common.Events.Project;

namespace Common.Domain.Models;

public class ProjectAggregate : AggregateRoot
{
    private string Name { get; set; } = string.Empty;
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
    
    public void Apply(ProjectCreatedEvent @event)
    {
        IsActive = true;
        Id = @event.Id;
        Name = @event.Name;
        CreatedBy = @event.CreatedBy;
        CreatedDate = @event.CreatedDate;
    }
    
    public void EditName(string name)
    {
        if (!IsActive)
        {
            throw new InvalidOperationException("You cannot edit deleted project!");
        }
        
        if(string.IsNullOrWhiteSpace(name))
        {
            throw new InvalidOperationException("Project name cannot be empty.");
        }

        if (string.Equals(name, Name, StringComparison.Ordinal))
        {
            throw new InvalidOperationException($"You cannot set the same name: {name}.");
        }
        
        RaiseEvent(new ProjectUpdatedEvent
        (
            Id: Id,
            FieldsChanged: new Dictionary<string, object> {{nameof(Name), name}} 
        ));
    }
    
    public void Apply(ProjectUpdatedEvent @event)
    {
        base.Apply(@event);
    }
    
    public void DeleteProject()
    {
        RaiseEvent(new ProjectDeletedEvent(Id));
    }
    
    public void Apply(ProjectDeletedEvent @event)
    {
        Id = @event.Id;
        IsActive = false;
    }
}