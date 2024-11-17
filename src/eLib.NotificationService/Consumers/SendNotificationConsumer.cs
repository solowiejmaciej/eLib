using eLib.Events.Events.Notifications;
using eLib.Events.Services;
using eLib.NotificationService.Notifications;
using eLib.NotificationService.Providers;
using eLib.NotificationService.Senders;
using MassTransit;

namespace eLib.NotificationService.Consumers;

public class SendNotificationConsumer : IEventConsumer<SendNotificationEvent>
{
    private readonly ILogger<SendNotificationConsumer> _logger;
    private readonly INotificationProcessor _notificationProcessor;

    public SendNotificationConsumer(
        ILogger<SendNotificationConsumer> logger,
        INotificationProcessor notificationProcessor)
    {
        _logger = logger;
        _notificationProcessor = notificationProcessor;
    }


    public async Task Consume(ConsumeContext<SendNotificationEvent> context)
    {
        var eventMessage = context.Message;
        var notificationType = eventMessage.NotificationType;
        await _notificationProcessor.ProcessAsync(notificationType, eventMessage.UserInfo, context.CancellationToken);
    }
}