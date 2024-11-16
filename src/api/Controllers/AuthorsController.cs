using eLib.Commands.Author;
using eLib.Queries.Author;
using eLib.Security.Attributes;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace eLib.Controllers;

[Authorize]
[ApiController]
[Route("/api/authors")]
public class AuthorsController : BaseController
{
    private readonly IMediator _mediator;

    public AuthorsController(IMediator mediator)
    {
        _mediator = mediator;
    }

    [HttpGet("{id}")]
    public async Task<IActionResult> GetById([FromRoute] Guid id)
    {
        var result = await _mediator.Send(new GetAuthorByIdQuery(id));
        return OkOrNotFound(result);
    }

    [HttpGet]
    public async Task<IActionResult> GetAll()
    {
        var result = await _mediator.Send(new GetAllAuthorsQuery());
        return OkOrBadRequest(result);
    }

    [HttpPost]
    [AdminOnly]
    public async Task<IActionResult> Create([FromBody] CreateAuthorCommand command)
    {
        var result = await _mediator.Send(command);
        return CreatedOrBadRequest(result, $"authors/{result.Value}");
    }

    [HttpDelete("{id}")]
    [AdminOnly]
    public async Task<IActionResult> Delete([FromRoute] Guid id)
    {
        var result = await _mediator.Send(new DeleteAuthorCommand(id));
        return NoContentOrBadRequest(result);
    }

    [HttpPut("{id}")]
    [AdminOnly]
    public async Task<IActionResult> Update([FromRoute] Guid id, [FromBody] UpdateAuthorCommand command)
    {
        command.Id = id;
        var result = await _mediator.Send(command);
        return NoContentOrBadRequest(result);
    }
}