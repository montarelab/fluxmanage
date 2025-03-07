using Common.Domain.Aggregates;
using Common.EventSourcing;
using FastEndpoints;
using Task = System.Threading.Tasks.Task;

namespace TaskWrite.Ticket;

public static class DeleteTicket
{
    public record DeleteTicketResponse(Guid Id);

    public class Endpoint : EndpointWithoutRequest<DeleteTicketResponse>
    {
        public IEventSourcingHandler<TicketAggregate> EventSourcingHandler { get; set; } = null!;
        
        public override void Configure()
        {
            Delete("/tickets/{id:guid}");
            AllowAnonymous();

            // todo introduce permissions
        }

        public override async Task HandleAsync(CancellationToken ct)
        {
            var id = Route<Guid>("id");
            var task = (await EventSourcingHandler.GetAggregateByIdAsync(id))!;
            task.DeleteTicket();
            await EventSourcingHandler.SaveAggregateAsync(task);
            await SendOkAsync(new DeleteTicketResponse(task.Entity.Id), ct);
        }
    }
}