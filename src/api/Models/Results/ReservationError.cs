using eLib.DAL.Entities;
using eLib.Models.Dtos;
using eLib.Models.Results.Base;
using MediatR;

namespace eLib.Models.Results;

public class ReservationError
{
    public static readonly Error NotFound = new(ErrorCodes.NotFound,
        $"Reservation not found");

    public static readonly Error InvalidEndDate = new(ErrorCodes.InvalidData,
        $"New end date must be greater than current end date");

    public static readonly Error Returned = new(ErrorCodes.InvalidOperation,
        $"This reservation has already been returned");

    public static readonly Error Canceled = new(ErrorCodes.InvalidOperation,
        $"This reservation has already been canceled");

    public static readonly Error BookNotAvailable = new(ErrorCodes.InvalidData,
        $"This book is not available for reservation");
}