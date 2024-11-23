using eLib.Common;
using eLib.Common.Dtos;

namespace eLib.NotificationService.Notifications;

public class TokenReplacer
{
    public string ReplaceTokens(string message, SerializedObject associatedObject)
    {
        switch (associatedObject.Type)
        {
            case nameof(UserInfo):
                return ReplaceTokens(message, associatedObject.Deserialize<UserInfo>());
            case nameof(ReservationDto):
                return ReplaceTokens(message, associatedObject.Deserialize<ReservationDto>());
            case nameof(BookDto):
                return ReplaceTokens(message, associatedObject.Deserialize<BookDto>());
            case nameof(TwoStepCodeDto):
                return ReplaceTokens(message, associatedObject.Deserialize<TwoStepCodeDto>());
            default:
                return message;
        }
    }

    private string ReplaceTokens(string message, TwoStepCodeDto associatedObject)
    {
        var result = message
            .Replace("{CODE_CODE}", associatedObject.Code)
            .Replace("{CODE_EXPIRES_AT}", associatedObject.ExpiresAt.AddHours(1).ToString("dd/MM/yyyy HH:mm"));

        return result;
    }

    private string ReplaceTokens(string message, UserInfo userInfo)
    {
        var result = message
            .Replace("{USER_NAME}", userInfo.Name)
            .Replace("{USER_SURNAME}", userInfo.Surname)
            .Replace("{USER_PHONENUMBER}", userInfo.PhoneNumber)
            .Replace("{USER_EMAIL}", userInfo.Email);

        return result;
    }

    private string ReplaceTokens(string message, ReservationDto reservation)
    {
        var result = message
            .Replace("{RESERVATION_ID}", reservation.Id.ToString())
            .Replace("{RESERVATION_STARTDATE}", reservation.StartDate.ToString("dd/MM/yyyy"))
            .Replace("{RESERVATION_ENDDATE}", reservation.EndDate?.ToString("dd/MM/yyyy") ?? "N/A")
            .Replace("{RESERVATION_RETURNEDAT}", reservation.ReturnedAt?.ToString("dd/MM/yyyy") ?? "N/A")
            .Replace("{RESERVATION_CANCELEDAT}", reservation.CanceledAt?.ToString("dd/MM/yyyy") ?? "N/A")
            .Replace("{RESERVATION_EXTENDEDAT}", reservation.ExtendedAt?.ToString("dd/MM/yyyy") ?? "N/A")
            .Replace("{RESERVATION_STATUS}", reservation.Status.ToString())
            .Replace("{RESERVATION_ISOVERDUE}", reservation.IsOverdue.ToString())
            .Replace("{RESERVATION_ISRETURNED}", reservation.IsReturned.ToString())
            .Replace("{RESERVATION_ISEXTENDED}", reservation.IsExtended.ToString())
            .Replace("{RESERVATION_ISACTIVE}", reservation.IsActive.ToString());

        return result;
    }

    private string ReplaceTokens(string message, BookDto book)
    {
        var result = message
            .Replace("{BOOK_ID}", book.Id.ToString())
            .Replace("{BOOK_TITLE}", book.Title)
            .Replace("{BOOK_AUTHORID}", book.AuthorId.ToString())
            .Replace("{BOOK_DESCRIPTION}", book.Details?.Description ?? "N/A")
            .Replace("{BOOK_COVERURL}", book.Details?.CoverUrl ?? "N/A")
            .Replace("{BOOK_QUANTITY}", book.Details?.Quantity.ToString() ?? "N/A");

        return result;
    }
}