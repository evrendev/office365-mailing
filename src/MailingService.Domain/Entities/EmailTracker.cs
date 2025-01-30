namespace MailingService.Domain.Entities
{
    public class EmailTracker
    {
        public int Id { get; set; }
        public string RecipientEmail { get; set; }
        public DateTime SentDate { get; set; }
        public string EmailTemplateId { get; set; }
        public bool IsSuccessful { get; set; }
        public string? ErrorMessage { get; set; }
    }
}