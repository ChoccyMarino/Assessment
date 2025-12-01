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

            var result = await _mediator.Send(command);
            
            if (result.Success)
            {
                return Ok(result);
            }
            
            return BadRequest(result);
    }

    [HttpGet]
    public async Task<IActionResult> ListTags()
    {
            var query = new ListTagsQuery();
            var result = await _mediator.Send(query);
            
            return Ok(result);
    }
}