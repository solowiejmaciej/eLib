using eLib.Commands.Reservation;
using eLib.Queries.Reservation;
using eLib.Security.Attributes;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace eLib.Controllers;

[ApiController]
[Route("reservations")]
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
    public async Task<IActionResult> GetAll()
    {
        var result = await _mediator.Send(new GetAllReservationsQuery());
        return OkOrBadRequest(result);
    }

    [HttpPost]
    public async Task<IActionResult> Create([FromBody] CreateReservationCommand command)
    {
        var result = await _mediator.Send(command);
        return CreatedOrBadRequest(result, $"reservations/{result.Value}");
    }

    [HttpPost("{reservationId}/cancel")]
    public async Task<IActionResult> Cancel([FromRoute] Guid id)
    {
        var result = await _mediator.Send(new CancelReservationCommand(id));
        return NoContentOrBadRequest(result);
    }

    [HttpPost("{reservationId}/extend")]
    public async Task<IActionResult> Extend([FromRoute] Guid id, [FromBody] ExtendReservationCommand command)
    {
        command.Id = id;
        var result = await _mediator.Send(command);
        return NoContentOrBadRequest(result);
    }

    [HttpPost("{reservationId}/return")]
    public async Task<IActionResult> Return([FromRoute] Guid id)
    {
        var result = await _mediator.Send(new ReturnReservationCommand(id));
        return NoContentOrBadRequest(result);
    }

    [HttpGet("user/{userId}")]
    public async Task<IActionResult> GetByUser([FromRoute] Guid userId)
    {
        var result = await _mediator.Send(new GetReservationsByUserIdQuery(userId));
        return OkOrNotFound(result);
    }


}