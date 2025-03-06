using Common.Events;

namespace Common.Domain;

public interface IAggregateRoot : IEntity
{
    bool IsActive { get; set; }
    int Version { get; set; }
    IEnumerable<DomainEvent> GetUncommittedChanges();
    void MarkChangesAsCommitted();
    void ReplayEvents(IEnumerable<DomainEvent> events);
}