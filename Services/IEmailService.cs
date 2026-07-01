using GLMS.Models;

namespace GLMS.Services
{
    public interface IEmailService
    {
        Task SendEmailAsync(string to, string subject, string body, string? relatedEntity = null, int? relatedEntityId = null);
        Task<List<EmailLog>> GetEmailLogsAsync();
        Task MarkEmailAsRead(int emailLogId);
    }
}