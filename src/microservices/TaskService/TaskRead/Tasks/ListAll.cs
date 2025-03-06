using Common.EventSourcing;
using FastEndpoints;
using Task = System.Threading.Tasks.Task;

namespace TaskRead.Tasks;

public static class ListAll
{
    public record ListAllTasksQuery(Guid ProjectId) : ICommand<ListAllTasksResponse>;
    public record ListAllTasksResponse(Guid Id); // todo fix it
    
    public class Endpoint : EndpointWithoutRequest<ListAllTasksResponse>
    {
        public override void Configure()
        {
            Get("/tasks/all/{projectId:guid}");
            AllowAnonymous();
            // todo introduce permissions
        }

        public override async Task HandleAsync(CancellationToken ct)
        {
            var projectId = Route<Guid>("projectId");
            var result = await new ListAllTasksQuery(projectId).ExecuteAsync(ct);
            await SendOkAsync(result, ct);
        }
    }
    
    public class CommandHandler : ICommandHandler<ListAllTasksQuery, ListAllTasksResponse>
    {
        public Task<ListAllTasksResponse> ExecuteAsync(ListAllTasksQuery command, CancellationToken ct)
        {
            Console.WriteLine(command.ProjectId);
            // todo data processing
            return Task.FromResult(new ListAllTasksResponse(Guid.NewGuid()));
        }
    }
}