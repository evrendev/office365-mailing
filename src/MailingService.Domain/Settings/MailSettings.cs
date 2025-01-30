namespace MailingService.Domain.Settings
{
    public class MailSettings
    {
        public string Office365Username { get; set; }
        public string Office365Password { get; set; }
        public string SmtpServer { get; set; } = "smtp.office365.com";
        public int SmtpPort { get; set; } = 587;
        public string FromEmail { get; set; }
        public string FromName { get; set; }
        public string HtmlTemplatePath { get; set; }
        public string CsvFilePath { get; set; }
        public int DailyEmailLimit { get; set; } = 3600;
        public bool EnableSsl { get; set; } = true;
    }
}