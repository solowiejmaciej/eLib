using eLib.Commands;
using eLib.Queries;
using MediatR;
using Microsoft.AspNetCore.Mvc;

namespace eLib.Controllers;

[ApiController]
[Route("authors")]
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
        var result = await _mediator.Send(new GetAuthorById(id));
        return OkOrNotFound(result);
    }

    [HttpGet]
    public async Task<IActionResult> GetAll()
    {
        var result = await _mediator.Send(new GetAllAuthors());
        return OkOrBadRequest(result);
    }

    [HttpPost]
    public async Task<IActionResult> Create([FromBody] CreateAuthorCommand command)
    {
        var result = await _mediator.Send(command);
        return CreatedOrBadRequest(result, $"authors/{result.Value}");
    }

    [HttpDelete("{id}")]
    public async Task<IActionResult> Delete([FromRoute] Guid id)
    {
        var result = await _mediator.Send(new DeleteAuthorCommand(id));
        return NoContentOrBadRequest(result);
    }

    [HttpPut("{id}")]
    public async Task<IActionResult> Update([FromRoute] Guid id, [FromBody] UpdateAuthorCommand command)
    {
        command.Id = id;
        var result = await _mediator.Send(command);
        return NoContentOrBadRequest(result);
    }
}