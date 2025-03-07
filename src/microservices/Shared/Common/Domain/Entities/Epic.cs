namespace Common.Domain.Entities;

public class Epic : Entity
{
    public string Title { get; set; } = string.Empty;
    public Guid ProjectId { get; set; }
}