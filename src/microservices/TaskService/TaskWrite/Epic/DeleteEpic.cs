using Common.Domain.Models;
using Common.EventSourcing;
using FastEndpoints;
using FluentValidation;

namespace TaskWrite.Epic;

public static class DeleteEpic
{
    public record DeleteEpicResponse(Guid Id);
    
    public class Endpoint : EndpointWithoutRequest<DeleteEpicResponse>
    {
        public IEventSourcingHandler<EpicAggregate> EventSourcingHandler { get; set; } = null!;
        
        public override void Configure()
        {
            Delete("/epics/{id:guid}");
            AllowAnonymous();
        }

        public override async Task HandleAsync(CancellationToken ct)
        {
            var id = Route<Guid>("id");
            var epic = (await EventSourcingHandler.GetByIdAsync(id))!;
            epic.DeleteEpic();
            await EventSourcingHandler.SaveAsync(epic);
            await SendOkAsync(new DeleteEpicResponse(epic.Id), ct);
        }
    }
}