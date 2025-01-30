using System.Threading.Tasks;

namespace MailingService.Domain.Interfaces
{
    public interface IEmailService
    {
        Task<bool> SendEmailAsync(string to, string subject, string htmlContent);
        Task<int> GetDailyEmailCountAsync();
        Task<bool> HasEmailBeenSentAsync(string email, string templateId);
        Task TrackEmailSentAsync(string email, string templateId, bool isSuccessful, string errorMessage = null);
    }
}