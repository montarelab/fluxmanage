using Common.Events.Models;
using FastEndpoints;

namespace TaskRead.KafkaConsumer;

public interface IUniversalEventHandler 
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
    IEventHandler<EpicDeletedEvent>;