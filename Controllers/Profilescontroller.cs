using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Assessment.Features.Profiles;
using FluentValidation;
using System.Security.Claims;
using Microsoft.AspNetCore.Http.HttpResults;

namespace Assessment.Controllers;

[ApiController]
[Route("api/[controller]")]
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
        try
        {
            // Get userId from token
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
            
            return BadRequest(result);
        }
        catch (Exception ex)
        {
            return BadRequest(new { message = "Failed to retrieve profile", error = ex.Message });
        }
    }

    [HttpPut]
    public async Task<IActionResult> UpdateProfile([FromBody] UpdateProfileCommand command)
    {
        try
        {
            // get the user id from the token
            var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;

            if (userId == null)
            {
                return Unauthorized(new { message = "User not authenticated" });
            }

            // set user Id from token
            command.UserId = int.Parse(userId);

            var result = await _mediator.Send(command);
            
            if (result.Success)
            {
                return Ok(result);
            }
            
            return BadRequest(result);
        }
        catch (ValidationException ex)
        {
            var errors = ex.Errors.Select(e => new { e.PropertyName, e.ErrorMessage });
            return BadRequest(new {errors});
        }
    }
}