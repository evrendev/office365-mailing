using System.Net;
using System.Net.Mail;
using MailingService.Domain.Entities;
using MailingService.Domain.Interfaces;
using MailingService.Domain.Settings;
using MailingService.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;

namespace MailingService.Application.Services
{
    public class EmailService : IEmailService
    {
        private readonly MailSettings _mailSettings;
        private readonly ApplicationDbContext _context;

        public EmailService(IOptions<MailSettings> mailSettings, ApplicationDbContext context)
        {
            _mailSettings = mailSettings.Value;
            _context = context;
        }

        public async Task<bool> SendEmailAsync(string to, string subject, string htmlContent)
        {
            try
            {
                var dailyCount = await GetDailyEmailCountAsync();
                if (dailyCount >= _mailSettings.DailyEmailLimit)
                {
                    throw new Exception("Daily email limit reached");
                }

                using var message = new MailMessage();
                message.From = new MailAddress(_mailSettings.FromEmail, _mailSettings.FromName);
                message.To.Add(to);
                message.Subject = subject;
                message.Body = htmlContent;
                message.IsBodyHtml = true;

                using var client = new SmtpClient(_mailSettings.SmtpServer, _mailSettings.SmtpPort);
                client.EnableSsl = _mailSettings.EnableSsl;
                client.Credentials = new NetworkCredential(_mailSettings.Office365Username, _mailSettings.Office365Password);

                await client.SendMailAsync(message);
                return true;
            }
            catch (Exception)
            {
                throw;
            }
        }

        public async Task<int> GetDailyEmailCountAsync()
        {
            var today = DateTime.UtcNow.Date;
            return await _context.EmailTrackers
                .CountAsync(x => x.SentDate.Date == today && x.IsSuccessful);
        }

        public async Task<bool> HasEmailBeenSentAsync(string email, string templateId)
        {
            return await _context.EmailTrackers
                .AnyAsync(x => x.RecipientEmail == email &&
                              x.EmailTemplateId == templateId &&
                              x.IsSuccessful);
        }

        public async Task TrackEmailSentAsync(string email, string templateId, bool isSuccessful, string errorMessage = null)
        {
            var tracker = new EmailTracker
            {
                RecipientEmail = email,
                EmailTemplateId = templateId,
                SentDate = DateTime.UtcNow,
                IsSuccessful = isSuccessful,
                ErrorMessage = errorMessage
            };

            _context.EmailTrackers.Add(tracker);
            await _context.SaveChangesAsync();
        }
    }
}