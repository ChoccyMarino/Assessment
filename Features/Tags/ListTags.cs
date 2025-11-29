using MediatR;
using Assessment.Data;
using Microsoft.EntityFrameworkCore;

namespace Assessment.Features.Tags;


// query, this just needs the userId
public class ListTagsQuery : IRequest<ListTagsResult>
{
    // no parameters are required, just list all of the tags that the user has created
}

// result, this is where we return the list of tags
public class ListTagsResult
{
    public bool Success { get; set; }
    public List<TagDto> Tags { get; set; } = new();
}

// DTO, 
public class TagDto
{
    public int Id { get; set; }
    public string Name { get; set; } = null!;
}

// handler, this is where we actually list the tags
public class ListTagsQueryHandler : IRequestHandler<ListTagsQuery,ListTagsResult>
{
    private readonly ApplicationDbContext _context;
    
    public ListTagsQueryHandler(ApplicationDbContext context)
    {
        _context = context;
    }

    public async Task<ListTagsResult> Handle(ListTagsQuery request, CancellationToken cancellationToken)
    {
        // get all tags
        var tags = await _context.Tags
            .OrderBy(t => t.Name) // alphabetical order
            .Select(t => new TagDto
            {
                Id = t.Id,
                Name = t.Name
            })
            .ToListAsync(cancellationToken);

        return new ListTagsResult
        {
            Success = true,
            Tags = tags
        };
    }
}