using eLib.NotificationService.DAL;
using MimeKit;
using MailKit.Net.Smtp;

namespace eLib.NotificationService.Senders.Email;

public class EmailNotificationSender : IEmailNotificationSender
{
    private readonly ILogger<EmailNotificationSender> _logger;
    private readonly IConfiguration _configuration;

    public EmailNotificationSender(
        ILogger<EmailNotificationSender> logger,
        IConfiguration configuration)
    {
        _logger = logger;
        _configuration = configuration;
    }

    public async Task SendAsync(Notification notification, CancellationToken cancellationToken)
    {
        if (notification.FailedAt != null)
            return;

        var senderEmail = _configuration["EmailSettings:SenderEmail"];
        var senderName = _configuration["EmailSettings:SenderName"];
        var host = _configuration["EmailSettings:Host"];
        var port = int.Parse(_configuration["EmailSettings:Port"]);
        var useSSL = bool.Parse(_configuration["EmailSettings:UseSSL"]);
        var username = _configuration["EmailSettings:Username"];
        var password = _configuration["EmailSettings:Password"];

        var smtpClient = new SmtpClient();
        try
        {
            var mailMessage = new MimeMessage();
            mailMessage.From.Add(new MailboxAddress(senderName, senderEmail));
            mailMessage.To.Add(new MailboxAddress("Client", notification.Details.Email));
            mailMessage.Subject = notification.Title;
            mailMessage.Body = new TextPart("html")
            {
                Text = notification.Message
            };


            await smtpClient.ConnectAsync(host, port, useSSL, cancellationToken);
            await smtpClient.AuthenticateAsync(username, password, cancellationToken);
            await smtpClient.SendAsync(mailMessage, cancellationToken);
            notification.MarkAsSent();
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Failed to send email notification");
            notification.MarkAsFailed();
        }
        finally
        {
            await smtpClient.DisconnectAsync(true, cancellationToken);
        }

        await smtpClient.DisconnectAsync(true, cancellationToken);
    }
}

public interface IEmailNotificationSender : INotificationSender;