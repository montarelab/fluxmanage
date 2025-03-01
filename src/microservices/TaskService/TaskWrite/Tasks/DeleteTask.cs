using Common.Domain.Models;
using Common.EventSourcing;
using FastEndpoints;
using FluentValidation;
using TaskCommon.Domain;

namespace TaskWrite.Tasks;

public static class DeleteTask
{
    public record DeleteTaskRequest(Guid Id);
    public record DeleteTaskResponse(Guid Id);
    
    public class Validator : Validator<DeleteTaskRequest>
    {
        public Validator(IEventSourcingHandler<TaskAggregate> eventSourcingHandler)
        {
            RuleFor(x => x.Id)
                .MustAsync(async (id, _) => await eventSourcingHandler.GetByIdAsync(id) != null)
                .WithMessage((_, id) => $"Task by id {id} not found");
        }
    }
    
    public class Endpoint : Endpoint<DeleteTaskRequest, DeleteTaskResponse>
    {
        public IEventSourcingHandler<TaskAggregate> EventSourcingHandler { get; set; } = null!;
        
        public override void Configure()
        {
            Delete("/tasks");
            AllowAnonymous();

            // todo introduce permissions
        }

        public override async Task HandleAsync(DeleteTaskRequest req, CancellationToken ct)
        {
            var task = (await EventSourcingHandler.GetByIdAsync(req.Id))!;
            task.DeleteTask();
            await EventSourcingHandler.SaveAsync(task);
            await SendOkAsync(new DeleteTaskResponse(task.Id), ct);
        }
    }
}