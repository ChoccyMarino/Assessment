using MediatR;
using Microsoft.AspNetCore.Mvc;
using Assessment.Features.Auth;
using FluentValidation;

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
}