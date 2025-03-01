using Common.Domain.Models;
using Common.EventSourcing;
using FastEndpoints;
using FluentValidation;
using TaskCommon.Domain;

namespace TaskWrite.Projects;

public static class DeleteProject
{
    public record DeleteProjectRequest(Guid Id);
    public record DeleteProjectResponse(Guid Id);
    
    public class Validator : Validator<DeleteProjectRequest>
    {
        public Validator(IEventSourcingHandler<ProjectAggregate> eventSourcingHandler)
        {
            RuleFor(x => x.Id)
                .MustAsync(async (id, _) => await eventSourcingHandler.GetByIdAsync(id) != null)
                .WithMessage((_, id) => $"Project by id {id} not found");
        }
    }
    
    public class Endpoint : Endpoint<DeleteProjectRequest, DeleteProjectResponse>
    {
        public IEventSourcingHandler<ProjectAggregate> EventSourcingHandler { get; set; } = null!;
        
        public override void Configure()
        {
            Delete("/projects");
            AllowAnonymous();
        }

        public override async Task HandleAsync(DeleteProjectRequest req, CancellationToken ct)
        {
            var project = (await EventSourcingHandler.GetByIdAsync(req.Id))!;
            project.DeleteProject();
            await EventSourcingHandler.SaveAsync(project);
            await SendOkAsync(new DeleteProjectResponse(project.Id), ct);
        }
    }
}