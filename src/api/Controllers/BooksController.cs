using eLib.Auth.Security.Attributes;
using eLib.Commands.Book;
using eLib.DAL.Pagination;
using eLib.Queries.Book;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace eLib.Controllers;

[ApiController]
[Route("/api/books")]
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

    [HttpGet("/author/{authorId}")]
    public async Task<IActionResult> GetByAuthorId([FromRoute] Guid authorId, PaginationParameters paginationParameters)
    {
        var result = await _mediator.Send(new GetBooksByAuthorIdQuery(authorId, paginationParameters));
        return OkOrNotFound(result);
    }

    [HttpGet]
    public async Task<IActionResult> GetAll([FromQuery] PaginationParameters paginationParameters)
    {
        var result = await _mediator.Send(new GetAllBooksQuery(paginationParameters));
        return OkOrNotFound(result);
    }

    [HttpPost]
    [AdminOnly]
    public async Task<IActionResult> Create([FromBody] CreateBookCommand command)
    {
        var result = await _mediator.Send(command);
        return CreatedOrBadRequest(result, $"books/{result.Value}");
    }

    [AdminOnly]
    [HttpDelete("{id}")]
    public async Task<IActionResult> Delete([FromRoute] Guid id)
    {
        var result = await _mediator.Send(new DeleteBookCommand(id));
        return NoContentOrBadRequest(result);
    }

    [AdminOnly]
    [HttpPut("{id}")]
    public async Task<IActionResult> Update([FromRoute] Guid id, [FromBody] UpdateBookCommand command)
    {
        command.Id = id;
        var result = await _mediator.Send(command);
        return NoContentOrBadRequest(result);
    }
}
