using eLib.Commands;
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
    public IActionResult GetById([FromRoute] int id)
    {
        return Ok(id);
    }

    [HttpGet]
    public IActionResult GetAll()
    {
        return Ok();
    }

    [HttpPost]
    public IActionResult Create()
    {
        var command = new CreateBookCommand();
        var result = _mediator.Send(command);
        return Ok(result);
    }
}