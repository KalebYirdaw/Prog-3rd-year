using GLMS.Models;

namespace GLMS.Services
{
    public class EmailService : IEmailService
    {
        private readonly ILogger<EmailService> _logger;

        public EmailService(ILogger<EmailService> logger)
        {
            _logger = logger;
        }

        public async Task SendEmailAsync(string to, string subject, string body, string? relatedEntity = null, int? relatedEntityId = null)
        {
            _logger.LogInformation($"📧 [FAKE EMAIL] To: {to}, Subject: {subject}");
            _logger.LogInformation($"Body: {body}");
            await Task.CompletedTask;
        }

        public async Task<List<EmailLog>> GetEmailLogsAsync()
        {
            return await Task.FromResult(new List<EmailLog>());
        }

        public async Task MarkEmailAsRead(int emailLogId)
        {
            await Task.CompletedTask;
        }
    }
}