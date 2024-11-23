using eLib.Common;
using eLib.Common.Notifications;
using eLib.NotificationService.Notifications;

namespace eLib.NotificationService.Providers;

public class NotificationContentProvider : INotificationContentProvider
{
    public string GetContent(
        ENotificationType notificationType,
        ENotificationChannel notificationChannel,
        IEnumerable<SerializedObject>? associatedObjects)
    {
        var tokenReplacer = new TokenReplacer();
        var contentWithTokens = string.Empty;
        switch (notificationChannel)
        {
            case ENotificationChannel.Email:
                contentWithTokens = GetEmailContent(notificationType);
                break;
            case ENotificationChannel.SMS:
                contentWithTokens = GetSmsContent(notificationType);
                break;
            case ENotificationChannel.System:
                contentWithTokens = GetSystemContent(notificationType);
                break;
            default:
                throw new NotSupportedException($"Channel {notificationChannel} is not supported.");
        }

        foreach (var associatedObject in associatedObjects)
        {
           var newContent = tokenReplacer.ReplaceTokens(contentWithTokens, associatedObject);
           contentWithTokens = newContent;
        }

        return contentWithTokens;
    }

    private string GetSystemContent(ENotificationType notificationType)
    {
        return notificationType switch
        {
            ENotificationType.ReservationCreated => "New reservation has been created. Please remember to return the book before {RESERVATION_ENDDATE}",
            ENotificationType.ReservationCanceled => "Your reservation with ID: {RESERVATION_ID} has been canceled.",
            ENotificationType.ReservationExpired => "Your reservation with ID: {RESERVATION_ID} has expired. Please return the book immediately to avoid penalties.",
            ENotificationType.ReservationReturned => "Reservation with ID: {RESERVATION_ID} has been returned. Thank you!",
            ENotificationType.AccountCreated => "New account created for: {USER_NAME}",
            ENotificationType.AccountDeleted => "Account deleted for user: {USER_NAME}",
            _ => string.Empty
        };
    }

    private string GetSmsContent(ENotificationType notificationType)
    {
        return notificationType switch
        {
            ENotificationType.ReservationCreated => "Your reservation (ID: {RESERVATION_ID}) has been created. Return date: {RESERVATION_ENDDATE}. Thank you!",
            ENotificationType.ReservationCanceled => "Your reservation (ID: {RESERVATION_ID}) has been canceled.",
            ENotificationType.ReservationExpired => "Your reservation (ID: {RESERVATION_ID}) has expired. Please return the book.",
            ENotificationType.ReservationReturned => "Thank you for returning reservation (ID: {RESERVATION_ID}).",
            ENotificationType.AccountCreated => "Welcome {USER_NAME}! Your account has been created",
            ENotificationType.AccountDeleted => "Your account has been deleted. Thank you for using our services, {USER_NAME}.",
            _ => string.Empty
        };
    }

    private string GetEmailContent(ENotificationType notificationType)
    {
        string baseTemplate = @"
    <!DOCTYPE html>
    <html>
    <head>
        <meta charset='UTF-8'>
        <style>
            body {{
                font-family: Arial, sans-serif;
                line-height: 1.6;
                color: #333333;
                max-width: 600px;
                margin: 0 auto;
                padding: 20px;
            }}
            .header {{
                background-color: #004d99;
                color: white;
                padding: 20px;
                text-align: center;
                border-radius: 5px 5px 0 0;
            }}
            .content {{
                background-color: #ffffff;
                padding: 20px;
                border: 1px solid #dddddd;
                border-top: none;
                border-radius: 0 0 5px 5px;
            }}
            .footer {{
                text-align: center;
                margin-top: 20px;
                padding: 20px;
                font-size: 12px;
                color: #666666;
            }}
            .button {{
                display: inline-block;
                padding: 10px 20px;
                background-color: #004d99;
                color: white;
                text-decoration: none;
                border-radius: 3px;
                margin: 10px 0;
            }}
            .important {{
                color: #cc0000;
                font-weight: bold;
            }}
            .details {{
                background-color: #f5f5f5;
                padding: 15px;
                margin: 10px 0;
                border-radius: 3px;
            }}
        </style>
    </head>
    <body>
        <div class='header'>
            <h1>eLib</h1>
        </div>
        <div class='content'>
            {0}
        </div>
        <div class='footer'>
            <p>Best regards,<br>eLib Team</p>
            <p>This is an automated message. Please do not reply to this email address.</p>
        </div>
    </body>
    </html>";

        string specificContent = notificationType switch
        {
            ENotificationType.ReservationCreated => @"
                <h2>Reservation Confirmation</h2>
                <p>Dear {USER_NAME},</p>
                <p>Thank you for making a reservation in the eLib system.</p>
                <div class='details'>
                    <h3>Reservation details:</h3>
                    <p>Reservation number: <strong>{RESERVATION_ID}</strong></p>
                    <p>Creation date: <strong>{RESERVATION_STARTDATE}</strong></p>
                    <p>Expiration date: <strong>{RESERVATION_ENDDATE}</strong></p>
                    <p>Book title: <strong>{BOOK_TITLE}</strong></p>
                </div>",

            ENotificationType.ReservationCanceled => @"
                <h2>Reservation Cancellation</h2>
                <p>Dear {USER_NAME},</p>
                <p>We inform you that reservation no. <strong>{RESERVATION_ID}</strong> for book ""{BOOK_TITLE}"" has been canceled.</p>
                <p class='important'>If this action was not initiated by you, please contact support immediately.</p>",

            ENotificationType.ReservationExpired => @"
                <h2>Reservation Expiration</h2>
                <p>Dear {USER_NAME},</p>
                <p>We remind you that your reservation no. <strong>{RESERVATION_ID}</strong> has expired.</p>
                <p class='important'>Please return ""{BOOK_TITLE}"" as soon as possible to avoid penalties.</p>
                <div class='details'>
                    <p>Expiration date: <strong>{RESERVATION_ENDDATE}</strong></p>
                    <p>Return status: <strong>{RESERVATION_ISRETURNED}</strong></p>
                </div>",

            ENotificationType.ReservationReturned => @"
                <h2>Return Confirmation</h2>
                <p>Dear {USER_NAME},</p>
                <p>We confirm the return of book ""{BOOK_TITLE}"" associated with reservation no. <strong>{RESERVATION_ID}</strong>.</p>
                <p>Thank you for the timely return.</p>",

            ENotificationType.AccountCreated => @"
                <h2>Welcome to eLib!</h2>
                <p>Dear {USER_NAME},</p>
                <p>Your account has been successfully created.</p>",

            ENotificationType.AccountDeleted => @"
                <h2>Account Deletion</h2>
                <p>Dear {USER_NAME},</p>
                <p>We inform you that your eLib account has been deleted.</p>
                <p>Thank you for using our services.</p>
                <p class='important'>If this action was not initiated by you, please contact support immediately.</p>",

            _ => string.Empty
        };

        return string.Format(baseTemplate, specificContent);
    }
}

public interface INotificationContentProvider
{
    string GetContent(
        ENotificationType notificationType,
        ENotificationChannel userInfoNotificationChannel,
        IEnumerable<SerializedObject>? associatedObjects);
}