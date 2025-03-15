using Auth.Entities;
using Auth.JWT;
using FastEndpoints;
using FluentValidation;
using Microsoft.AspNetCore.Identity;

namespace Auth.Auth;

public class SignIn
{
    public record LoginRequest(string Email, string Password) : ICommand<ApplicationTokenResponse>;
    
    public class SignInValidator : Validator<LoginRequest>
    {
        public SignInValidator(UserManager<ApplicationUser> userManager)
        {
            RuleFor(x => x.Email)
                .NotEmpty()
                .EmailAddress()
                .WithMessage("Invalid email address")
                .MustAsync(async (email, ct) => await userManager.FindByEmailAsync(email) is not null)
                .WithMessage("User not found");
                
            
            RuleFor(x => x.Password)
                .NotEmpty()
                .MinimumLength(6)
                .WithMessage("Password must be at least 6 characters long")
                .MustAsync(async (req, password, ct) =>
                {
                    var user = await userManager.FindByEmailAsync(req.Email)!;
                    return await userManager.CheckPasswordAsync(user!, password);
                })
                .WithMessage("Invalid password");
        }
    } 
    
    public class Endpoint : Endpoint<LoginRequest, ApplicationTokenResponse>
    {
        public override void Configure()
        {
            Post("/auth/login");
            AllowAnonymous();
        }

        public override async Task HandleAsync(LoginRequest req, CancellationToken ct)
        {
            await req.ExecuteAsync(ct);
            var result = await req.ExecuteAsync(ct);
            await SendOkAsync(result, ct);
        }
    }

    public class CommandHandler(
        ILogger<CommandHandler> logger,
        UserManager<ApplicationUser> userManager,
        SignInManager<ApplicationUser> signInManager,
        ITokenService tokenService)
        : ICommandHandler<LoginRequest, ApplicationTokenResponse>
    {
        public async Task<ApplicationTokenResponse> ExecuteAsync(LoginRequest req, CancellationToken ct)
        {
            logger.LogInformation($"Login request from {req.Email}");
            var user = await userManager.FindByEmailAsync(req.Email)!;
            await signInManager.PasswordSignInAsync(user!, req.Password, false, false);
            var token = tokenService.GenerateAccessToken(req.Email);
            var refreshToken = tokenService.GenerateRefreshToken();
            return new ApplicationTokenResponse(token, refreshToken);
        }
    }
}