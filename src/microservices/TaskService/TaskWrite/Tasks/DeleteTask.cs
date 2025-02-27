using Common.EventSourcing;
using FastEndpoints;
using TaskCommon.Domain;

namespace TaskWrite.Tasks;

public static class DeleteTask
{
    public record DeleteTaskRequest(Guid Id);
    public record DeleteTaskResponse(Guid Id);
    
    public class Endpoint : Endpoint<DeleteTaskRequest, DeleteTaskResponse>
    {
        private IEventSourcingHandler<TaskAggregate> EventSourcingHandler { get; set; } = null!;
        
        public override void Configure()
        {
            Delete("/tasks");
            // todo introduce permissions
        }

        public override async Task HandleAsync(DeleteTaskRequest req, CancellationToken ct)
        {
            var task = await EventSourcingHandler.GetByIdAsync(req.Id);
            task.DeleteTask();
            await EventSourcingHandler.SaveAsync(task);
            await SendAsync(new DeleteTaskResponse(task.Id), cancellation: ct);
        }
    }
}