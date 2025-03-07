using Common.Domain.Entities;
using Common.Events.Models;
using FastEndpoints;
using TaskRead.Services;
using Epic = Common.Domain.Entities.Epic;

namespace TaskRead.KafkaConsumer;

public class EventHandler(
    IRepository<Ticket> taskRepository,
    IRepository<Common.Domain.Entities.Ticket> epicRepository,
    IRepository<Project> projectRepository
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
        throw new NotImplementedException();
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