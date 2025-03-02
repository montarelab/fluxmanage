using FastEndpoints;

namespace TaskRead.Projects;

public static class GetProjectById
{
    public record GetProjectByIdQuery(Guid Id) : ICommand<GetProjectByIdResponse>;
    public record GetProjectByIdResponse(Guid Id);

    public class Endpoint : EndpointWithoutRequest<GetProjectByIdResponse>
    {
        public override void Configure()
        {
            Get("/projects/getById/{id:guid}");
            AllowAnonymous();
            // todo introduce permissions
        }

        public override async Task HandleAsync(CancellationToken ct)
        {
            var id = Route<Guid>("id");
            var result = await new GetProjectByIdQuery(id).ExecuteAsync(ct);
            await SendOkAsync(result, ct);
        }
    }

    public class CommandHandler : ICommandHandler<GetProjectByIdQuery, GetProjectByIdResponse>
    {
        public Task<GetProjectByIdResponse> ExecuteAsync(GetProjectByIdQuery command, CancellationToken ct)
        {
            Console.WriteLine(command.Id);
            return Task.FromResult(new GetProjectByIdResponse(Guid.NewGuid()));
        }
    }
}