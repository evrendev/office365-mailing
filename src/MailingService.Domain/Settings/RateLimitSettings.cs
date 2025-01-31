namespace MailingService.Domain.Settings
{
    public class RateLimitSettings
    {
        public const int SECONDS_PER_DAY = 86400;

        private int _dailyEmailLimit = 3600;
        public int DailyEmailLimit
        {
            get => _dailyEmailLimit;
            set
            {
                _dailyEmailLimit = value;
                // Calculate seconds between each email
                SecondsBetweenEmails = SECONDS_PER_DAY / value;
                // Calculate emails per second
                EmailsPerSecond = (double)value / SECONDS_PER_DAY;
            }
        }

        public int SecondsBetweenEmails { get; private set; } = SECONDS_PER_DAY / 3600; // Default 24 seconds
        public double EmailsPerSecond { get; private set; } = 3600.0 / SECONDS_PER_DAY; // Default 0.0416666...
    }
}