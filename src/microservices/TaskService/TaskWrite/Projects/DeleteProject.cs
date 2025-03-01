using Common.Domain.Models;
using Common.EventSourcing;
using FastEndpoints;
using FluentValidation;
using TaskCommon.Domain;

namespace TaskWrite.Projects;

public static class DeleteProject
{
    public record DeleteProjectResponse(Guid Id);
    
    public class Endpoint : EndpointWithoutRequest<DeleteProjectResponse>
    {
        public IEventSourcingHandler<ProjectAggregate> EventSourcingHandler { get; set; } = null!;
        
        public override void Configure()
        {
            Delete("/projects/{id:guid}");
            AllowAnonymous();
        }

        public override async Task HandleAsync(CancellationToken ct)
        {
            var id = Route<Guid>("id");
            var project = (await EventSourcingHandler.GetByIdAsync(id))!;
            project.DeleteProject();
            await EventSourcingHandler.SaveAsync(project);
            await SendOkAsync(new DeleteProjectResponse(project.Id), ct);
        }
    }
}