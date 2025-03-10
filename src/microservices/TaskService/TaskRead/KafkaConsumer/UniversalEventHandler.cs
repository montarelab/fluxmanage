using Common.Domain.Entities;
using Common.Events.Models;
using Common.Exceptions;
using TaskRead.Services;

namespace TaskRead.KafkaConsumer;

public class UniversalEventHandler(
    IRepository<Ticket> taskRepository,
    IRepository<Epic> epicRepository,
    IRepository<Project> projectRepository,
    ILogger<UniversalEventHandler> logger
    )
    : IUniversalEventHandler
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

    public async Task HandleAsync(TicketUpdatedEvent eventModel, CancellationToken ct)
    {
        logger.LogInformation("Handling TicketUpdatedEvent");
        var task = await taskRepository.GetByIdAsync(eventModel.Id, ct)
            ?? throw new AggregateNotFoundException("Incorrect ticket aggregate Id provided! " + eventModel.Id);
    
        task.Id = eventModel.Id;
        task.Title = eventModel.Title ?? task.Title;
        task.Description = eventModel.Description ?? task.Description;
        task.StartDate = eventModel.StartDate ?? task.StartDate;
        task.DueDate = eventModel.DueDate ?? task.DueDate;
        task.AssigneeId = eventModel.AssigneeId ?? task.AssigneeId;
        task.ParentTicketId = eventModel.ParentTicketId ?? task.ParentTicketId;
        task.EpicId = eventModel.EpicId ?? task.EpicId;
        task.EstimatedStoryPoints = eventModel.EstimatedStoryPoints ?? task.EstimatedStoryPoints;
        task.Status = eventModel.Status ?? task.Status;
        task.CustomFields = eventModel.CustomFields ?? task.CustomFields;
        await taskRepository.UpdateAsync(task, ct);
    }

    public async Task HandleAsync(TicketCompletedEvent eventModel, CancellationToken ct)
    {
        logger.LogInformation("Handling TicketCompletedEvent");
        var task = await taskRepository.GetByIdAsync(eventModel.Id, ct)
                   ?? throw new AggregateNotFoundException("Incorrect ticket aggregate Id provided! " + eventModel.Id);
        task.Status = TicketStatus.Completed;
        await taskRepository.UpdateAsync(task, ct);
    }

    public async Task HandleAsync(TicketAssignedEvent eventModel, CancellationToken ct)
    {
        logger.LogInformation("Handling TicketAssignedEvent");
        var task = await taskRepository.GetByIdAsync(eventModel.Id, ct)
                   ?? throw new AggregateNotFoundException("Incorrect ticket aggregate Id provided! " + eventModel.Id);
    
        task.AssigneeId = eventModel.AssigneeId;
        await taskRepository.UpdateAsync(task, ct);
    }

    public async Task HandleAsync(ProjectUpdatedEvent eventModel, CancellationToken ct)
    {
        logger.LogInformation("Handling ProjectUpdatedEvent");
        var project = await projectRepository.GetByIdAsync(eventModel.Id, ct)
            ?? throw new AggregateNotFoundException("Incorrect project aggregate Id provided! " + eventModel.Id);

        project.Title = eventModel.Title ?? project.Title;
        await projectRepository.UpdateAsync(project, ct);
    }

    public async Task HandleAsync(EpicUpdatedEvent eventModel, CancellationToken ct)
    {
        logger.LogInformation("Handling EpicUpdatedEvent");
        var epic = await epicRepository.GetByIdAsync(eventModel.Id, ct)
            ?? throw new AggregateNotFoundException("Incorrect epic aggregate Id provided! " + eventModel.Id);

        epic.Title = eventModel.Title ?? epic.Title;
        await epicRepository.UpdateAsync(epic, ct);
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