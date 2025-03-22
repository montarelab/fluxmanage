using System;
using MailKit.Net.Smtp;
using MailKit;
using MimeKit;
using MailKit.Security;

var email = new MimeMessage();

// Get credentials from environment variables or configuration
string senderEmail = Environment.GetEnvironmentVariable("EMAIL_SENDER") ?? "fluxmanage.start@gmail.com";
string senderName = Environment.GetEnvironmentVariable("EMAIL_SENDER_NAME") ?? "Flux Manage";
string recipientEmail = Environment.GetEnvironmentVariable("EMAIL_RECIPIENT") ?? "dmitriytkachenko350@gmail.com";
string recipientName = Environment.GetEnvironmentVariable("EMAIL_RECIPIENT_NAME") ?? "Receiver Name";
string password = Environment.GetEnvironmentVariable("EMAIL_PASSWORD");

if (string.IsNullOrEmpty(password))
{
    Console.WriteLine("Warning: Email password not set in environment variables. Using fallback method.");
    // In production, you should NOT have a fallback password in code
    // This is only for development/testing purposes
    password = "your-app-password"; // Replace with an app password for Gmail
}

email.From.Add(new MailboxAddress(senderName, senderEmail));
email.To.Add(new MailboxAddress(recipientName, recipientEmail));

email.Subject = "Testing out email sending";
email.Body = new TextPart(MimeKit.Text.TextFormat.Html) { 
    Text = "<b>Hello all the way from the land of C#</b>"
};

try
{
    using (var smtp = new SmtpClient())
    {
        smtp.Connect("smtp.gmail.com", 587, SecureSocketOptions.StartTls);

        // Note: Gmail requires using App Passwords if 2FA is enabled
        // Create an app password at: https://myaccount.google.com/apppasswords
        smtp.Authenticate(senderEmail, password);

        smtp.Send(email);
        smtp.Disconnect(true);
        
        Console.WriteLine("Email sent successfully!");
    }
}
catch (Exception ex)
{
    Console.WriteLine($"Failed to send email: {ex.Message}");
    // In production code, consider logging the exception properly
}
