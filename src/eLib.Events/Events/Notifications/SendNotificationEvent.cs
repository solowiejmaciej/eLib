using eLib.Common;
using eLib.Common.Notifications;

namespace eLib.Events.Events.Notifications;

public class SendNotificationEvent : EventBase
{
    public SendNotificationEvent(ENotificationType notificationType, UserInfo userInfo)
    {
        NotificationType = notificationType;
        EventType = EEventType.Notification;
        UserInfo = userInfo;
    }

    public ENotificationType NotificationType { get; }
    public UserInfo UserInfo { get; }
}