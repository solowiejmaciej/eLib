using eLib.Auth.Security.Attributes;
using eLib.Commands.Reviews;
using eLib.DAL.Pagination;
using eLib.Queries.Reviews;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace eLib.Controllers;

[ApiController]
[Route("/api/reviews")]
public class ReviewsController : BaseController
{
    private readonly IMediator _mediator;

    public ReviewsController(IMediator mediator)
    {
        _mediator = mediator;
    }

    [AdminOrCurrentUser]
    [HttpGet("user/{id}")]
    public async Task<IActionResult> GetByUserId([FromRoute] Guid id, [FromQuery] PaginationParameters paginationParameters)
    {
        var result = await _mediator.Send(new GetReviewsByUserId(id, paginationParameters));
        return OkOrNotFound(result);
    }

    [HttpGet("book/{id}")]
    public async Task<IActionResult> GetByBookId([FromRoute] Guid id, [FromQuery] PaginationParameters paginationParameters)
    {
        var result = await _mediator.Send(new GetReviewsByBookId(id, paginationParameters));
        return OkOrNotFound(result);
    }

    [HttpPost]
    [Authorize]
    public async Task<IActionResult> Create([FromBody] CreateReviewCommand command)
    {
        var result = await _mediator.Send(command);
        return CreatedOrBadRequest(result, $"reservations/{result.Value}");
    }

    [HttpPut("{id}")]
    [Authorize]
    public async Task<IActionResult> Update([FromRoute] Guid id, [FromBody] UpdateReviewCommand command)
    {
        command.Id = id;
        var result = await _mediator.Send(command);
        return NoContentOrBadRequest(result);
    }

    [HttpDelete("{id}")]
    [Authorize]
    public async Task<IActionResult> Delete([FromRoute] Guid id)
    {
        var result = await _mediator.Send(new DeleteReviewCommand(id));
        return NoContentOrBadRequest(result);
    }
}