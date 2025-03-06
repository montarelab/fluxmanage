using Common.Auth;
using Common.Domain.Aggregates;
using Common.EventSourcing;
using FastEndpoints;
using FluentValidation;
using Task = System.Threading.Tasks.Task;

namespace TaskWrite.Projects;

public static class CreateProject
{
    public record CreateProjectRequest(string Title);
    public record CreateProjectResponse(Guid Id);

    public class Validator : Validator<CreateProjectRequest>
    {
        public Validator()
        {
            RuleFor(x => x.Title)
                .NotEmpty()
                .WithMessage("Title is required")
                .MaximumLength(20)
                .WithMessage("Title must be less than 20 characters");
        }
    }
    
    public class Endpoint : Endpoint<CreateProjectRequest, CreateProjectResponse>
    {
        public IEventSourcingHandler<ProjectAggregate> EventSourcingHandler { get; set; } = null!;
        public ICurrentUserService CurrentUserService { get; set; } = null!;
        
        public override void Configure()
        {
            Post("/projects");
            AllowAnonymous();
            // todo return permissions after
            // Permissions(Auth.Permissions.GetPermission(Resources.Project, Actions.Create));
        }

        public override async Task HandleAsync(CreateProjectRequest req, CancellationToken ct)
        {
            var project = new ProjectAggregate(
                id: Guid.NewGuid(),
                name: req.Title,
                createdBy: CurrentUserService.GetUserId());

            await EventSourcingHandler.SaveAggregateAsync(project);
            await SendOkAsync(new CreateProjectResponse(project.Entity.Id), ct);
        }
    }
}