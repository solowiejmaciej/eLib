using eLib.Common;
using eLib.Common.Notifications;
using eLib.NotificationService.DAL;
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
    private readonly INotificationRepository _notificationRepository;

    public NotificationProcessor(
        ILogger<NotificationProcessor> logger,
        INotificationFactory notificationFactory,
        INotificationSenderFacade notificationSenderFacade,
        INotificationContentProvider notificationContentProvider,
        INotificationRepository notificationRepository)
    {
        _logger = logger;
        _notificationFactory = notificationFactory;
        _notificationSenderFacade = notificationSenderFacade;
        _notificationContentProvider = notificationContentProvider;
        _notificationRepository = notificationRepository;
    }

    public async Task ProcessAsync(
        ENotificationType notificationType,
        UserInfo userInfo,
        CancellationToken cancellationToken,
        IEnumerable<SerializedObject>? associatedObjects)
    {
        var notification = _notificationFactory.Create(
            userInfo,
            _notificationContentProvider.GetContent(notificationType, userInfo.NotificationChannel, associatedObjects),
            notificationType,
            userInfo.NotificationChannel
        );

        await _notificationRepository.AddAsync(notification, cancellationToken);
        await _notificationRepository.SaveChangesAsync(cancellationToken);
        await _notificationSenderFacade.SendAsync(notification, cancellationToken);
        await _notificationRepository.SaveChangesAsync(cancellationToken);
    }

    public async Task ProcessAsync(
        ENotificationType notificationType,
        Guid userId,
        ENotificationChannel notificationChannel,
        string? phoneNumber,
        string? email,
        string message,
        CancellationToken cancellationToken)
    {
        var userInfo = UserInfo.Create(userId, notificationChannel, phoneNumber, email);

        var notification = _notificationFactory.Create(
            userInfo,
            message,
            notificationType,
            userInfo.NotificationChannel
        );

        await _notificationRepository.AddAsync(notification, cancellationToken);
        await _notificationRepository.SaveChangesAsync(cancellationToken);
        await _notificationSenderFacade.SendAsync(notification, cancellationToken);
        await _notificationRepository.SaveChangesAsync(cancellationToken);
    }

    public async Task ProcessEmailAsync(ENotificationType type, UserInfo userInfo, CancellationToken cancellationToken,
        IEnumerable<SerializedObject>? associatedObjects)
    {
        var notification = _notificationFactory.Create(
            userInfo,
            _notificationContentProvider.GetContent(type, ENotificationChannel.Email, associatedObjects),
            type,
            ENotificationChannel.Email
        );

        await _notificationRepository.AddAsync(notification, cancellationToken);
        await _notificationRepository.SaveChangesAsync(cancellationToken);
        await _notificationSenderFacade.SendAsync(notification, cancellationToken);
        await _notificationRepository.SaveChangesAsync(cancellationToken);
    }

    public async Task ProcessSMSAsync(ENotificationType type, UserInfo userInfo, CancellationToken cancellationToken,
        IEnumerable<SerializedObject>? associatedObjects)
    {
        var notification = _notificationFactory.Create(
            userInfo,
            _notificationContentProvider.GetContent(type, ENotificationChannel.SMS, associatedObjects),
            type,
            ENotificationChannel.SMS
        );

        await _notificationRepository.AddAsync(notification, cancellationToken);
        await _notificationRepository.SaveChangesAsync(cancellationToken);
        await _notificationSenderFacade.SendAsync(notification, cancellationToken);
        await _notificationRepository.SaveChangesAsync(cancellationToken);
    }
}

public interface INotificationProcessor
{
    Task ProcessAsync(
        ENotificationType type,
        UserInfo userInfo,
        CancellationToken cancellationToken,
        IEnumerable<SerializedObject>? associatedObjects);

    Task ProcessAsync(
        ENotificationType type,
        Guid userId,
        ENotificationChannel notificationChannel,
        string? phoneNumber,
        string? email,
        string message,
        CancellationToken cancellationToken);

    Task ProcessEmailAsync(
        ENotificationType type,
        UserInfo userInfo,
        CancellationToken cancellationToken,
        IEnumerable<SerializedObject>? associatedObjects);

    Task ProcessSMSAsync(
        ENotificationType type,
        UserInfo userInfo,
        CancellationToken cancellationToken,
        IEnumerable<SerializedObject>? associatedObjects);
}