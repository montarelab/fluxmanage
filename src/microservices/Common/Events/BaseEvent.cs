namespace Common.Events;

public abstract class BaseEvent(Guid id)
{
    public Guid Id { get; set; } = id;
}
