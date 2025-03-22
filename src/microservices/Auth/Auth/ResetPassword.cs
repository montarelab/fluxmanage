using Auth.Entities;
using Auth.JWT;
using FastEndpoints;
using FluentValidation;
using Microsoft.AspNetCore.Identity;

namespace Auth.Auth;

public class ResetPassword
{
    public record ResetPasswordRequest(
        string Email,
        string Token,
        string NewPassword,
        string ConfirmPassword
    ) : ICommand<ApplicationTokenResponse>;

    public class ResetPasswordValidator : Validator<ResetPasswordRequest>
    {
        public ResetPasswordValidator(UserManager<ApplicationUser> userManager)
        {
            RuleFor(request => request.Email)
                .NotEmpty()
                .EmailAddress()
                .WithMessage("Invalid email address")
                .MustAsync(async (email, ct) => await userManager.FindByEmailAsync(email) is not null)
                .WithMessage("User not found");

            RuleFor(request => request.Token)
                .NotEmpty()
                .WithMessage("Token is required");

            RuleFor(request => request.NewPassword)
                .NotEmpty()
                .MinimumLength(6)
                .WithMessage("Password must be at least 6 characters long");

            RuleFor(request => request.ConfirmPassword)
                .NotEmpty()
                .MinimumLength(6)
                .Must((req, confirmPassword) => string.Equals(req.NewPassword, confirmPassword,
                    StringComparison.InvariantCulture))
                .WithMessage("Passwords do not match");
        }
    }
    
    public class Endpoint : Endpoint<ResetPasswordRequest, ApplicationTokenResponse>
    {
        public override void Configure()
        {
            Post("/auth/reset-password");
            AllowAnonymous();
        }

        public override async Task HandleAsync(ResetPasswordRequest req, CancellationToken ct)
        {
            var result = await req.ExecuteAsync(ct);
            await SendOkAsync(result, ct);
        }
    }

    public class CommandHandler(
        ILogger<CommandHandler> logger,
        UserManager<ApplicationUser> userManager,
        SignInManager<ApplicationUser> signInManager,
        ITokenService tokenService)
        : ICommandHandler<ResetPasswordRequest, ApplicationTokenResponse>
    {
        public async Task<ApplicationTokenResponse> ExecuteAsync(ResetPasswordRequest req, CancellationToken ct)
        {
            logger.LogInformation($"Reset password request for email {req.Email}");
            var user = await userManager.FindByEmailAsync(req.Email);
            
            if (user == null)
            {
                throw new Exception("User not found");
            }
            
            // Reset the password using the token
            var result = await userManager.ResetPasswordAsync(user, req.Token, req.NewPassword);
            
            if (!result.Succeeded)
            {
                var errors = string.Join(", ", result.Errors.Select(e => e.Description));
                throw new Exception($"Failed to reset password: {errors}");
            }
            
            // Automatically sign in the user after successful password reset
            await signInManager.SignInAsync(user, isPersistent: false);
            
            // Generate JWT tokens
            var token = tokenService.GenerateAccessToken(req.Email);
            var refreshToken = tokenService.GenerateRefreshToken();
            
            logger.LogInformation($"Password successfully reset for user {req.Email}");
            
            return new ApplicationTokenResponse(token, refreshToken);
        }
    }
}