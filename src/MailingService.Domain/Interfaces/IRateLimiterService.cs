namespace MailingService.Domain.Interfaces
{
    public interface IRateLimiterService
    {
        Task WaitForNextSlotAsync();
        bool CanSendEmail();
        int GetRemainingTimeInSeconds();
    }
}