using Common.Events.Models;

namespace Common.Domain.Models;

public class EpicAggregate : AggregateRoot
{
    private string Name { get; set; } = string.Empty;
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
    
    public void Apply(EpicCreatedEvent @event)
    {
        IsActive = true;
        Id = @event.Id;
        Name = @event.Name;
        CreatedBy = @event.CreatedBy;
        ProjectId = @event.ProjectId;
        CreatedDate = @event.CreatedDate;
    }
    
    public void EditName(string name)
    {
        if (!IsActive)
        {
            throw new InvalidOperationException("You cannot edit deleted epic!");
        }
        
        if(string.IsNullOrWhiteSpace(name))
        {
            throw new InvalidOperationException("Epic name cannot be empty.");
        }

        if (string.Equals(name, Name, StringComparison.Ordinal))
        {
            throw new InvalidOperationException($"You cannot set the same name: {name}.");
        }
        
        RaiseEvent(new EpicUpdatedEvent
        (
            Id: Id,
            FieldsChanged: new Dictionary<string, object> {{nameof(Name), name}} 
        ));
    }
    
    public void Apply(EpicUpdatedEvent @event)
    {
        base.Apply(@event);
    }
    
    public void DeleteEpic()
    {
        RaiseEvent(new EpicDeletedEvent(Id));
    }
    
    public void Apply(EpicDeletedEvent @event)
    {
        Id = @event.Id;
        IsActive = false;
    }
}