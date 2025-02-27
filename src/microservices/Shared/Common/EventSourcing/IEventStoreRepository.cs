using Common.Events;

namespace Common.EventSourcing;

public interface IEventStoreRepository
{
    Task SaveAsync(EventModel @event);

    Task<List<EventModel>> FindByAggregateId(Guid aggregateId);
	
    Task<List<EventModel>> FindAllAsync();
}
