using Common.Domain.Aggregates;
using Common.EventSourcing;
using FastEndpoints;

namespace TaskWrite.Epics;

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
            var epic = (await EventSourcingHandler.GetAggregateByIdAsync(id))!;
            epic.DeleteEpic();
            await EventSourcingHandler.SaveAggregateAsync(epic);
            await SendOkAsync(new DeleteEpicResponse(epic.Entity.Id), ct);
        }
    }
}