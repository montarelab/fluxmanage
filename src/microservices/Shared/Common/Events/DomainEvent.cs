using Common.Domain;

namespace Common.Events;

public abstract record DomainEvent(Guid Id) 
{
    public DateTime CreatedDate { get; init; } = DateTime.Now;
    public int Version { get; init; } = -1;
    public abstract string AggregateType { get; }
}

public abstract record DomainEvent<TAggregateType>(Guid Id) : DomainEvent(Id)
    where TAggregateType : IEntity
{
    public override string AggregateType => typeof(TAggregateType).Name;
}
