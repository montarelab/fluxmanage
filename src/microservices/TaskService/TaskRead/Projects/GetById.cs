using Common.Domain.Entities;
using FastEndpoints;
using Mapster;
using TaskRead.Dto;
using TaskRead.Services;

namespace TaskRead.Projects;

public static class GetProjectById
{
    public record GetProjectByIdQuery(Guid Id) : ICommand<GetProjectByIdResponse>;
    public record GetProjectByIdResponse(ProjectDto Project);

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

    public class CommandHandler (
        ILogger<CommandHandler> logger,
        IRepository<Project> repo)
        : ICommandHandler<GetProjectByIdQuery, GetProjectByIdResponse>
    {
        public async Task<GetProjectByIdResponse> ExecuteAsync(GetProjectByIdQuery command, CancellationToken ct)
        {
            logger.LogInformation($"Query project by id {command.Id}");
            var project = await repo.GetByIdAsync(command.Id, ct);
            return new GetProjectByIdResponse(project.Adapt<ProjectDto>());
        }
    }
}