using Common.Auth;
using Common.EventSourcing;
using FastEndpoints;
using TaskCommon.Domain;

namespace TaskWrite.Tasks;

public static class CreateTask
{
    public record CreateTaskRequest(Guid ProjectId, string Title);
    public record CreateTaskResponse(Guid Id);
    
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
            // todo validation
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