namespace Common.Events;

public abstract class DomainEvent(Guid id)
{
    public Guid Id { get; set; } = id;
    public DateTime CreatedDate { get; set; } = DateTime.Now;
}
