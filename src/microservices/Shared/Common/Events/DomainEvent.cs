using Common.Domain;

namespace Common.Events;

public abstract record DomainEvent(Guid Id) 
{
    public DateTime TriggeredOn { get; } = DateTime.Now;
    public int Version { get; init; }
    public abstract string AggregateType { get; }
    public abstract string EventType { get; }
}

public abstract record DomainEvent<TAggregateType>(Guid Id) : DomainEvent(Id)
    where TAggregateType : IEntity
{
    public override string AggregateType => typeof(TAggregateType).Name;
    public override string EventType => GetType().Name;
}
