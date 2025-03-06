using Common.Domain.Entities;

namespace Common.Events.Models;

public record ProjectCreatedEvent(Guid Id, string Name, Guid CreatedBy) : EntityCreatedEvent<Project>(Id);

public record ProjectUpdatedEvent(Guid Id, string Title) : EntityUpdatedEvent<Project>(Id);

public record ProjectDeletedEvent(Guid Id) : EntityDeletedEvent<Project>(Id);



