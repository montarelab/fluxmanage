using Common.Domain.Entities;
using FastEndpoints;
using Mapster;
using TaskRead.Dto;
using TaskRead.Services;

namespace TaskRead.Epics;

public static class GetEpicById
{
    public record GetEpicByIdQuery(Guid Id) : ICommand<GetEpicByIdResponse>;
    public record GetEpicByIdResponse(EpicDto Epic);

    public class Endpoint : EndpointWithoutRequest<GetEpicByIdResponse>
    {
        public override void Configure()
        {
            Get("/epics/getById/{id:guid}");
            AllowAnonymous();
            // todo introduce permissions
        }

        public override async Task HandleAsync(CancellationToken ct)
        {
            var id = Route<Guid>("id");
            var result = await new GetEpicByIdQuery(id).ExecuteAsync(ct);
            await SendOkAsync(result, ct);
        }
    }

    public class CommandHandler(
        ILogger<CommandHandler> logger,
        IRepository<Epic> repo)
        : ICommandHandler<GetEpicByIdQuery, GetEpicByIdResponse>
    {
        public async Task<GetEpicByIdResponse> ExecuteAsync(GetEpicByIdQuery command, CancellationToken ct)
        {
            logger.LogInformation($"Query epic by id {command.Id}");
            var epic = await repo.GetByIdAsync(command.Id, ct);
            return new GetEpicByIdResponse(epic.Adapt<EpicDto>());
        }
    }
}