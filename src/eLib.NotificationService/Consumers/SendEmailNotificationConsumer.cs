using eLib.Events.Events.Notifications;
using eLib.Events.Services;
using MassTransit;

namespace eLib.NotificationService.Consumers;

public class SendEmailNotificationConsumer : IEventConsumer<SendEmailNotificationEvent>
{
    private readonly ILogger<SendNotificationConsumer> _logger;
    private readonly INotificationProcessor _notificationProcessor;

    public SendEmailNotificationConsumer(
        ILogger<SendNotificationConsumer> logger,
        INotificationProcessor notificationProcessor)
    {
        _logger = logger;
        _notificationProcessor = notificationProcessor;
    }


    public async Task Consume(ConsumeContext<SendEmailNotificationEvent> context)
    {
        var eventMessage = context.Message;
        var notificationType = eventMessage.NotificationType;
        await _notificationProcessor.ProcessEmailAsync(notificationType, eventMessage.UserInfo, context.CancellationToken, eventMessage.AssociatedObjects);
    }
}