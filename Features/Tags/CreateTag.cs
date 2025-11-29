using MediatR;
using FluentValidation;
using Assessment.Data;
using Assessment.Models;
using Microsoft.EntityFrameworkCore;

namespace Assessment.Features.Tags;

// command ,this just needs the tag name
public class CreateTagCommand : IRequest<CreateTagResult>
{
    public string Name { get; set; } = null!;
}

// result, this is where we return the created tag
public class CreateTagResult
{
    public bool Success { get; set; }
    public string Message { get; set; } = null!;
    public int? TagId { get; set;}
}

// validator, this is where we validate the command
public class CreateTagCommandValidator : AbstractValidator<CreateTagCommand>
{
    public CreateTagCommandValidator()
    {
        RuleFor(x => x.Name)
            .NotEmpty().WithMessage("Tag name is required")
            .MinimumLength(2).WithMessage("Tag name must be at least 2 characters")
            .MaximumLength(50).WithMessage("Tag name must not exceed 50 characters");
    }
}

// handler, this is where we actually create the tag
public class CreateTagCommandHandler : IRequestHandler<CreateTagCommand, CreateTagResult>
{
    private readonly ApplicationDbContext _context;
    
    public CreateTagCommandHandler(ApplicationDbContext context)
    {
        _context = context;
    }

    public async Task<CreateTagResult> Handle(CreateTagCommand request, CancellationToken cancellationToken)
    {
        // check if tag already exists
        var existingTag = await _context.Tags
            .FirstOrDefaultAsync(t => t.Name.ToLower() == request.Name.ToLower(), cancellationToken);
   
        if (existingTag != null)
        {
            return new CreateTagResult
            {
                Success = false,
                Message = "Tag already exists",
                TagId = existingTag.Id
            };
        }

        // create new tag
        var tag = new Tag
        {
            Name = request.Name
        };

        _context.Tags.Add(tag);
        await _context.SaveChangesAsync(cancellationToken);

        return new CreateTagResult
        {
            Success = true,
            Message = "Tag created successfully",
            TagId = tag.Id
        };
    }
}