namespace Common.Domain.Entities;

public class Epic : Entity
{
    public string Title { get; set; } = string.Empty;
    public Guid CreatedBy { get; set; } = Guid.Empty;
    public Guid ProjectId { get; set; }
    public DateTime CreatedDate { get; set; } = DateTime.Now;
}