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
    logger.LogInformation("Handling TicketUpdatedEvent");
    var task = new Ticket
    {
        Id = eventModel.Id,
        Title = eventModel.Title ?? string.Empty,
        Description = eventModel.Description ?? string.Empty,
        StartDate = eventModel.StartDate ?? DateTime.Now,
        DueDate = eventModel.DueDate ?? DateTime.Now.AddDays(7),
        AssigneeId = eventModel.AssigneeId ?? Guid.Empty,
        ParentTicketId = eventModel.ParentTicketId,
        EpicId = eventModel.EpicId,
        EstimatedStoryPoints = eventModel.EstimatedStoryPoints ?? 0,
        Status = eventModel.Status ?? TicketStatus.Created,
        CustomFields = eventModel.CustomFields,
        ProjectId = Guid.Empty // Set appropriate ProjectId
    };
    return taskRepository.UpdateAsync(task, ct);
}

public Task HandleAsync(TicketCompletedEvent eventModel, CancellationToken ct)
{
    logger.LogInformation("Handling TicketCompletedEvent");
    var task = new Ticket
    {
        Id = eventModel.Id,
        Status = TicketStatus.Completed
    };
    return taskRepository.UpdateAsync(task, ct);
}

public Task HandleAsync(TicketAssignedEvent eventModel, CancellationToken ct)
{
    logger.LogInformation("Handling TicketAssignedEvent");
    var task = new Ticket
    {
        Id = eventModel.Id,
        AssigneeId = eventModel.AssigneeId
    };
    return taskRepository.UpdateAsync(task, ct);
}

public Task HandleAsync(ProjectUpdatedEvent eventModel, CancellationToken ct)
{
    logger.LogInformation("Handling ProjectUpdatedEvent");
    var project = new Project
    {
        Id = eventModel.Id,
        Title = eventModel.Title ?? string.Empty
    };
    return projectRepository.UpdateAsync(project, ct);
}

public Task HandleAsync(EpicUpdatedEvent eventModel, CancellationToken ct)
{
    logger.LogInformation("Handling EpicUpdatedEvent");
    var epic = new Epic
    {
        Id = eventModel.Id,
        Title = eventModel.Title ?? string.Empty
    };
    return epicRepository.UpdateAsync(epic, ct);
}

    public Task HandleAsync(TicketDeletedEvent eventModel, CancellationToken ct)
    {
        logger.LogInformation("Handling TicketDeletedEvent");
        return taskRepository.DeleteAsync(eventModel.Id, ct);
    }
    
    public Task HandleAsync(ProjectCreatedEvent eventModel, CancellationToken ct)
    {
        logger.LogInformation("Handling ProjectCreatedEvent");
        var project = new Project
        {
            Id = eventModel.Id,
            Title = eventModel.Name,
            CreatedBy = eventModel.CreatedBy,
            CreatedDate = eventModel.TriggeredOn
        };
        return projectRepository.AddAsync(project, ct);
    }
    
    public Task HandleAsync(ProjectDeletedEvent eventModel, CancellationToken ct)
    {
        logger.LogInformation("Handling ProjectDeletedEvent");
        return projectRepository.DeleteAsync(eventModel.Id, ct);
    }

    public Task HandleAsync(EpicCreatedEvent eventModel, CancellationToken ct)
    {
        logger.LogInformation("Handling EpicCreatedEvent");
        var epic = new Epic
        {
            Id = eventModel.Id,
            ProjectId = eventModel.ProjectId,
            Title = eventModel.Name,
            CreatedBy = eventModel.CreatedBy,
            CreatedDate = eventModel.TriggeredOn
        };
        return epicRepository.AddAsync(epic, ct);
    }
    
    public Task HandleAsync(EpicDeletedEvent eventModel, CancellationToken ct)
    {
        logger.LogInformation("Handling EpicDeletedEvent");
        return epicRepository.DeleteAsync(eventModel.Id, ct);
    }
}