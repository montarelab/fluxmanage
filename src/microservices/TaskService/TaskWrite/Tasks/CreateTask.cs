using Common.Auth;
using Common.EventSourcing;
using FastEndpoints;
using FluentValidation;
using TaskCommon.Domain;

namespace TaskWrite.Tasks;

public static class CreateTask
{
    public record CreateTaskRequest(Guid ProjectId, string Title);
    public record CreateTaskResponse(Guid Id);
    
    public class Validator : Validator<CreateTaskRequest>
    {
        public Validator(IEventSourcingHandler<ProjectAggregate> eventSourcingHandler)
        {
            RuleFor(x => x.Title)
                .NotEmpty()
                .WithMessage("Title is required")
                .MaximumLength(20)
                .WithMessage("Title must be less than 20 characters");
            
            RuleFor(x => x.ProjectId)
                .MustAsync(async (id, _) => await eventSourcingHandler.GetByIdAsync(id) != null)
                .WithMessage((_, id) => $"Project by id {id} not found");
        }
    }
    
    public class Endpoint : Endpoint<CreateTaskRequest, CreateTaskResponse>
    {
        private IEventSourcingHandler<TaskAggregate> EventSourcingHandler { get; set; } = null!;
        private ICurrentUserService CurrentUserService { get; set; } = null!;
        
        public override void Configure()
        {
            Post("/tasks");
            // todo introduce permissions
        }

        public override async Task HandleAsync(CreateTaskRequest req, CancellationToken ct)
        {
            var task = new TaskAggregate(
                id: Guid.NewGuid(),
                projectId: req.ProjectId,
                title: req.Title,
                createdBy: CurrentUserService.GetUserId());
            
            await EventSourcingHandler.SaveAsync(task);
            await SendAsync(new CreateTaskResponse(task.Id), cancellation: ct);
        }
    }
}