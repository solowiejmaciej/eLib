using eLib.Auth.Security.Attributes;
using eLib.NotificationService.Commands;
using eLib.NotificationService.DAL.Pagination;
using eLib.NotificationService.Queries;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace eLib.NotificationService.Controllers;

[ApiController]
[Route("/api/notifications")]
public class NotificationsController : ControllerBase
{
    private readonly ILogger<NotificationsController> _logger;
    private readonly IMediator _mediator;

    public NotificationsController(
        ILogger<NotificationsController> logger,
        IMediator mediator)
    {
        _logger = logger;
        _mediator = mediator;
    }

    [HttpGet]
    // [AdminOnly]
    public async Task<IActionResult> Get([FromQuery] PaginationParameters paginationParameters)
    {
        var notifications = await _mediator.Send(new GetAllNotificationsQuery(paginationParameters));
        return Ok(notifications);
    }

    [HttpGet("{id}")]
    [AdminOrCurrentUser]
    public async Task<IActionResult> GetForUser(Guid id, [FromQuery] PaginationParameters paginationParameters)
    {
        var notifications = await _mediator.Send(new GetNotificationByUserIdQuery(id, paginationParameters));
        return Ok(notifications);
    }

    [HttpPost]
    [AdminOnly]
    public async Task<IActionResult> Create([FromBody] CreateNewNotificationCommand command)
    {
        await _mediator.Send(command);
        return Ok();
    }

    [HttpDelete("{id}")]
    [AdminOnly]
    public async Task<IActionResult> Delete(Guid id)
    {
        await _mediator.Send(new DeleteNotificationCommand(id));
        return NoContent();
    }
}