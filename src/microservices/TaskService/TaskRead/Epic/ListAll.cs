using FastEndpoints;

namespace TaskRead.Epic;

public static class ListAllEpics
{
    public record ListAllEpicsQuery(Guid ProjectId) : ICommand<ListAllEpicsResponse>;
    public record ListAllEpicsResponse(Guid Id);

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

    public class CommandHandler : ICommandHandler<ListAllEpicsQuery, ListAllEpicsResponse>
    {
        public Task<ListAllEpicsResponse> ExecuteAsync(ListAllEpicsQuery command, CancellationToken ct)
        {
            Console.WriteLine(command.ProjectId);
            return Task.FromResult(new ListAllEpicsResponse(Guid.NewGuid()));
        }
    }
}