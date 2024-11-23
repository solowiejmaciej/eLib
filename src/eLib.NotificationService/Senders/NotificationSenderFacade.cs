using eLib.Common.Notifications;
using eLib.NotificationService.DAL;
using eLib.NotificationService.Notifications;
using eLib.NotificationService.Senders.Email;
using eLib.NotificationService.Senders.SMS;
using eLib.NotificationService.Senders.System;

namespace eLib.NotificationService.Senders;

public class NotificationSenderFacade : INotificationSenderFacade
{
    private readonly ILogger<NotificationSenderFacade> _logger;
    private readonly ISystemNotificationSender _systemNotificationSender;
    private readonly ISMSNotificationSender _smsNotificationSender;
    private readonly IEmailNotificationSender _emailNotificationSender;

    public NotificationSenderFacade(
        ILogger<NotificationSenderFacade> logger,
        ISystemNotificationSender systemNotificationSender,
        ISMSNotificationSender smsNotificationSender,
        IEmailNotificationSender emailNotificationSender)
    {
        _logger = logger;
        _systemNotificationSender = systemNotificationSender;
        _smsNotificationSender = smsNotificationSender;
        _emailNotificationSender = emailNotificationSender;
    }

    public async Task SendAsync(Notification notification, CancellationToken cancellationToken)
    {
        switch (notification.Channel)
        {
            case ENotificationChannel.System:
                await _systemNotificationSender.SendAsync(notification, cancellationToken);
                break;
            case ENotificationChannel.SMS:
                await _smsNotificationSender.SendAsync(notification, cancellationToken);
                break;
            case ENotificationChannel.Email:
                await _emailNotificationSender.SendAsync(notification, cancellationToken);
                break;
            default:
                throw new NotImplementedException();
        }
    }
}

public interface INotificationSenderFacade
{
    Task SendAsync(Notification notification, CancellationToken cancellationToken);
}