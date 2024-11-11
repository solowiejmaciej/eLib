using eLib.Commands;
using eLib.Commands.Book;
using eLib.Queries;
using eLib.Queries.Book;
using MediatR;
using Microsoft.AspNetCore.Mvc;

namespace eLib.Controllers;

[ApiController]
[Route("books")]
public class BooksController : BaseController
{
    private readonly IMediator _mediator;

    public BooksController(IMediator mediator)
    {
        _mediator = mediator;
    }

    [HttpGet("{id}")]
    public async Task<IActionResult> GetById([FromRoute] Guid id)
    {
        var result = await _mediator.Send(new GetBookByIdQuery(id));
        return OkOrNotFound(result);
    }

    [HttpGet]
    public async Task<IActionResult> GetAll()
    {
        var result = await _mediator.Send(new GetAllBooksQuery());
        return OkOrBadRequest(result);
    }

    [HttpPost]
    public async Task<IActionResult> Create([FromBody] CreateBookCommand command)
    {
        var result = await _mediator.Send(command);
        return CreatedOrBadRequest(result, $"books/{result.Value}");
    }

    [HttpDelete("{id}")]
    public async Task<IActionResult> Delete([FromRoute] Guid id)
    {
        var result = await _mediator.Send(new DeleteBookCommand(id));
        return NoContentOrBadRequest(result);
    }

    [HttpPut("{id}")]
    public async Task<IActionResult> Update([FromRoute] Guid id, [FromBody] UpdateBookCommand command)
    {
        command.Id = id;
        var result = await _mediator.Send(command);
        return NoContentOrBadRequest(result);
    }
}
