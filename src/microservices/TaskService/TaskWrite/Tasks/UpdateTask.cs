using Common.EventSourcing;
using FastEndpoints;
using TaskCommon.Domain;

namespace TaskWrite.Tasks;

public static class UpdateTask
{
    public record UpdateTaskRequest(Guid Id, string NewTitle);
    public class UpdateTaskResponse(Guid Id);
    
    public class Endpoint : Endpoint<UpdateTaskRequest, UpdateTaskResponse>
    {
        private IEventSourcingHandler<TaskAggregate> EventSourcingHandler { get; set; } = null!;
        
        public override void Configure()
        {
            Put("/tasks");
            // todo introduce permissions
        }

        public override async Task HandleAsync(UpdateTaskRequest req, CancellationToken ct)
        {
            var task = await EventSourcingHandler.GetByIdAsync(req.Id);
            task.Update(req.NewTitle); // todo : improve the logic, make it more flexible
            await EventSourcingHandler.SaveAsync(task);
            await SendAsync(new UpdateTaskResponse(task.Id), cancellation: ct);
        }
    }
}