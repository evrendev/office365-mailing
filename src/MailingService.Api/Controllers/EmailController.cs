using Microsoft.AspNetCore.Mvc;
using MailingService.Domain.Interfaces;
using MailingService.Domain.Settings;
using Microsoft.Extensions.Options;
using System.Threading.Tasks;
using System;
using System.IO;
using CsvHelper;
using System.Globalization;
using System.Linq;
using System.Collections.Generic;

namespace MailingService.Api.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class EmailController : ControllerBase
    {
        private readonly IEmailService _emailService;
        private readonly MailSettings _mailSettings;

        public class EmailRecipient
        {
            public string Email { get; set; }
            public string Name { get; set; }
        }

        public EmailController(IEmailService emailService, IOptions<MailSettings> mailSettings)
        {
            _emailService = emailService;
            _mailSettings = mailSettings.Value;
        }

        [HttpPost("send-batch")]
        public async Task<IActionResult> SendBatchEmails()
        {
            try
            {
                // Read HTML template
                if (!System.IO.File.Exists(_mailSettings.HtmlTemplatePath))
                {
                    return BadRequest("HTML template file not found");
                }
                string htmlTemplate = await System.IO.File.ReadAllTextAsync(_mailSettings.HtmlTemplatePath);

                // Read CSV file
                if (!System.IO.File.Exists(_mailSettings.CsvFilePath))
                {
                    return BadRequest("CSV file not found");
                }

                var recipients = new List<EmailRecipient>();
                using (var reader = new StreamReader(_mailSettings.CsvFilePath))
                using (var csv = new CsvReader(reader, CultureInfo.InvariantCulture))
                {
                    recipients = csv.GetRecords<EmailRecipient>().ToList();
                }

                var results = new List<string>();
                foreach (var recipient in recipients)
                {
                    // Check if email was already sent
                    if (await _emailService.HasEmailBeenSentAsync(recipient.Email, _mailSettings.HtmlTemplatePath))
                    {
                        results.Add($"Skipped {recipient.Email} - Already sent");
                        continue;
                    }

                    // Check daily limit
                    var dailyCount = await _emailService.GetDailyEmailCountAsync();
                    if (dailyCount >= _mailSettings.DailyEmailLimit)
                    {
                        return BadRequest("Daily email limit reached");
                    }

                    try
                    {
                        // Personalize the template if needed
                        string personalizedHtml = htmlTemplate.Replace("{{Name}}", recipient.Name);

                        // Send email
                        await _emailService.SendEmailAsync(
                            recipient.Email,
                            "Your Email Subject",
                            personalizedHtml);

                        // Track successful send
                        await _emailService.TrackEmailSentAsync(
                            recipient.Email,
                            _mailSettings.HtmlTemplatePath,
                            true);

                        results.Add($"Successfully sent to {recipient.Email}");
                    }
                    catch (Exception ex)
                    {
                        // Track failed send
                        await _emailService.TrackEmailSentAsync(
                            recipient.Email,
                            _mailSettings.HtmlTemplatePath,
                            false,
                            ex.Message);

                        results.Add($"Failed to send to {recipient.Email}: {ex.Message}");
                    }
                }

                return Ok(new { Results = results });
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Internal server error: {ex.Message}");
            }
        }

        [HttpGet("daily-count")]
        public async Task<IActionResult> GetDailyCount()
        {
            try
            {
                var count = await _emailService.GetDailyEmailCountAsync();
                var remaining = _mailSettings.DailyEmailLimit - count;
                return Ok(new
                {
                    SentToday = count,
                    RemainingLimit = remaining,
                    DailyLimit = _mailSettings.DailyEmailLimit
                });
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Internal server error: {ex.Message}");
            }
        }
    }
}