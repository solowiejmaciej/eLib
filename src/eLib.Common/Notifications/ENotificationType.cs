namespace eLib.Common.Notifications;

public enum ENotificationType
{
    Unknown = 0,
    ReservationCreated = 1,
    ReservationCanceled = 2,
    ReservationExpired = 3,
    ReservationReturned = 4,
    AccountCreated = 5,
    AccountDeleted = 6,
    Custom = 7,
}