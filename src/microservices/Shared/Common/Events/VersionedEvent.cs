namespace Common.Events;

public class VersionedEvent(DomainEvent Event, int Version) : DomainEvent
{ 
    public Guid Id { get; set; } = Guid.NewGuid();
    public DateTime CreatedDate { get; set; } = DateTime.Now;
}