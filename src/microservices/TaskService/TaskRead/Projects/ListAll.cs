using Common.Domain.Entities;
using FastEndpoints;
using Mapster;
using TaskRead.Dto;
using TaskRead.Services;

namespace TaskRead.Projects;

public static class ListAllProjects
{
    public record ListAllProjectsQuery : ICommand<ListAllProjectsResponse>;
    public record ListAllProjectsResponse(IEnumerable<ProjectDto> Projects);

    public class Endpoint : EndpointWithoutRequest<ListAllProjectsResponse>
    {
        public override void Configure()
        {
            Get("/projects/all");
            AllowAnonymous();
            // todo introduce permissions
        }

        public override async Task HandleAsync(CancellationToken ct)
        {
            var result = await new ListAllProjectsQuery().ExecuteAsync(ct);
            await SendOkAsync(result, ct);
        }
    }

    public class CommandHandler(ILogger<CommandHandler> logger, IRepository<Project> repo) 
        : ICommandHandler<ListAllProjectsQuery, ListAllProjectsResponse>
    {
        public async Task<ListAllProjectsResponse> ExecuteAsync(ListAllProjectsQuery command, CancellationToken ct)
        {
            logger.LogInformation("Query all projects");
            var projects = await repo.GetAllAsync(null, ct);
            return new ListAllProjectsResponse(projects.Adapt<IEnumerable<ProjectDto>>());
        }
    }
}