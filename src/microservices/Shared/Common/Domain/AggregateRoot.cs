using System.Reflection;
using Common.Events;

namespace Common.Domain;

public abstract class AggregateRoot<TEntity> : IAggregateRoot where TEntity : Entity, new()
{
    private readonly List<DomainEvent> _changes = []; // uncommitted changes
    public TEntity Entity { get; } = new();
    public Guid Id { get; set; }

    public bool IsActive { get; set; }
    public int Version { get; set; } = -1; // 0 is the first version
    /// <summary>
    ///     This method is being running only when we save changes to the event store
    ///     These changes we store during RaiseEvent, i.e. during every RaiseEvent and ReplayEvents
    ///     Right after execution we clear _changes
    /// </summary>
    /// <returns></returns>
    public IEnumerable<DomainEvent> GetUncommittedChanges()
    {
        return _changes;
    }

    public void MarkChangesAsCommitted()
    {
        _changes.Clear();
    }

    protected void Apply(EntityUpdatedEvent<TEntity> @event)
    {
        Id = @event.Id;
        foreach (var eventProperty in @event.GetType().GetProperties(BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic))
        {
            var entityProperty = Entity.GetType().GetProperty(eventProperty.Name, 
                BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic);

            if (entityProperty == null || !entityProperty.CanWrite) continue;
            
            var value = eventProperty.GetValue(@event);

            object? convertedValue = null;
            if(entityProperty.PropertyType == eventProperty.PropertyType)
            {
                convertedValue = value;
            }
            else
            {
                convertedValue = value != null 
                    ? Convert.ChangeType(value, entityProperty.PropertyType) 
                    : null;
            }

            entityProperty.SetValue(Entity, convertedValue);
        }
    }

    /// <summary>
    ///     Applies incoming change.
    ///     Runs Apply (of a concrete aggregate) method with an input event
    ///     If the event is new => add to uncommitted changes
    /// </summary>
    /// <param name="event"></param>
    /// <param name="isNew"></param>
    /// <exception cref="ArgumentNullException"></exception>
    protected void ApplyChange(DomainEvent @event, bool isNew)
    {
        var method = GetType().GetMethod(
            nameof(Apply), 
            BindingFlags.Instance | BindingFlags.NonPublic, 
            null,  
            [@event.GetType()], 
            null);

        if (method is null)
        {
            throw new ArgumentNullException(nameof(method),
                $"The Apply method was not found in the aggregate for {@event.GetType().Name}");
        }

        method.Invoke(this, [@event]);

        if (isNew)
        {
            _changes.Add(@event);
        }
    }

    /// <summary>
    ///     Invokes event
    /// </summary>
    /// <param name="event"></param>
    protected void RaiseEvent(DomainEvent @event)
    {
        ApplyChange(@event, true);
    }

    /// <summary>
    ///     Replays all the events to recreate the latest state of the aggregate
    ///     before new uncommitted changes will be applied
    ///     It is being running only while fetching Aggregate from EventSourcingHandler
    /// </summary>
    /// <param name="events"></param>
    public void ReplayEvents(IEnumerable<DomainEvent> events)
    {
        foreach (var @event in events) ApplyChange(@event, false);
    }


}