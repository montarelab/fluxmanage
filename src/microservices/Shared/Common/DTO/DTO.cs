using Common.Domain.Entities;

namespace Common.DTO;

public record TicketUpdateData
{
    public Guid Id { get; init; }
    public string? Title { get; init; }
    public string? Description { get; init; }
    public DateTime? StartDate { get; init; }
    public DateTime? DueDate { get; init; }
    public Guid? AssigneeId { get; init; }
    public Guid? ParentTicketId { get; init; }
    public Guid? EpicId { get; init; }
    public int? EstimatedStoryPoints { get; init; }
    public TicketStatus? Status { get; init; }
    public IDictionary<string, string>? CustomFields { get; init; }
}