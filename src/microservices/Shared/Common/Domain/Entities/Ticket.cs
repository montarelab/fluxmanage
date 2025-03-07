namespace Common.Domain.Entities;

public class Ticket : Entity
{
    public string Title { get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty;
    public Guid? EpicId { get; set; } 
    public Guid? ParentTicketId { get; set; }
    public int EstimatedStoryPoints { get; set; } = 0;
    public DateTime StartDate { get; set; } = DateTime.Now; 
    public DateTime DueDate { get; set; } = DateTime.Now.AddDays(7);
    public IDictionary<string, string>? CustomFields { get; set; }
    public Guid AssigneeId { get; set; }
    public Guid ProjectId { get; set; }
    public TicketStatus Status { get; set; } = TicketStatus.Created;
}