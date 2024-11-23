using eLib.NotificationService.DAL;
using RestSharp;

namespace eLib.NotificationService.Senders.SMS;

public class SMSNotificationSender : ISMSNotificationSender
{
    private readonly ILogger<SMSNotificationSender> _logger;
    private readonly IConfiguration _configuration;

    public SMSNotificationSender(
        ILogger<SMSNotificationSender> logger,
        IConfiguration configuration)
    {
        _logger = logger;
        _configuration = configuration;
    }

    public async Task SendAsync(Notification notification, CancellationToken cancellationToken)
    {
        _logger.LogInformation($"Sending sms notification: {notification.Id}");

        var baseUrl = _configuration["SMSSettings:BaseUrl"];
        var key = _configuration["SMSSettings:Key"];
        var password = _configuration["SMSSettings:Password"];
        var sendername = _configuration["SMSSettings:SenderName"];

        var options = new RestClientOptions(baseUrl);
        var client = new RestClient(options);
        var request = new RestRequest("/sms", Method.Post);

        _logger.LogInformation("Creating request");

        request.AddHeader("Content-Type", "application/x-www-form-urlencoded");
        request.AddParameter("key", key);
        request.AddParameter("password", password);
        request.AddParameter("from", sendername);
        request.AddParameter("to", notification.Details.PhoneNumber);
        request.AddParameter("msg", notification.Message);

        var response = await client.ExecuteAsync<ErrorResponse>(request, cancellationToken);
        _logger.LogInformation($"Request fired to {baseUrl}");

        var data = response.Data;

        if (data.errorMsg is not null)
            notification.MarkAsFailed();
        else
            notification.MarkAsSent();
    }

    private class ErrorResponse
    {
        public int errorCode { get; }
        public string errorMsg { get; }
    }
}

public interface ISMSNotificationSender : INotificationSender;