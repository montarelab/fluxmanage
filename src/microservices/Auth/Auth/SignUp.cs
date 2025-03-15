using Auth.Entities;
using Auth.JWT;
using FastEndpoints;
using FluentValidation;
using Microsoft.AspNetCore.Identity;

namespace Auth.Auth;

public class SignUp
{
    public record RegisterRequest(
        string Email, 
        string Username, 
        string Password,
        string ConfirmPassword
        ) : ICommand<ApplicationTokenResponse>;

    public class SignInValidator : Validator<RegisterRequest>
    {
        public SignInValidator(UserManager<ApplicationUser> userManager)
        {
            RuleFor(request => request.Email)
                .NotEmpty()
                .EmailAddress()
                .WithMessage("Invalid email address")
                .MustAsync(async (email, ct) => await userManager.FindByEmailAsync(email) is null)
                .WithMessage((_, email) => $"User by email {email} already exists");

            RuleFor(request => request.Username)
                .NotEmpty()
                .MinimumLength(6)
                .WithMessage("Username must be at least 6 characters long");

            RuleFor(request => request.Password)
                .NotEmpty()
                .MinimumLength(6)
                .WithMessage("Password must be at least 6 characters long");

            RuleFor(request => request.ConfirmPassword)
                .NotEmpty()
                .MinimumLength(6)
                .Must((req, confirmPassword) => string.Equals(req.Password, confirmPassword,
                    StringComparison.InvariantCulture))
                .WithMessage("Password must be at least 6 characters long");
        }
    } 
    
    public class Endpoint : Endpoint<RegisterRequest, ApplicationTokenResponse>
    {
        public override void Configure()
        {
            Post("/auth/login");
            AllowAnonymous();
        }

        public override async Task HandleAsync(RegisterRequest req, CancellationToken ct)
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
        : ICommandHandler<RegisterRequest, ApplicationTokenResponse>
    {
        public async Task<ApplicationTokenResponse> ExecuteAsync(RegisterRequest req, CancellationToken ct)
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