using Common.Domain.Entities;
using FastEndpoints;
using Mapster;
using TaskRead.Dto;
using TaskRead.Services;

namespace TaskRead.Epics;

public static class ListAllEpics
{
    public record ListAllEpicsQuery(Guid ProjectId) : ICommand<ListAllEpicsResponse>;
    public record ListAllEpicsResponse(IEnumerable<EpicDto> Epics);

    public class Endpoint : EndpointWithoutRequest<ListAllEpicsResponse>
    {
        public override void Configure()
        {
            Get("/epics/all/{projectId:guid}");
            AllowAnonymous();
            // todo introduce permissions
        }

        public override async Task HandleAsync(CancellationToken ct)
        {
            var projectId = Route<Guid>("projectId");
            var result = await new ListAllEpicsQuery(projectId).ExecuteAsync(ct);
            await SendOkAsync(result, ct);
        }
    }

    public class CommandHandler(
        ILogger<CommandHandler> logger,
        IRepository<Epic> repo)
        : ICommandHandler<ListAllEpicsQuery, ListAllEpicsResponse>
    {
        public async Task<ListAllEpicsResponse> ExecuteAsync(ListAllEpicsQuery command, CancellationToken ct)
        {
            logger.LogInformation($"Query all epics for project {command.ProjectId}");
            var epics = await repo.GetAllAsync(e => e.ProjectId == command.ProjectId, ct);
            return new ListAllEpicsResponse(epics.Adapt<IEnumerable<EpicDto>>());
        }
    }
}