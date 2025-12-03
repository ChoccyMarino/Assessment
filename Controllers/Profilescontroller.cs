using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Assessment.Features.Profiles;
using System.Security.Claims;

namespace Assessment.Controllers;

[ApiController]
[Route("api/profile")]
[Authorize] // all endpoints in this controller require authentication

public class ProfilesController : ControllerBase
{
    private readonly IMediator _mediator;
     public ProfilesController (IMediator mediator)
    {
        _mediator = mediator;
    }

    [HttpGet]
    public async Task<IActionResult> GetProfile()
    {
        var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;

        if (userId == null)
        {
            return Unauthorized(new { message = "User not authenticated" });
        }

        var query = new GetProfileQuery
        {
            UserId = int.Parse(userId)
        };

        var result = await _mediator.Send(query);

        if (result.Success)
        {
            return Ok(result);
        }

        if (result.Message.Contains("not found", StringComparison.OrdinalIgnoreCase))
        {
            return NotFound(result);
        }

        return BadRequest(result);
    }

    [HttpPut]
    public async Task<IActionResult> UpdateProfile([FromBody] UpdateProfileCommand command)
    {
        var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;

        if (userId == null)
        {
            return Unauthorized(new { message = "User not authenticated" });
        }

        command.UserId = int.Parse(userId);
        // if the command is valid, send it to the mediator

        var result = await _mediator.Send(command);

        if (result.Success)
        {
            return Ok(result);
        }

        return BadRequest(result);
    }
}
