using System.Net.Mail;
using System.Net;
using EmailSenderApp.Domain;
using Microsoft.AspNetCore.Http;

namespace EmailSenderApp.Service;

public class EmailService : IEmailService
{
    private readonly IConfiguration _config;
    private readonly ILogger<EmailService> _logger;

    public EmailService(IConfiguration config, ILogger<EmailService> logger)
    {
        _config = config;
        _logger = logger;
    }

    public async Task<bool> Send(Email email)
    {
        // List to hold streams that need to be disposed after sending
        List<MemoryStream> attachmentStreams = new List<MemoryStream>();

        try
        {
            var emailSettings = _config.GetSection("EmailSettings");

            using var mailMessage = new MailMessage
            {
                From = new MailAddress(emailSettings["Sender"], emailSettings["SenderName"]),
                Subject = email.Subject,
                Body = email.Body,
                IsBodyHtml = true
            };

            mailMessage.To.Add(email.To);

            // Handle attachments
            if (email.Attachment != null && email.Attachment.Any())
            {
                foreach (var file in email.Attachment)
                {
                    if (file.Length > 0)
                    {
                        // Create and store the stream
                        var memoryStream = new MemoryStream();
                        await file.CopyToAsync(memoryStream);
                        memoryStream.Position = 0;
                        attachmentStreams.Add(memoryStream); // Keep track for disposal

                        // Create attachment
                        var attachment = new Attachment(memoryStream, file.FileName, file.ContentType);
                        mailMessage.Attachments.Add(attachment);
                    }
                }
            }

            using var smtpClient = new SmtpClient(emailSettings["MailServer"], int.Parse(emailSettings["MailPort"]))
            {
                Credentials = new NetworkCredential(emailSettings["Sender"], emailSettings["Password"]),
                EnableSsl = true,
                Timeout = 10000
            };

            await smtpClient.SendMailAsync(mailMessage);
            return true;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error sending email");
            return false;
        }
        finally
        {
            // Dispose all attachment streams after email is sent
            foreach (var stream in attachmentStreams)
            {
                stream.Dispose();
            }
        }
    }
}