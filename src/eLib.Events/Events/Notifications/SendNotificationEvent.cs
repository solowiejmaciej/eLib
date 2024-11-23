using eLib.Common;
using eLib.Common.Notifications;

namespace eLib.Events.Events.Notifications;

public class SendNotificationEvent : EventBase
{
    public SendNotificationEvent(ENotificationType notificationType, UserInfo userInfo, IEnumerable<SerializedObject> associatedObjects)
    {
        NotificationType = notificationType;
        EventType = EEventType.Notification;
        UserInfo = userInfo;
        AssociatedObjects = associatedObjects;
    }

    public ENotificationType NotificationType { get; }
    public UserInfo UserInfo { get; }
    public IEnumerable<SerializedObject>? AssociatedObjects { get; }
}