using eLib.Auth.Providers;
using eLib.Auth.Security.Attributes;
using eLib.Commands.User;
using eLib.Common.Notifications;
using eLib.Events.Events.Notifications;
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
    private readonly IEventPublisher _eventPublisher;
    private readonly IUserInfoProvider _userInfoProvider;

    public UsersController(IMediator mediator, IEventPublisher eventPublisher, IUserInfoProvider userInfoProvider)
    {
        _mediator = mediator;
        _eventPublisher = eventPublisher;
        _userInfoProvider = userInfoProvider;
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
    public async Task<IActionResult> GetAll()
    {
        var result = await _mediator.Send(new GetAllUsersQuery());
        return OkOrBadRequest(result);
    }

    [HttpPost]
    public async Task<IActionResult> Create([FromBody] CreateUserCommand command)
    {
        var result = await _mediator.Send(command);
        return CreatedOrBadRequest(result, $"users/{result.Value}");
    }

    [HttpDelete("{id}")]
    [AdminOrCurrentUser]
    public async Task<IActionResult> Delete([FromRoute] Guid id)
    {
        var result = await _mediator.Send(new DeleteUserCommand(id));
        return NoContentOrBadRequest(result);
    }

    [HttpPut("{id}")]
    [AdminOrCurrentUser]
    public async Task<IActionResult> Update([FromRoute] Guid id, [FromBody] UpdateUserCommand command)
    {
        command.Id = id;
        var result = await _mediator.Send(command);
        return NoContentOrBadRequest(result);
    }

    [HttpPost("test")]
    [Authorize]
    public async Task<IActionResult> Test()
    {
        var userInfo = _userInfoProvider.GetCurrentUser();
        await _eventPublisher.PublishAsync(new SendNotificationEvent(ENotificationType.ReservationCanceled, userInfo), CancellationToken.None);
        return Ok();
    }
}