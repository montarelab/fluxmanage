namespace Common.Events;

public abstract record DomainEvent(Guid Id)
{
    public DateTime CreatedDate { get; init; } = DateTime.Now;
    public int Version { get; init; } = -1;
}
