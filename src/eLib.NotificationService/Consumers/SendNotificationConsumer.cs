using eLib.Events.Events.Notifications;
using eLib.Events.Services;
using MassTransit;

namespace eLib.NotificationService.Consumers;

public class SendNotificationConsumer : IEventConsumer<SendNotificationEvent>
{
    private readonly ILogger<SendNotificationConsumer> _logger;

    public SendNotificationConsumer(
        ILogger<SendNotificationConsumer> logger)
    {
        _logger = logger;
    }

    public async Task Consume(ConsumeContext<SendNotificationEvent> context)
    {
        var notification = context.Message;
        _logger.LogInformation($"Notification sent: {notification.GetType().Name}");
    }
}