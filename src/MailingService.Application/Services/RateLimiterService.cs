using MailingService.Domain.Interfaces;
using MailingService.Domain.Settings;
using Microsoft.Extensions.Options;

namespace MailingService.Application.Services
{
    public class RateLimiterService : IRateLimiterService
    {
        private readonly RateLimitSettings _settings;
        private DateTime _lastEmailSent = DateTime.MinValue;
        private readonly object _lock = new object();

        public RateLimiterService(IOptions<RateLimitSettings> settings)
        {
            _settings = settings.Value;
        }

        public async Task WaitForNextSlotAsync()
        {
            lock (_lock)
            {
                var now = DateTime.UtcNow;
                var timeSinceLastEmail = (now - _lastEmailSent).TotalSeconds;

                if (timeSinceLastEmail < _settings.SecondsBetweenEmails)
                {
                    var waitTime = _settings.SecondsBetweenEmails - (int)timeSinceLastEmail;
                    Task.Delay(TimeSpan.FromSeconds(waitTime)).Wait();
                }

                _lastEmailSent = DateTime.UtcNow;
            }

            await Task.CompletedTask;
        }

        public bool CanSendEmail()
        {
            var timeSinceLastEmail = (DateTime.UtcNow - _lastEmailSent).TotalSeconds;
            return timeSinceLastEmail >= _settings.SecondsBetweenEmails;
        }

        public int GetRemainingTimeInSeconds()
        {
            var timeSinceLastEmail = (DateTime.UtcNow - _lastEmailSent).TotalSeconds;
            var remainingTime = _settings.SecondsBetweenEmails - (int)timeSinceLastEmail;
            return remainingTime > 0 ? remainingTime : 0;
        }
    }
}