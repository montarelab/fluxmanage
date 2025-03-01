using Common.Domain.Models;
using Common.EventSourcing;
using FastEndpoints;
using FluentValidation;

namespace TaskWrite.Epic;

public static class DeleteEpic
{
    public record DeleteEpicRequest(Guid Id);
    public record DeleteEpicResponse(Guid Id);
    
    public class Validator : Validator<DeleteEpicRequest>
    {
        public Validator(IEventSourcingHandler<EpicAggregate> eventSourcingHandler)
        {
            RuleFor(x => x.Id)
                .MustAsync(async (id, _) => await eventSourcingHandler.GetByIdAsync(id) != null)
                .WithMessage((_, id) => $"Epic by id {id} not found");
        }
    }
    
    public class Endpoint : Endpoint<DeleteEpicRequest, DeleteEpicResponse>
    {
        public IEventSourcingHandler<EpicAggregate> EventSourcingHandler { get; set; } = null!;
        
        public override void Configure()
        {
            Delete("/epics");
            AllowAnonymous();
        }

        public override async Task HandleAsync(DeleteEpicRequest req, CancellationToken ct)
        {
            var epic = (await EventSourcingHandler.GetByIdAsync(req.Id))!;
            epic.DeleteEpic();
            await EventSourcingHandler.SaveAsync(epic);
            await SendOkAsync(new DeleteEpicResponse(epic.Id), ct);
        }
    }
}