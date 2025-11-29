using MediatR;
using Assessment.Data;
using Microsoft.EntityFrameworkCore;
using System.Text.Json.Serialization;

namespace Assessment.Features.Posts;


// command, this just needs the postId
public class DeletePostCommand : IRequest<DeletePostResult>
{
    [JsonIgnore]
    public int PostId { get; set; } //from the route

    [JsonIgnore]
    public int UserId { get; set; } // from token (for ownership check)
}

// result, this is where we return the deleted post
public class DeletePostResult
{
    public bool Success { get; set; }
    public string Message { get; set; } =null!;
}

// handler, this is where we actually delete the post
public class DeletePostCommandHandler : IRequestHandler<DeletePostCommand, DeletePostResult>
{
    private readonly ApplicationDbContext _context;

    public DeletePostCommandHandler(ApplicationDbContext context)
    {
        _context = context;
    }

    public async Task<DeletePostResult> Handle(DeletePostCommand request, CancellationToken cancellationToken)
    {
        // find the post
        var post = await _context.Posts
            .FirstOrDefaultAsync(p => p.Id == request.PostId, cancellationToken);

        if (post == null)
        {
            return new DeletePostResult
            {
                Success = false,
                Message = "Post not found"
            };
        }

        // check ownership, only the author can delete
        if (post.UserId != request.UserId)
        {
            return new DeletePostResult
            {
                Success = false,
                Message = "You are not the author of this post"
            };
        }

        // delete the post
        _context.Posts.Remove(post);
        await _context.SaveChangesAsync(cancellationToken);

        return new DeletePostResult
        {
            Success = true,
            Message = "Post deleted successfully"
        };
    }
}
