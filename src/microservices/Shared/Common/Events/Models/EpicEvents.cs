using Common.Domain.Aggregates;
using Common.Domain.Entities;
using MongoDB.Bson;

namespace Common.Events.Models;

public record EpicCreatedEvent(Guid Id, Guid ProjectId, string Name, Guid CreatedBy) 
    : EntityCreatedEvent<Epic>(Id);

public record EpicUpdatedEvent(Guid Id, string Title) : EntityUpdatedEvent<Epic>(Id);

public record EpicDeletedEvent(Guid Id) : EntityDeletedEvent<Epic>(Id);

