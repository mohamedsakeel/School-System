using Hangfire;
using SMS.Notification;

namespace SMS.Web.Services
{
    public class EmailService : IEmailService
    {
        private readonly IEmailSender _sender;

        public EmailService(IEmailSender sender)
        {
            _sender = sender;
        }

        public Task SendEmailAsync(string email, string subject, string message)
        {
            // Queue the email sending job to Hangfire
            BackgroundJob.Enqueue(() => _sender.SendEmailAsync(email, subject, message));
            return Task.CompletedTask;
        }
    }
}
