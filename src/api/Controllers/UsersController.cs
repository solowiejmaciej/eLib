using eLib.Auth.Providers;
using eLib.Auth.Security.Attributes;
using eLib.Commands.User;
using eLib.DAL.Pagination;
using eLib.Events.Services;
using eLib.Queries.User;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace eLib.Controllers;

[ApiController]
[Route("/api/users")]
public class UsersController : BaseController
{
    private readonly IMediator _mediator;

    public UsersController(IMediator mediator)
    {
        _mediator = mediator;
    }

    [HttpGet("{id}")]
    [AdminOrCurrentUser]
    public async Task<IActionResult> GetById([FromRoute] Guid id)
    {
        var result = await _mediator.Send(new GetUserByIdQuery(id));
        return OkOrNotFound(result);
    }

    [HttpGet]
    [AdminOnly]
    public async Task<IActionResult> GetAll([FromQuery] PaginationParameters paginationParameters)
    {
        var result = await _mediator.Send(new GetAllUsersQuery(paginationParameters));
        return OkOrBadRequest(result);
    }

    [HttpPost]
    public async Task<IActionResult> Create([FromBody] CreateUserCommand command)
    {
        var result = await _mediator.Send(command);
        return CreatedOrBadRequest(result, $"users/{result.Value}");
    }

    [HttpDelete("{userId}")]
    [AdminOrCurrentUser]
    public async Task<IActionResult> Delete([FromRoute] Guid userId)
    {
        var result = await _mediator.Send(new DeleteUserCommand(userId));
        return NoContentOrBadRequest(result);
    }

    [HttpPut("{userId}")]
    [AdminOrCurrentUser]
    public async Task<IActionResult> Update([FromRoute] Guid userId, [FromBody] UpdateUserCommand command)
    {
        command.Id = userId;
        var result = await _mediator.Send(command);
        return NoContentOrBadRequest(result);
    }

    [HttpPost("change-password")]
    [Authorize]
    public async Task<IActionResult> ChangePassword([FromBody] ChangePasswordCommand command)
    {
        var result = await _mediator.Send(command);
        return NoContentOrBadRequest(result);
    }

    [HttpPost("send-confirm-email")]
    [Authorize]
    public async Task<IActionResult> ConfirmEmail([FromBody] SendConfirmEmailCommand command)
    {
        var result = await _mediator.Send(command);
        return NoContentOrBadRequest(result);
    }

    [HttpPost("send-confirm-phone-number")]
    [Authorize]
    public async Task<IActionResult> ConfirmPhoneNumber([FromBody] SendConfirmPhoneNumberCommand command)
    {
        var result = await _mediator.Send(command);
        return NoContentOrBadRequest(result);
    }

    [HttpPost("confirm-phone-number")]
    [Authorize]
    public async Task<IActionResult> ConfirmPhoneNumber([FromBody] ConfirmPhoneNumberCommand command)
    {
        var result = await _mediator.Send(command);
        return NoContentOrBadRequest(result);
    }

    [HttpPost("confirm-email")]
    [Authorize]
    public async Task<IActionResult> ConfirmEmail([FromBody] ConfirmEmailCommand command)
    {
        var result = await _mediator.Send(command);
        return NoContentOrBadRequest(result);
    }

    [HttpPut("{userId}/change-notification-channel")]
    [Authorize]
    public async Task<IActionResult> ChangeNotificationChannel([FromBody] ChangeNotificationChannelCommand command, [FromRoute] Guid userId)
    {
        command.UserId = userId;
        var result = await _mediator.Send(command);
        return NoContentOrBadRequest(result);
    }
}