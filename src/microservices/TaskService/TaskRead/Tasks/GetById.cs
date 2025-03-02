using FastEndpoints;

namespace TaskRead.Tasks;

public static class GetTaskById
{
    public record GetTaskByIdQuery(Guid Id) : ICommand<GetTaskByIdResponse>;
    public record GetTaskByIdResponse(Guid Id);
    
    public class Endpoint : EndpointWithoutRequest<GetTaskByIdResponse>
    {
        public override void Configure()
        {
            Get("/tasks/getById/{id:guid}");
            AllowAnonymous(); 
            // todo introduce permissions
        }

        public override async Task HandleAsync(CancellationToken ct)
        {
            var id = Route<Guid>("id");
            var result = await new GetTaskByIdQuery(id).ExecuteAsync(ct);
            await SendOkAsync(result, ct);
        }
    }
    
    public class CommandHandler : ICommandHandler<GetTaskByIdQuery, GetTaskByIdResponse>
    {
        public Task<GetTaskByIdResponse> ExecuteAsync(GetTaskByIdQuery command, CancellationToken ct)
        {
            Console.WriteLine(command.Id);
            return Task.FromResult(new GetTaskByIdResponse(Guid.NewGuid()));
        }
    }
}