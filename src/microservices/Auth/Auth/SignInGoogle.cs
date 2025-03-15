using Auth.Entities;
using Auth.JWT;
using FastEndpoints;
using Google.Apis.Auth;
using Microsoft.AspNetCore.Identity;

namespace Auth.Auth;

public class SignInGoogle
{
    public record LoginGoogleRequest(string Credential) : ICommand<ApplicationTokenResponse>;
    public class Endpoint : Endpoint<LoginGoogleRequest, ApplicationTokenResponse>
    {
        public override void Configure()
        {
            Post("/signin-google");
            AllowAnonymous();
        }

        public override async Task HandleAsync(LoginGoogleRequest req, CancellationToken ct)
        {
            await req.ExecuteAsync(ct);
            var result = await req.ExecuteAsync(ct);
            await SendOkAsync(result, ct);
        }
    }

    public class CommandHandler(
        ILogger<CommandHandler> logger,
        UserManager<ApplicationUser> userManager,
        ITokenService tokenService)
        : ICommandHandler<LoginGoogleRequest, ApplicationTokenResponse>
    {
        public async Task<ApplicationTokenResponse> ExecuteAsync(LoginGoogleRequest req, CancellationToken ct)
        {
            logger.LogInformation($"Login request from google with credential {req.Credential}");
            var payload = await GoogleJsonWebSignature.ValidateAsync(req.Credential) ??
                throw new Exception("Invalid google credential");
            
            if(payload.Email is null)
                throw new Exception("Invalid google credential");
            
            var user = await userManager.FindByEmailAsync(payload.Email);
            if (user == null)
            {
                // Optionally create the user in your system or return an error
                user = new ApplicationUser { Email = payload.Email, UserName = payload.Email };
                await userManager.CreateAsync(user);
            }
            
            // todo assign roles to user
            
            var token = tokenService.GenerateAccessToken(payload.Email);
            var refreshToken = tokenService.GenerateRefreshToken();
            return new ApplicationTokenResponse(token, refreshToken);
        }
    }
}