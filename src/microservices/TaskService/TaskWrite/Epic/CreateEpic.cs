using Common.Auth;
using Common.Domain.Aggregates;
using Common.EventSourcing;
using FastEndpoints;
using FluentValidation;
using Task = System.Threading.Tasks.Task;

namespace TaskWrite.Epic;

public static class CreateEpic
{
    public record CreateEpicRequest(Guid ProjectId, string Title);
    public record CreateEpicResponse(Guid Id);

    public class Validator : Validator<CreateEpicRequest>
    {
        public Validator(IEventSourcingHandler<ProjectAggregate> eventSourcingHandler)
        {
            RuleFor(x => x.ProjectId)
                .MustAsync(async (id, _) => await eventSourcingHandler.GetAggregateByIdAsync(id) != null)
                .WithMessage((_, id) => $"Project by id {id} not found");
            
            RuleFor(x => x.Title)
                .NotEmpty()
                .WithMessage("Title is required")
                .MaximumLength(20)
                .WithMessage("Title must be less than 20 characters");
        }
    }
    
    public class Endpoint : Endpoint<CreateEpicRequest, CreateEpicResponse>
    {
        public IEventSourcingHandler<EpicAggregate> EventSourcingHandler { get; set; } = null!;
        public ICurrentUserService CurrentUserService { get; set; } = null!;
        
        public override void Configure()
        {
            Post("/epics");
            AllowAnonymous();
            // todo return permissions after
            // Permissions(Auth.Permissions.GetPermission(Resources.Project, Actions.Create));
        }

        public override async Task HandleAsync(CreateEpicRequest req, CancellationToken ct)
        {
            var epic = new EpicAggregate(
                id: Guid.NewGuid(),
                projectId: req.ProjectId,
                name: req.Title,
                createdBy: CurrentUserService.GetUserId());
            
            await EventSourcingHandler.SaveAggregateAsync(epic);
            await SendOkAsync(new CreateEpicResponse(epic.Entity.Id), ct);
        }
    }
}