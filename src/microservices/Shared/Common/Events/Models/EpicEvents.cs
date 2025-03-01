using Common.Domain.Models;
using MongoDB.Bson;

namespace Common.Events.Models;

public record EpicCreatedEvent(Guid Id, Guid ProjectId, string Name, Guid CreatedBy) 
    : EntityCreatedEvent<EpicAggregate>(Id);

public record EpicUpdatedEvent(Guid Id, string Title) : EntityUpdatedEvent<EpicAggregate>(Id);

public record EpicDeletedEvent(Guid Id) : EntityDeletedEvent<EpicAggregate>(Id);

