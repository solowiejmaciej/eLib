using eLib.Auth.Security.Attributes;
using eLib.Commands.ReadingList;
using eLib.DAL.Pagination;
using eLib.Queries.ReadingList;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace eLib.Controllers;

[ApiController]
[Route("/api/reading-list")]
public class ReadingListController : BaseController
{
    private readonly IMediator _mediator;

    public ReadingListController(IMediator mediator)
    {
        _mediator = mediator;
    }

    [HttpGet("{userId}")]
    [AdminOrCurrentUser]
    public async Task<IActionResult> GetReadingList([FromRoute] Guid userId, [FromQuery] PaginationParameters paginationParameters)
    {
        var result = await _mediator.Send(new GetReadingListForUserQuery(userId, paginationParameters));
        return OkOrNotFound(result);
    }

    [HttpPost("{bookId}")]
    [Authorize]
    public async Task<IActionResult> AddBookToReadingList([FromRoute] Guid bookId)
    {
        var result = await _mediator.Send(new AddBookToReadingListCommand(bookId));
        return OkOrBadRequest(result);
    }

    [HttpDelete("{bookId}")]
    [Authorize]
    public async Task<IActionResult> RemoveBookFromReadingList([FromRoute] Guid bookId)
    {
        var result = await _mediator.Send(new RemoveBookFromReadingListCommand(bookId));
        return OkOrNotFound(result);
    }

    [HttpGet("{bookId}/exists")]
    [Authorize]
    public async Task<IActionResult> ExistsInReadingList([FromRoute] Guid bookId)
    {
        var result = await _mediator.Send(new ExistsInReadingListQuery(bookId));
        return OkOrNotFound(result);
    }

    [HttpPut("{bookId}/progress")]
    [Authorize]
    public async Task<IActionResult> UpdateProgress([FromRoute] Guid bookId, [FromBody] UpdateProgressCommand command)
    {
        command.BookId = bookId;
        var result = await _mediator.Send(command);
        return OkOrNotFound(result);
    }

    [HttpPost("{bookId}/mark-as-finished")]
    [Authorize]
    public async Task<IActionResult> MarkAsFinished([FromRoute] Guid bookId)
    {
        var result = await _mediator.Send(new MarkAsFinishedCommand(bookId));
        return OkOrBadRequest(result);
    }

    [HttpPost("{bookId}/mark-as-unfinished/")]
    [Authorize]
    public async Task<IActionResult> MarkAsUnfinished([FromRoute] Guid bookId)
    {
        var result = await _mediator.Send(new MarkAsUnfinishedCommand(bookId));
        return OkOrBadRequest(result);
    }


}