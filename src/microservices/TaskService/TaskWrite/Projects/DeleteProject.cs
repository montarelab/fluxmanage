using Common.Domain.Models;
using Common.EventSourcing;
using FastEndpoints;
using FluentValidation;
using TaskCommon.Domain;

namespace TaskWrite.Projects;

public static class DeleteProject
{
    public record DeleteProjectRequest(Guid Id);
    public class DeleteProjectResponse(Guid Id);
    
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
        private IEventSourcingHandler<ProjectAggregate> EventSourcingHandler { get; set; } = null!;
        
        public override void Configure()
        {
            Delete("/projects");
        }

        public override async Task HandleAsync(DeleteProjectRequest req, CancellationToken ct)
        {
            var project = (await EventSourcingHandler.GetByIdAsync(req.Id))!;
            project.DeleteProject();
            await EventSourcingHandler.SaveAsync(project);
            await SendAsync(new DeleteProjectResponse(project.Id), cancellation: ct);
        }
    }
}