namespace Common.Domain.Entities;

public class Project : Entity
{
    public string Title { get; set; } = string.Empty;
    public Guid CreatedBy { get; set; } = Guid.Empty;
    public DateTime CreatedDate { get; set; } = DateTime.Now;
}