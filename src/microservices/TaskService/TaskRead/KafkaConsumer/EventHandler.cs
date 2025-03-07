using Common.Domain.Entities;
using Common.Events.Models;
using FastEndpoints;
using TaskRead.Services;

namespace TaskRead.KafkaConsumer;

public class EventHandler(
    IRepository<Ticket> taskRepository,
    IRepository<Epic> epicRepository,
    IRepository<Project> projectRepository,
    ILogger<EventHandler> logger
    )
    : IEventHandler<TicketCreatedEvent>,
        IEventHandler<TicketUpdatedEvent>,
        IEventHandler<TicketDeletedEvent>,
        IEventHandler<TicketCompletedEvent>,
        IEventHandler<TicketAssignedEvent>,
        IEventHandler<ProjectCreatedEvent>,
        IEventHandler<ProjectUpdatedEvent>,
        IEventHandler<ProjectDeletedEvent>,
        IEventHandler<EpicCreatedEvent>,
        IEventHandler<EpicUpdatedEvent>,
        IEventHandler<EpicDeletedEvent>
{
    public Task HandleAsync(TicketCreatedEvent eventModel, CancellationToken ct)
    {
        logger.LogInformation("Handling TicketCreatedEvent");
        var task = new Ticket
        {
            Id = eventModel.Id,
            Title = eventModel.Title,
            ProjectId = eventModel.ProjectId,
            CreatedBy = eventModel.CreatedBy,
            CreatedDate = eventModel.TriggeredOn
        };
        return taskRepository.AddAsync(task, ct);
    }

    public Task HandleAsync(TicketUpdatedEvent eventModel, CancellationToken ct)
    {
        throw new NotImplementedException();
    }

    public Task HandleAsync(TicketDeletedEvent eventModel, CancellationToken ct)
    {
        throw new NotImplementedException();
    }

    public Task HandleAsync(TicketCompletedEvent eventModel, CancellationToken ct)
    {
        throw new NotImplementedException();
    }

    public Task HandleAsync(TicketAssignedEvent eventModel, CancellationToken ct)
    {
        throw new NotImplementedException();
    }

    public Task HandleAsync(ProjectCreatedEvent eventModel, CancellationToken ct)
    {
        throw new NotImplementedException();
    }

    public Task HandleAsync(ProjectUpdatedEvent eventModel, CancellationToken ct)
    {
        throw new NotImplementedException();
    }

    public Task HandleAsync(ProjectDeletedEvent eventModel, CancellationToken ct)
    {
        throw new NotImplementedException();
    }

    public Task HandleAsync(EpicCreatedEvent eventModel, CancellationToken ct)
    {
        throw new NotImplementedException();
    }

    public Task HandleAsync(EpicUpdatedEvent eventModel, CancellationToken ct)
    {
        throw new NotImplementedException();
    }

    public Task HandleAsync(EpicDeletedEvent eventModel, CancellationToken ct)
    {
        throw new NotImplementedException();
    }
}