using MediatR;
using Assessment.Data;
using Assessment.Models;
using Microsoft.EntityFrameworkCore;

namespace Assessment.Features.Posts;

// query, this is the data/ dto we are sending
public class ListPostsQuery : IRequest<ListPostsResult>
{
    public int? UserId { get; set; } // optional, if provided we filter posts by this user
}

// result, this is the data we are getting back
public class ListPostsResult
{
    public bool Success { get; set; }
    public List <PostDto> Posts { get; set; } = new(); // list of posts
}

// DTO for returning the post data
public class PostDto
{
    public int Id { get; set; }
    public string Title { get; set; } = null!;
    public string Content { get; set; } = null!;
    public int UserId { get; set; }
    public DateTime CreatedAt { get; set; }
    public DateTime UpdatedAt { get; set; }
}

// handler this is where we contain the logic (DB calls, etc)
public class ListPostsQueryHandler : IRequestHandler<ListPostsQuery, ListPostsResult>
{
    private readonly ApplicationDbContext _context;

    public ListPostsQueryHandler(ApplicationDbContext context)
    {
        _context = context;
    }

    public async Task<ListPostsResult> Handle(ListPostsQuery request, CancellationToken cancellationToken)
    {
        // start with all the posts
        var query = _context.Posts.AsQueryable(); //.AsQueryable() allows us to build the query step by step

        //filter by userId if provided
        if (request.UserId.HasValue)
        {
            query = query.Where(p => p.UserId == request.UserId.Value);
        }

        // get the post and convert to DTO
        var posts = await query
            .OrderByDescending(p => p.CreatedAt) // newest is first
            .Select(p => new PostDto
            {
                Id = p.Id,
                Title = p.Title,
                Content = p.Content,
                UserId = p.UserId,
                CreatedAt = p.CreatedAt,
                UpdatedAt = p.UpdatedAt
            })
            .ToListAsync(cancellationToken);

        return new ListPostsResult
        {
            Success = true,
            Posts = posts
        };
    }
}