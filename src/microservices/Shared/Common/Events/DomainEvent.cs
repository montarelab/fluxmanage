namespace Common.Events;

public abstract class DomainEvent(Guid id)
{
    public DateTime CreatedDate { get; set; } = DateTime.Now;
}
