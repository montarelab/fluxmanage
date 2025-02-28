namespace Common.DTO;

public abstract record UpdateData 
{
    public IDictionary<string, object> Changes => GetType()
            .GetProperties()
            .Select(p => new { Field = p.Name, Value = p.GetValue(this) })
            .Where(p => p.Value != null)
            .ToDictionary(p => p.Field, p => p.Value!);        
}

public record ProjectDto();
public record TaskDto();
public record TaskUpdateData : UpdateData
{
    public Guid Id { get; init; }
    public string? Title { get; init; }
    public string? Description { get; init; }
    public DateTime? StartDate { get; init; }
    public DateTime? DueDate { get; init; }
    public Guid? AssigneeId { get; init; }
    public Guid? ParentTaskId { get; init; }
    public Guid? EpicId { get; init; }
    public int? EstimatedStoryPoints { get; init; }
    public TaskStatus? Status { get; init; }
    public IDictionary<string, string>? CustomFields { get; init; }
}