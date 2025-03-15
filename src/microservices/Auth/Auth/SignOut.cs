using Auth.Entities;
using Auth.JWT;
using FastEndpoints;
using FluentValidation;
using Microsoft.AspNetCore.Identity;

namespace Auth.Auth;

public class SignOut
{
    public record LogoutRequest : ICommand;
    public class Endpoint : Endpoint<LogoutRequest>
    {
        public override void Configure()
        {
            Post("/auth/logout");
            AllowAnonymous();
        }

        public override async Task HandleAsync(LogoutRequest req, CancellationToken ct)
        {
            await req.ExecuteAsync(ct);
            await SendOkAsync(ct);
        }
    }

    public class CommandHandler(
        ILogger<CommandHandler> logger,
        SignInManager<ApplicationUser> signInManager)
        : ICommandHandler<LogoutRequest>
    {
        public async Task ExecuteAsync(LogoutRequest req, CancellationToken ct)
        {
            logger.LogInformation($"Logout request received");
            await signInManager.SignOutAsync();
        }
    }
}