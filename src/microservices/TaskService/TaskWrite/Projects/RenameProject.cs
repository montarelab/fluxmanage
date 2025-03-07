using Common.Domain.Aggregates;
using Common.EventSourcing;
using FastEndpoints;
using FluentValidation;

namespace TaskWrite.Projects;

public static class RenameProject
{
    public record RenameProjectRequest(Guid Id, string NewTitle);
    public record RenameProjectResponse(Guid Id);
    
    public class Validator : Validator<RenameProjectRequest>
    {
        public Validator(IEventSourcingHandler<ProjectAggregate> eventSourcingHandler)
        {
            RuleFor(x => x.Id)
                .MustAsync(async (id, _) => await eventSourcingHandler.GetAggregateByIdAsync(id) != null)
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
        public IEventSourcingHandler<ProjectAggregate> EventSourcingHandler { get; set; } = null!;
        
        public override void Configure()
        {
            Put("/projects");
            AllowAnonymous();
            // todo introduce permissions
        }

        public override async Task HandleAsync(RenameProjectRequest req, CancellationToken ct)
        {
            var project = (await EventSourcingHandler.GetAggregateByIdAsync(req.Id))!;
            project.EditName(req.NewTitle);
            await EventSourcingHandler.SaveAggregateAsync(project);
            await SendOkAsync(new RenameProjectResponse(project.Entity.Id), ct);
        }
    }
}