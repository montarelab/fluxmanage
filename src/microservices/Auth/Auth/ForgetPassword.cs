using Auth.Entities;
using FastEndpoints;
using FluentValidation;
using Microsoft.AspNetCore.Identity;

namespace Auth.Auth;

public class ForgetPassword
{
    public record ForgotPasswordRequest(
        string Email
    ) : ICommand;

    public class ForgotPasswordValidator : Validator<ForgotPasswordRequest>
    {
        public ForgotPasswordValidator(UserManager<ApplicationUser> userManager)
        {
            RuleFor(request => request.Email)
                .NotEmpty()
                .EmailAddress()
                .WithMessage("Invalid email address")
                .MustAsync(async (email, ct) => await userManager.FindByEmailAsync(email) is not null)
                .WithMessage("User not found");
        }
    }
    
    public class Endpoint : Endpoint<ForgotPasswordRequest>
    {
        public override void Configure()
        {
            Post("/auth/forgot-password");
            AllowAnonymous();
        }

        public override async Task HandleAsync(ForgotPasswordRequest req, CancellationToken ct)
        {
            await req.ExecuteAsync(ct);
            await SendOkAsync(ct);
        }
    }

    public class CommandHandler(
        ILogger<CommandHandler> logger,
        UserManager<ApplicationUser> userManager)
        : ICommandHandler<ForgotPasswordRequest>
    {
        public async Task ExecuteAsync(ForgotPasswordRequest req, CancellationToken ct)
        {
            logger.LogInformation($"Forgot password request for email {req.Email}");
            var user = await userManager.FindByEmailAsync(req.Email);
            
            if (user != null)
            {
                // Generate password reset token
                var token = await userManager.GeneratePasswordResetTokenAsync(user);
                
                // In a real-world scenario, you would send this token to the user's email
                // along with a link to the reset password page
                logger.LogInformation($"Generated password reset token for user {req.Email}");
                
                // TODO: Send email with reset link that includes token
                // Example reset link: https://yourapp.com/reset-password?email={email}&token={token}
            }
        }
    }
}