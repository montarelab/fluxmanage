using TaskStatus = Common.Domain.Entities.TaskStatus;

namespace Common.DTO;

public record TaskUpdateData
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