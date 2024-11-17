using eLib.Common;
using eLib.Common.Notifications;
using eLib.NotificationService.Notifications.Email;
using eLib.NotificationService.Notifications.SMS;
using eLib.NotificationService.Notifications.System;

namespace eLib.NotificationService.Notifications;

public class NotificationFactory : INotificationFactory
{
    public INotification Create(
        UserInfo userInfo,
        string message,
        ENotificationType type,
        ENotificationChannel channel)
    {
        switch (channel)
        {
            case ENotificationChannel.System:
                return SystemNotification.Create(userInfo.Id, message, type);
            case ENotificationChannel.SMS:
                return SMSNotification.Create(userInfo.Id, message, type);
            case ENotificationChannel.Email:
                return EmailNotification.Create(userInfo.Id, message, type);
            default:
                throw new NotSupportedException($"Channel {channel} is not supported.");
        }
    }
}

public interface INotificationFactory
{
    INotification Create(
        UserInfo userInfo,
        string message,
        ENotificationType type,
        ENotificationChannel channel
        );
}