using Common.Domain.Aggregates;
using Common.EventSourcing;
using FastEndpoints;
using Task = System.Threading.Tasks.Task;

namespace TaskWrite.Tasks;

public static class DeleteTask
{
    public record DeleteTaskResponse(Guid Id);

    public class Endpoint : EndpointWithoutRequest<DeleteTaskResponse>
    {
        public IEventSourcingHandler<TaskAggregate> EventSourcingHandler { get; set; } = null!;
        
        public override void Configure()
        {
            Delete("/tasks/{id:guid}");
            AllowAnonymous();

            // todo introduce permissions
        }

        public override async Task HandleAsync(CancellationToken ct)
        {
            var id = Route<Guid>("id");
            var task = (await EventSourcingHandler.GetAggregateByIdAsync(id))!;
            task.DeleteTask();
            await EventSourcingHandler.SaveAggregateAsync(task);
            await SendOkAsync(new DeleteTaskResponse(task.Entity.Id), ct);
        }
    }
}