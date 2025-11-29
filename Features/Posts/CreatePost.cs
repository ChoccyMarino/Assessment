using MediatR;
using FluentValidation;
using Assessment.Data;
using Assessment.Models;
using Microsoft.EntityFrameworkCore;
using System.Security.Claims;
using System.Text.Json.Serialization;

namespace Assessment.Features.Posts;

// command, this is the DTO / data we are sending
public class CreatePostCommand : IRequest<CreatePostResult>
{
    public string Title { get; set; } = null!;
    public string Content { get; set; } = null!;
    public List<int> TagIds { get; set; } = new(); // optional, list of tag IDs to attach. new() initializes to empty list

    //this will be set by the controller, not by the client
    [JsonIgnore]
    public int UserId { get; set; }
}

// result, this is the data we are getting back
public class CreatePostResult
{
    public bool Success { get; set; }
    public string Message { get; set; } = null!;
    public int? PostId { get; set; } // optional, will only be set if creation is successful
}

//validator, this is where we check if the data is valid or not
public class CreatePostCommandValidator : AbstractValidator<CreatePostCommand>
{
    public CreatePostCommandValidator()
    {
        RuleFor(x => x.Title)
            .NotEmpty().WithMessage("Title is required")
            .MinimumLength(5).WithMessage("Title must be at least 5 characters long")
            .MaximumLength(100).WithMessage("Title cannot exceed 100 characters");

        RuleFor(x => x.Content)
            .NotEmpty().WithMessage("Content is required")
            .MinimumLength(10).WithMessage("Content must be at least 10 characters long");
    }
}

// handler, this is where we contain the logic (DB calls, etc)
public class CreatePostCommandHandler : IRequestHandler<CreatePostCommand, CreatePostResult>
{
    private readonly ApplicationDbContext _context;

    public CreatePostCommandHandler(ApplicationDbContext context)
    {
        _context = context;
    }

    public async Task<CreatePostResult> Handle(CreatePostCommand request, CancellationToken cancellationToken)
    {
        // we wiill get userId from the controller (passed as a separate parameter)

        //create new post
    var post = new Post
    {
        Title = request.Title,
        Content = request.Content,
        CreatedAt = DateTime.UtcNow,
        UpdatedAt = DateTime.UtcNow,
        UserId = request.UserId
    };

    _context.Posts.Add(post);
    await _context.SaveChangesAsync(cancellationToken);

    //handle any tags if provided
    if (request.TagIds.Any())
    {
        foreach (var tagId in request.TagIds)
        {
            var postTag = new PostTag
            {
                PostId = post.Id,
                TagId = tagId
            };
            _context.Set<PostTag>().Add(postTag);
        }
        await _context.SaveChangesAsync(cancellationToken);
    }

    return new CreatePostResult { Success = true, Message = "Post created successfully", PostId = post.Id };
}
}