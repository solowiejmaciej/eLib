using eLib.Models.Results.Base;
using Microsoft.AspNetCore.Mvc;

namespace eLib.Controllers;

public class BaseController : ControllerBase
{
    public IActionResult OkOrBadRequest<T>(Result<T,Error> result)
    {
        if (result.HasError())
        {
            return BadRequest(result.Error);
        }
        return Ok(result.Value);
    }

    public IActionResult CreatedOrBadRequest<T>(Result<T,Error> result, string location)
    {
        if (result.HasError())
        {
            return BadRequest(result.Error);
        }
        return Created(location, null);
    }

    public IActionResult OkOrNotFound<T>(Result<T,Error> result)
    {
        if (result.HasError())
        {
            return NotFound(result.Error);
        }
        return Ok(result.Value);
    }

    public IActionResult NoContentOrBadRequest<T>(Result<T,Error> result)
    {
        if (result.HasError())
        {
            return BadRequest(result.Error);
        }
        return NoContent();
    }
}