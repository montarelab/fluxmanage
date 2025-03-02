using FastEndpoints;

namespace TaskRead.Projects;

public static class ListAllProjects
{
    public record ListAllProjectsQuery : ICommand<ListAllProjectsResponse>;
    public record ListAllProjectsResponse(Guid Id);

    public class Endpoint : EndpointWithoutRequest<ListAllProjectsResponse>
    {
        public override void Configure()
        {
            Get("/projects/all/{projectId:guid}");
            AllowAnonymous();
            // todo introduce permissions
        }

        public override async Task HandleAsync(CancellationToken ct)
        {
            var projectId = Route<Guid>("projectId");
            var result = await new ListAllProjectsQuery().ExecuteAsync(ct);
            await SendOkAsync(result, ct);
        }
    }

    public class CommandHandler : ICommandHandler<ListAllProjectsQuery, ListAllProjectsResponse>
    {
        public Task<ListAllProjectsResponse> ExecuteAsync(ListAllProjectsQuery command, CancellationToken ct)
        {
            Console.WriteLine("All projects query");
            return Task.FromResult(new ListAllProjectsResponse(Guid.NewGuid()));
        }
    }
}