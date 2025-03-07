using Common.Domain.Entities;
using Common.DTO;
using Common.Events.Models;

namespace Common.Domain.Aggregates;

public class TicketAggregate : AggregateRoot<Ticket>
{
    public TicketAggregate() { }

    public TicketAggregate(Guid id, Guid projectId, string title, Guid createdBy)
    {
        RaiseEvent(new TicketCreatedEvent
        (
            Id: id,
            ProjectId: projectId,
            Title: title,
            CreatedBy: createdBy
        ));
    }
    
    protected void Apply(TicketCreatedEvent @event)
    {
        IsActive = true;
        Entity.Id = @event.Id;
        Id = @event.Id;
        Entity.Title = @event.Title;
        Entity.ProjectId = @event.ProjectId;
        Entity.CreatedBy = @event.CreatedBy;
        Entity.CreatedDate = @event.TriggeredOn;
    }

    public void Update(TicketUpdateData updatedData)
    {
        if (!IsActive)
        {
            throw new InvalidOperationException("You cannot edit deleted Ticket!");
        }
        
        RaiseEvent(new TicketUpdatedEvent(
            updatedData.Id,
            updatedData.Title,
            updatedData.Description,
            updatedData.StartDate,
            updatedData.DueDate,
            updatedData.AssigneeId,
            updatedData.ParentTicketId,
            updatedData.EpicId,
            updatedData.EstimatedStoryPoints,
            updatedData.Status,
            updatedData.CustomFields));
    }
    
    protected void Apply(TicketUpdatedEvent @event)
    {
        base.Apply(@event);
    }
    
    public void DeleteTicket()
    {
        if (!IsActive)
        {
            throw new InvalidOperationException("You cannot delete already deleted Ticket!");
        }
        
        RaiseEvent(new TicketDeletedEvent(Entity.Id));
    }
    
    protected void Apply(TicketDeletedEvent @event)
    {
        Entity.Id = @event.Id;
        IsActive = false;
    }
    
    public void CompleteTicket()
    {
        RaiseEvent(new TicketCompletedEvent(Entity.Id));
    }
    
    protected void Apply(TicketCompletedEvent @event)
    {
        Entity.Id = @event.Id;
        Entity.Status = TicketStatus.Completed;
    }
    
    public void AssignTicket(Guid assigneeId)
    {
        RaiseEvent(new TicketAssignedEvent(Entity.Id, assigneeId));
    }
    
    protected void Apply(TicketAssignedEvent @event)
    {
        Entity.Id = @event.Id;
        Id = @event.Id;
        Entity.AssigneeId = @event.AssigneeId;
    }
}