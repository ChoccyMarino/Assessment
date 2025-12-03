using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Assessment.Features.Posts;
using FluentValidation;
using System.Security.Claims;

namespace Assessment.Controllers;

[ApiController]
[Route("api/[controller]")]
[Authorize] // all endpoints in this controller require authentication

public class PostsController : ControllerBase
{
    private readonly IMediator _mediator;

    public PostsController(IMediator mediator)
    {
        _mediator = mediator;
    }

    [HttpPost]
    public async Task<IActionResult> CreatePost([FromBody] CreatePostCommand command)
    {

            //get user id from the JWT token claims
            var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;

            if (userId == null)
            {
                return Unauthorized(new { message = "User not authenticated" });
            }

            //set userId on command (not from user input)
            command.UserId = int.Parse(userId);

            var result = await _mediator.Send(command);

            if (result.Success)
            {
                return Ok(result);
            }

            if (result.Message.Contains("not found", StringComparison.OrdinalIgnoreCase))
            {
                return NotFound(result);
            }

            if (result.Message.Contains("not the author", StringComparison.OrdinalIgnoreCase))
            {
                return Forbid();
            }

            return BadRequest(result);
    }

    [HttpGet]
    public async Task<IActionResult> ListPosts([FromQuery] int? userId)
    {
            var query = new ListPostsQuery
            {
                UserId = userId
            };

            var result = await _mediator.Send(query);

            return Ok(result);
    }

    [HttpDelete("{postId}")]
    public async Task<IActionResult> DeletePost(int postId)
    {
            // get the user id from the JWT token claims
            var userId = User.FindFirst(System.Security.Claims.ClaimTypes.NameIdentifier)?.Value;

            if (userId == null)
            {
                return Unauthorized(new { message = "User not authenticated "});
            }

            var command = new DeletePostCommand
            {
                PostId = postId,
                UserId = int.Parse(userId)
            };

            var result = await _mediator.Send(command);

            if (result.Success)
            {
                return Ok(result);
            }

            return BadRequest(result);
    }

    [HttpPut("{postId}")]
    public async Task<IActionResult> UpdatePost(int postId, [FromBody] UpdatePostCommand command)
    {
            // get the user id from the JWT token claims
            var userId = User.FindFirst(System.Security.Claims.ClaimTypes.NameIdentifier)?.Value;

            if (userId == null)
            {
                return Unauthorized(new { message = "User not authenticated "});
            }

            // set postId and userId (from the route and token, not from user input)
            command.PostId = postId;
            command.UserId = int.Parse(userId);

            var result = await _mediator.Send(command);

            if (result.Success)
            {
                return Ok(result);
            }

            if (result.Message.Contains("not found", StringComparison.OrdinalIgnoreCase))
            {
                return NotFound(result);
            }

            if (result.Message.Contains("not the author", StringComparison.OrdinalIgnoreCase))
            {
                return Forbid();
            }

            return BadRequest(result);
    }
}
