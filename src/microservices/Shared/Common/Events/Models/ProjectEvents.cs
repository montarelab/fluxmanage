using Common.Domain.Models;
using MongoDB.Bson;

namespace Common.Events.Models;

public record ProjectCreatedEvent(Guid Id, string Name, Guid CreatedBy) : EntityCreatedEvent<ProjectAggregate>(Id);

public record ProjectUpdatedEvent(Guid Id, string Title) : EntityUpdatedEvent<ProjectAggregate>(Id);

public record ProjectDeletedEvent(Guid Id) : EntityDeletedEvent<ProjectAggregate>(Id);



