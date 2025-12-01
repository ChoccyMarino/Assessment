using MediatR;
using Assessment.Data;
using Microsoft.EntityFrameworkCore;
using Assessment.Services;

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
    private readonly RedisService _redis; // 1. add RedisService component
    
    // 2. inject redisservice into the constructor
    public ListTagsQueryHandler(ApplicationDbContext context, RedisService redis)
    {
        _context = context;
        _redis = redis;
    }

    public async Task<ListTagsResult> Handle(ListTagsQuery request, CancellationToken cancellationToken)
    {

        var cacheKey = "tags_list";

        //3. try cache first
        // ask redis "do we have tags_list?"
        var cachedTags = await _redis.GetAsync<List<TagDto>>(cacheKey);

        if (cachedTags != null)
        {
            // cache is hit, found in redis and should be returned immediately
            return new ListTagsResult
            {
                Success = true,
                Tags = cachedTags
            };
        }

        // 4. else, cache is missed
        var tags = await _context.Tags
            .OrderBy(t => t.Name) // alphabetical order
            .Select(t => new TagDto
            {
                Id = t.Id,
                Name = t.Name
            })
            .ToListAsync(cancellationToken);

        // 5. save to cache
        await _redis.SetAsync(cacheKey, tags, TimeSpan.FromMinutes(10));

        return new ListTagsResult
        {
            Success = true,
            Tags = tags
        };
    }
}