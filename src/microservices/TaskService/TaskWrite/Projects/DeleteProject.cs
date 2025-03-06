using Common.Domain.Aggregates;
using Common.EventSourcing;
using FastEndpoints;
using Task = System.Threading.Tasks.Task;

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
            var project = (await EventSourcingHandler.GetAggregateByIdAsync(id))!;
            project.DeleteProject();
            await EventSourcingHandler.SaveAggregateAsync(project);
            await SendOkAsync(new DeleteProjectResponse(project.Entity.Id), ct);
        }
    }
}