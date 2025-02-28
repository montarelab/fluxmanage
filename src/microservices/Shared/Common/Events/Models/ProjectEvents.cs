using Common.Domain.Models;

namespace Common.Events.Models;

public record ProjectCreatedEvent(Guid Id, string Name, Guid CreatedBy) : EntityCreatedEvent<ProjectAggregate>(Id);

public record ProjectUpdatedEvent(Guid Id, IDictionary<string, object> FieldsChanged) 
    : EntityUpdatedEvent<ProjectAggregate>(Id, FieldsChanged);

public record ProjectDeletedEvent(Guid Id) : EntityDeletedEvent<ProjectAggregate>(Id);



