using eLib.Auth.Providers;
using eLib.Auth.Security.Attributes;
using eLib.Commands.User;
using eLib.Events.Services;
using eLib.Queries.User;
using MediatR;
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
}