using Common.Domain.Entities;
using FastEndpoints;
using Mapster;
using TaskRead.Dto;
using TaskRead.Services;

namespace TaskRead.Tickets;

public static class GetTicketById
{
    public record GetTicketByIdQuery(Guid Id) : ICommand<GetTicketByIdResponse>;
    public record GetTicketByIdResponse(TicketDto Ticket);

    public class Endpoint : EndpointWithoutRequest<GetTicketByIdResponse>
    {
        public override void Configure()
        {
            Get("/tickets/getById/{id:guid}");
            AllowAnonymous();
            // todo introduce permissions
        }

        public override async Task HandleAsync(CancellationToken ct)
        {
            var id = Route<Guid>("id");
            var result = await new GetTicketByIdQuery(id).ExecuteAsync(ct);
            await SendOkAsync(result, ct);
        }
    }

    public class CommandHandler(
        ILogger<CommandHandler> logger,
        IRepository<Ticket> repo)
        : ICommandHandler<GetTicketByIdQuery, GetTicketByIdResponse>
    {
        public async Task<GetTicketByIdResponse> ExecuteAsync(GetTicketByIdQuery command, CancellationToken ct)
        {
            logger.LogInformation($"Query task by id {command.Id}");
            var task = await repo.GetByIdAsync(command.Id, ct);
            return new GetTicketByIdResponse(task.Adapt<TicketDto>());
        }
    }
}