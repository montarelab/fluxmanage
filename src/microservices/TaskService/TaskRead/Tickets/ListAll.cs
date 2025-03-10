using Common.Domain.Entities;
using FastEndpoints;
using Mapster;
using TaskRead.Dto;
using TaskRead.Services;

namespace TaskRead.Tickets;

public static class ListAllTickets
{
    public record ListAllTicketQuery(Guid ProjectId) : ICommand<ListAllTicketsResponse>;
    public record ListAllTicketsResponse(IEnumerable<TicketDto> Tickets);

    public class Endpoint : EndpointWithoutRequest<ListAllTicketsResponse>
    {
        public override void Configure()
        {
            Get("/tickets/all/{projectId:guid}");
            AllowAnonymous();
            // todo introduce permissions
        }

        public override async Task HandleAsync(CancellationToken ct)
        {
            var projectId = Route<Guid>("projectId");
            var result = await new ListAllTicketQuery(projectId).ExecuteAsync(ct);
            await SendOkAsync(result, ct);
        }
    }

    public class CommandHandler(
        ILogger<CommandHandler> logger,
        IRepository<Ticket> repo)
        : ICommandHandler<ListAllTicketQuery, ListAllTicketsResponse>
    {
        public async Task<ListAllTicketsResponse> ExecuteAsync(ListAllTicketQuery command, CancellationToken ct)
        {
            logger.LogInformation($"Query all tickets for project {command.ProjectId}");
            var tickets = await repo.GetAllAsync(t => t.ProjectId == command.ProjectId, ct);
            return new ListAllTicketsResponse(tickets.Adapt<IEnumerable<TicketDto>>());
        }
    }
}