using eLib.Common;
using eLib.Common.Notifications;
using eLib.NotificationService.DAL;
using eLib.NotificationService.Notifications.Email;
using eLib.NotificationService.Notifications.SMS;
using eLib.NotificationService.Notifications.System;

namespace eLib.NotificationService.Notifications;

public class NotificationFactory : INotificationFactory
{
    public Notification Create(
        UserInfo userInfo,
        string message,
        ENotificationType type,
        ENotificationChannel channel)
    {
        switch (channel)
        {
            case ENotificationChannel.System:
                return SystemNotification.Create(message, type, userInfo);
            case ENotificationChannel.SMS:
                return SMSNotification.Create(message, type, userInfo);
            case ENotificationChannel.Email:
                return EmailNotification.Create(message, type, userInfo);
            default:
                throw new NotSupportedException($"Channel {channel} is not supported.");
        }
    }
}

public interface INotificationFactory
{
    Notification Create(
        UserInfo userInfo,
        string message,
        ENotificationType type,
        ENotificationChannel channel
        );
}