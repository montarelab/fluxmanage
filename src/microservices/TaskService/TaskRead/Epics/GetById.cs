using FastEndpoints;

namespace TaskRead.Epics;

public static class GetEpicById
{
    public record GetEpicByIdQuery(Guid Id) : ICommand<GetEpicByIdResponse>;
    public record GetEpicByIdResponse(Guid Id);

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

    public class CommandHandler : ICommandHandler<GetEpicByIdQuery, GetEpicByIdResponse>
    {
        public Task<GetEpicByIdResponse> ExecuteAsync(GetEpicByIdQuery command, CancellationToken ct)
        {
            Console.WriteLine(command.Id);
            return Task.FromResult(new GetEpicByIdResponse(Guid.NewGuid()));
        }
    }
}