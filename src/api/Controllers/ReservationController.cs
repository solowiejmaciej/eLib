using eLib.Auth.Security.Attributes;
using eLib.Commands.Reservation;
using eLib.DAL.Pagination;
using eLib.Queries.Reservation;
using MediatR;
using Microsoft.AspNetCore.Mvc;

namespace eLib.Controllers;

[ApiController]
[Route("/api/reservations")]
[AdminOrCurrentUser]
public class ReservationController : BaseController
{
    private readonly IMediator _mediator;

    public ReservationController(IMediator mediator)
    {
        _mediator = mediator;
    }

    [HttpGet("{id}")]
    public async Task<IActionResult> GetById([FromRoute] Guid id)
    {
        var result = await _mediator.Send(new GetReservationByIdQuery(id));
        return OkOrNotFound(result);
    }

    [HttpGet]
    public async Task<IActionResult> GetAll([FromQuery] PaginationParameters paginationParameters)
    {
        var result = await _mediator.Send(new GetAllReservationsQuery(paginationParameters));
        return OkOrBadRequest(result);
    }

    [HttpPost]
    public async Task<IActionResult> Create([FromBody] CreateReservationCommand command)
    {
        var result = await _mediator.Send(command);
        return CreatedOrBadRequest(result, $"reservations/{result.Value}");
    }

    [HttpPost("{reservationId}/cancel")]
    public async Task<IActionResult> Cancel([FromRoute] Guid reservationId)
    {
        var result = await _mediator.Send(new CancelReservationCommand(reservationId));
        return NoContentOrBadRequest(result);
    }

    [HttpPost("{reservationId}/extend")]
    public async Task<IActionResult> Extend([FromRoute] Guid reservationId, [FromBody] ExtendReservationCommand command)
    {
        command.Id = reservationId;
        var result = await _mediator.Send(command);
        return NoContentOrBadRequest(result);
    }

    [HttpPost("{reservationId}/return")]
    public async Task<IActionResult> Return([FromRoute] Guid reservationId)
    {
        var result = await _mediator.Send(new ReturnReservationCommand(reservationId));
        return NoContentOrBadRequest(result);
    }

    [HttpGet("user/{userId}")]
    public async Task<IActionResult> GetByUser([FromRoute] Guid userId, [FromQuery] PaginationParameters paginationParameters)
    {
        var result = await _mediator.Send(new GetReservationsByUserIdQuery(userId, paginationParameters));
        return OkOrNotFound(result);
    }
}