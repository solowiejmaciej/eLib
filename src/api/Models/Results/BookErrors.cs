using eLib.Models.Results.Base;
using MediatR;

namespace eLib.Models.Results;

public class BookErrors
{
    public static readonly Error NotFound =  new(ErrorCodes.NotFound,
        $"Book not found");

    public static readonly Error NoAvailableCopies = new(ErrorCodes.InvalidOperation,
        $"No available copies");

    public static readonly Error HasReservations = new(ErrorCodes.InvalidOperation,
        $"Can't delete book with reservations");}