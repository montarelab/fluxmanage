using Common.Domain.Models;
using Common.EventSourcing;
using FastEndpoints;
using FluentValidation;
using TaskCommon.Domain;

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
            var task = (await EventSourcingHandler.GetByIdAsync(id))!;
            task.DeleteTask();
            await EventSourcingHandler.SaveAsync(task);
            await SendOkAsync(new DeleteTaskResponse(task.Id), ct);
        }
    }
}