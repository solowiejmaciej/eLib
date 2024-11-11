using eLib.Commands.Token;
using MediatR;
using Microsoft.AspNetCore.Mvc;

namespace eLib.Controllers;

[ApiController]
[Route("tokens")]
public class TokenController : BaseController
{
    private readonly IMediator _mediator;

    public TokenController(IMediator mediator)
    {
        _mediator = mediator;
    }

    [HttpPost("email")]
    public async Task<IActionResult> CreateFromEmail([FromBody] CreateTokenFromEmailCommand command)
    {
        var result = await _mediator.Send(command);
        return OkOrBadRequest(result);
    }

    [HttpPost("phone-number")]
    [HttpPost]
    public async Task<IActionResult> CreateFromPhoneNumber([FromBody] CreateTokenFromPhoneNumberCommand command)
    {
        var result = await _mediator.Send(command);
        return OkOrBadRequest(result);
    }

    // [HttpPost]
    // public async Task<IActionResult> CreateFromRefreshToken([FromBody] CreateTokenFromRefreshTokenCommand command)
    // {
    //     var result = await _mediator.Send(command);
    //     return OkOrBadRequest(result);
    // }
}