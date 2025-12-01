using MediatR;
using Microsoft.AspNetCore.Mvc;
using Assessment.Features.Auth;
using FluentValidation;
using Microsoft.AspNetCore.Authorization;
namespace Assessment.Controllers;


[ApiController] // API Controller                                                          
[Route("api/[controller]")] //url: api/auth
public class AuthController : ControllerBase
{
    private readonly IMediator _mediator; //uses MediatR to send commands
    public AuthController(IMediator mediator) //ASP.NET gives me IMediator
    {
        _mediator = mediator; // saves it so i can use it later
    }



    [HttpPost("register")]
    public async Task<IActionResult> Register([FromBody] RegisterCommand command)
    {
        try
        {
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
            return BadRequest(new { errors });
        }
    }

    [HttpPost("login")]
    public async Task<IActionResult> Login([FromBody] LoginCommand command)
        {

            var result = await _mediator.Send(command);

            if (result.Success)
            {
                return Ok(result);
            }

            return Unauthorized(result); // 401 unauthorized (not bad request) because
            // the user is not authenticated, not because the request is invalid
        }

    [HttpGet("profile")]
    [Authorize]  // This means "must be logged in"
    public IActionResult GetProfile()
    {
        // Get userId from the token
        var userId = User.FindFirst(System.Security.Claims.ClaimTypes.NameIdentifier)?.Value;
        var email = User.FindFirst(System.Security.Claims.ClaimTypes.Email)?.Value;
        
        return Ok(new
        {
            message = "You are authenticated!",
            userId = userId,
            email = email
        });
    }
}