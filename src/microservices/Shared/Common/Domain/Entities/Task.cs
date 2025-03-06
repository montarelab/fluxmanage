namespace Common.Domain.Entities;

public class Task : Entity
{
    public string Title { get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty;
    public Guid CreatedBy { get; set; } = Guid.Empty;
    public Guid? EpicId { get; set; } 
    public Guid? ParentTaskId { get; set; }
    public int EstimatedStoryPoints { get; set; } = 0;
    public DateTime CreatedDate { get; set; } = DateTime.Now;
    public DateTime StartDate { get; set; } = DateTime.Now; 
    public DateTime DueDate { get; set; } = DateTime.Now.AddDays(7);
    public IDictionary<string, string>? CustomFields { get; set; }
    public Guid AssigneeId { get; set; }
    public Guid ProjectId { get; set; }
    public TaskStatus Status { get; set; } = TaskStatus.Created;
}