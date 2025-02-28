using Common.Domain.Models;

namespace Common.Events.Models;

public record EpicCreatedEvent(Guid Id, Guid ProjectId, string Name, Guid CreatedBy) 
    : EntityCreatedEvent<EpicAggregate>(Id);

public record EpicUpdatedEvent(Guid Id, IDictionary<string, object> FieldsChanged) 
    : EntityUpdatedEvent<EpicAggregate>(Id, FieldsChanged);

public record EpicDeletedEvent(Guid Id) : EntityDeletedEvent<EpicAggregate>(Id);

