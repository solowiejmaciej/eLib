using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace eLib.NotificationService.Controllers;

[ApiController]
[Route("/api/notifications")]
[Authorize]
public class NotificationsController : ControllerBase
{
    private readonly ILogger<NotificationsController> _logger;

    public NotificationsController(ILogger<NotificationsController> logger)
    {
        _logger = logger;
    }

    [HttpGet]
    public async Task<IActionResult> Get()
    {
        return Ok();
    }
}