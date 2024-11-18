using eLib.NotificationService.Notifications;
using eLib.NotificationService.Providers;
using eLib.NotificationService.Senders;
using eLib.NotificationService.Senders.Email;
using eLib.NotificationService.Senders.SMS;
using eLib.NotificationService.Senders.System;

namespace eLib.NotificationService;

public static class ServiceCollectionExtensions
{
    public static void AddServices(this IServiceCollection services, ConfigurationManager configuration)
    {
        services.AddScoped<INotificationFactory, NotificationFactory>();
        services.AddScoped<ISMSNotificationSender, SMSNotificationSender>();
        services.AddScoped<ISystemNotificationSender, SystemNotificationSender>();
        services.AddScoped<INotificationContentProvider, NotificationContentProvider>();
        services.AddScoped<IEmailNotificationSender, EmailNotificationSender>();
        services.AddScoped<INotificationProcessor, NotificationProcessor>();
        services.AddScoped<INotificationSenderFacade, NotificationSenderFacade>();
    }
}
