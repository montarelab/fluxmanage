using Auth.JWT;
using FastEndpoints;

namespace Auth.Auth;

public class SignIn
{
    public record LoginRequest(string Email, string Password) : ICommand<Guid>;
    public class Endpoint : Endpoint<LoginRequest, Guid>
    {
        public override void Configure()
        {
            Post("/auth/login");
            AllowAnonymous();
        }

        public override async Task HandleAsync(LoginRequest req, CancellationToken ct)
        {
            await req.ExecuteAsync(ct);
            var result = await new LoginRequest(id).ExecuteAsync(ct);
            await SendOkAsync(result, ct);
        }
    }

    public class CommandHandler(
        ILogger<CommandHandler> logger,
        ITokenService tokenService)
        : ICommandHandler<LoginRequest, Guid>
    {
        public async Task<Guid> ExecuteAsync(LoginRequest req, CancellationToken ct)
        {
            logger.LogInformation($"Login request from {req.Email}");
            var token = tokenService.GenerateToken(req.Email);
            var task = await repo.GetByIdAsync(req.Id, ct);
            return new GetTicketByIdResponse(task.Adapt<TicketDto>());
        }
    }
}