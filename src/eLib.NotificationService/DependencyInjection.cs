using eLib.NotificationService.DAL;
using eLib.NotificationService.Notifications;
using eLib.NotificationService.Providers;
using eLib.NotificationService.Senders;
using eLib.NotificationService.Senders.Email;
using eLib.NotificationService.Senders.SMS;
using eLib.NotificationService.Senders.System;
using Microsoft.EntityFrameworkCore;

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

    public static void UsePostgres(this IServiceCollection services, ConfigurationManager configuration)
    {
        services.AddDbContext<NotificationsDbContext>(options =>
        {
            options.UseNpgsql(configuration.GetConnectionString("DefaultConnection"));
        });
    }

    public static void AddRepositories(this IServiceCollection services, ConfigurationManager configuration)
    {
        services.AddScoped<INotificationRepository, NotificationRepository>();
    }

    public static void AddAutomaticMigrations(this IServiceCollection services)
    {
        using var scope = services.BuildServiceProvider().CreateScope();
        var context = scope.ServiceProvider.GetRequiredService<NotificationsDbContext>();
        context.Database.Migrate();
    }
}
