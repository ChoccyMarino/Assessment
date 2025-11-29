using MediatR;
using FluentValidation;
using Assessment.Data;
using Microsoft.EntityFrameworkCore;
using System.Text.Json.Serialization;
using Assessment.Features.Posts;
using Assessment.Models;

namespace Assessment.Features.Posts;

// command, this just needs the postId
public class UpdatePostCommand : IRequest<UpdatePostResult>
{
    [JsonIgnore]
    public int PostId { get; set; } //from the route

    public string Title { get; set; } = null!;
    public string Content { get; set; } = null!;
    public List<int> TagIds { get; set; } = new();
    [JsonIgnore]
    public int UserId { get; set; } // from token (for ownership check)
}

// result, this is where we return the updated post
public class UpdatePostResult
{
    public bool Success { get; set; }
    public string Message { get; set; } =null!;
}

// handler, this is where we actually update the post
public class UpdatePostCommandValidator : AbstractValidator<UpdatePostCommand>
{
    public UpdatePostCommandValidator()
    {
        RuleFor(x => x.Title)
            .NotEmpty().WithMessage("Title is required")
            .MinimumLength(5).WithMessage("Title must be at least 5 characters")
            .MaximumLength(100).WithMessage("Title must not exceed 100 characters");

        RuleFor(x => x.Content)
            .NotEmpty().WithMessage("Content is required")
            .MinimumLength(10).WithMessage("Content must be at least 10 characters")
            .MaximumLength(10000).WithMessage("Content must not exceed 10000 characters");
    }
}

// handler, this is where we actually update the post
public class UpdatePostCommandHandler(ApplicationDbContext context) : IRequestHandler<UpdatePostCommand, UpdatePostResult>
{
    public async Task<UpdatePostResult> Handle(UpdatePostCommand request, CancellationToken cancellationToken)
    {
        //find the post
        var post = await context.Posts
            .Include(p => p.PostTags) // include tags for updating
            .FirstOrDefaultAsync(p => p.Id == request.PostId, cancellationToken);

        if (post == null)
        {
            return new UpdatePostResult
            {
                Success = false,
                Message = "Post not found"
            };
        }

        // check ownership, only the author can update
        if (post.UserId != request.UserId)
        {
            return new UpdatePostResult
            {
                Success = false,
                Message = "You are not the author of this post"
            };
        }

        //update the post
        post.Title = request.Title;
        post.Content = request.Content;
        post.UpdatedAt = DateTime.UtcNow;

        // update the post tags
        context.Set<PostTag>().RemoveRange(post.PostTags);

        if (request.TagIds.Any())
        {
            foreach (var tagId in request.TagIds)
            {
                var postTag = new PostTag
                {
                    PostId = post.Id,
                    TagId = tagId
                };
                context.Set<PostTag>().Add(postTag);
            }
        }

        await context.SaveChangesAsync(cancellationToken);

        return new UpdatePostResult
        {
            Success = true,
            Message = "Post updated successfully"
        };
    }
}
