using Common.EventSourcing;
using FastEndpoints;
using FluentValidation;
using TaskCommon.Domain;

namespace TaskWrite.Projects;

public static class RenameProject
{
    public record RenameProjectRequest(Guid Id, string NewTitle);
    public class RenameProjectResponse(Guid Id);
    
    public class Validator : Validator<RenameProjectRequest>
    {
        public Validator(IEventSourcingHandler<ProjectAggregate> eventSourcingHandler)
        {
            RuleFor(x => x.Id)
                .MustAsync(async (id, _) => await eventSourcingHandler.GetByIdAsync(id) != null)
                .WithMessage((_, id) => $"Project by id {id} not found");
            
            RuleFor(x => x.NewTitle)
                .NotEmpty()
                .WithMessage("Title is required")
                .MaximumLength(20)
                .WithMessage("Title must be less than 20 characters");
        }
    }
    
    public class Endpoint : Endpoint<RenameProjectRequest, RenameProjectResponse>
    {
        private IEventSourcingHandler<ProjectAggregate> EventSourcingHandler { get; set; } = null!;
        
        public override void Configure()
        {
            Put("/projects");
            // todo introduce permissions
        }

        public override async Task HandleAsync(RenameProjectRequest req, CancellationToken ct)
        {
            var project = await EventSourcingHandler.GetByIdAsync(req.Id);
            project.EditName(req.NewTitle);
            await EventSourcingHandler.SaveAsync(project);
            await SendAsync(new RenameProjectResponse(project.Id), cancellation: ct);
        }
    }
}