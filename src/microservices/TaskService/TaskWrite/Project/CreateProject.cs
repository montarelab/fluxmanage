using Common.Auth;
using Common.EventSourcing;
using FastEndpoints;
using TaskCommon.Domain;
using TaskCommon.DTO;

namespace TaskWrite.Project;

public static class CreateProject
{
    public record Request(string Title);
    public class Response(Guid Id);
    
    public class Endpoint : Endpoint<Request, Response>
    {
        private IEventSourcingHandler<ProjectAggregate> EventSourcingHandler { get; set; } = null!;
        private ICurrentUserService CurrentUserService { get; set; } = null!;
        
        public override void Configure()
        {
            Post("/projcts");
            // todo introduce permissions
        }

        public override async Task HandleAsync(Request req, CancellationToken ct)
        {
            var project = new ProjectAggregate(
                id: Guid.NewGuid(),
                name: req.Title,
                userId: CurrentUserService.GetUserId());
            
            await EventSourcingHandler.SaveAsync(project);
            await SendAsync(new Response(project.Id), cancellation: ct);
        }
    }
}