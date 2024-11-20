using eLib.Common;
using eLib.Common.Notifications;
using eLib.NotificationService.Notifications;
using eLib.NotificationService.Providers;
using eLib.NotificationService.Senders;

namespace eLib.NotificationService;

public class NotificationProcessor : INotificationProcessor
{
    private readonly ILogger<NotificationProcessor> _logger;
    private readonly INotificationFactory _notificationFactory;
    private readonly INotificationSenderFacade _notificationSenderFacade;
    private readonly INotificationContentProvider _notificationContentProvider;

    public NotificationProcessor(
        ILogger<NotificationProcessor> logger,
        INotificationFactory notificationFactory,
        INotificationSenderFacade notificationSenderFacade,
        INotificationContentProvider notificationContentProvider)
    {
        _logger = logger;
        _notificationFactory = notificationFactory;
        _notificationSenderFacade = notificationSenderFacade;
        _notificationContentProvider = notificationContentProvider;
    }

    public async Task ProcessAsync(ENotificationType notificationType, UserInfo userInfo, CancellationToken cancellationToken)
    {
        var notification = _notificationFactory.Create(
            userInfo,
            _notificationContentProvider.GetContent(notificationType),
            notificationType,
            userInfo.NotificationChannel
        );

        await _notificationSenderFacade.SendAsync(notification, cancellationToken);
    }
}

public interface INotificationProcessor
{
    Task ProcessAsync(ENotificationType type, UserInfo userInfo, CancellationToken cancellationToken);
}