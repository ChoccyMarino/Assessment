using MediatR;
using FluentValidation;
using Assessment.Data;
using Assessment.Models;
using Microsoft.EntityFrameworkCore;
using System.Text.Json.Serialization;


namespace Assessment.Features.Profiles;

// command, this just need userId
public class UpdateProfileCommand : IRequest<UpdateProfileResult>
{
    public string Bio { get; set; } = null!;

    [JsonIgnore]
    public int UserId { get; set; } // from the token
}

// result, this is where we return the updated profile
public class UpdateProfileResult
{
    public bool Success { get; set; }
    public string Message { get; set; } = null!;
}

// validator, this is where we validate the command
public class UpdateProfileCommandValidator : AbstractValidator<UpdateProfileCommand>
{
    public UpdateProfileCommandValidator()
    {
        RuleFor(x => x.Bio)
            .MaximumLength(500).WithMessage("Bio must not exceed 500 characters");
    }
}

// handler, this is where we actually update the profile
public class UpdateProfileCommandHandler : IRequestHandler<UpdateProfileCommand, UpdateProfileResult>
{
    private readonly ApplicationDbContext _context;

    public UpdateProfileCommandHandler(ApplicationDbContext context)
    {
        _context = context;
    }

    public async Task<UpdateProfileResult> Handle(UpdateProfileCommand requests, CancellationToken cancellationToken)
    {
        // get user with profile
        var user = await _context.Users
            .Include(u => u.UserProfile)
            .FirstOrDefaultAsync(u => u.Id == requests.UserId, cancellationToken);

        if (user == null)
        {
            return new UpdateProfileResult
            {
                Success = false,
                Message = "User not found"
            };
        }

        //if profile doesn't exist, create it
        if (user.UserProfile == null)
        {
            user.UserProfile = new UserProfile
            {
                UserId = user.Id,
                Bio = requests.Bio
            };
            _context.UserProfiles.Add(user.UserProfile);
        }
        else
        {
            // update existing profile
            user.UserProfile.Bio = requests.Bio;
        }

        await _context.SaveChangesAsync(cancellationToken);

        return new UpdateProfileResult
        {
            Success = true,
            Message = "Profile updated successfully"
        };
    }
}