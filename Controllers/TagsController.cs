using MediatR;
using Microsoft.AspNetCore.Mvc;
using Assessment.Features.Tags;
using FluentValidation;

namespace Assessment.Controllers;

[ApiController]
[Route("api/[controller]")]
public class TagsController : ControllerBase
{
    private readonly IMediator _mediator;

    public TagsController(IMediator mediator)
    {
        _mediator = mediator;
    }

    [HttpPost]
    public async Task<IActionResult> CreateTag([FromBody] CreateTagCommand command)
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

    [HttpGet]
    public async Task<IActionResult> ListTags()
    {
        try
        {
            var query = new ListTagsQuery();
            var result = await _mediator.Send(query);
            
            return Ok(result);
        }
        catch (Exception ex)
        {
            return BadRequest(new { message = "Failed to retrieve tags", error = ex.Message });
        }
    }
}