using eLib.Events.Events.Notifications;
using eLib.Events.Services;
using MassTransit;

namespace eLib.NotificationService.Consumers;

public class SendSMSNotificationConsumer : IEventConsumer<SendSMSNotificationEvent>
{
    private readonly ILogger<SendNotificationConsumer> _logger;
    private readonly INotificationProcessor _notificationProcessor;

    public SendSMSNotificationConsumer(
        ILogger<SendNotificationConsumer> logger,
        INotificationProcessor notificationProcessor)
    {
        _logger = logger;
        _notificationProcessor = notificationProcessor;
    }


    public async Task Consume(ConsumeContext<SendSMSNotificationEvent> context)
    {
        var eventMessage = context.Message;
        var notificationType = eventMessage.NotificationType;
        await _notificationProcessor.ProcessSMSAsync(notificationType, eventMessage.UserInfo, context.CancellationToken, eventMessage.AssociatedObjects);
    }
}