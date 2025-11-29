using MediatR;
using Assessment.Data;
using Microsoft.EntityFrameworkCore;
using System.Text.Json.Serialization;

namespace Assessment.Features.Profiles;


// query, this just needs the userId
public class GetProfileQuery : IRequest<GetProfileResult>
{
    [JsonIgnore]
    public int UserId { get; set; } // from the token
}

// result, this is where we return the profile
public class GetProfileResult
{
    public bool Success { get; set; }
    public string Message { get; set; } =null!;
    public UserProfileDto? Profile { get; set; }
}

// DTO
public class UserProfileDto
{
    public int UserId { get; set; }
    public string Username { get; set; } = null!;
    public string Email { get; set; } = null!;
    public string? Bio { get; set; } = null!;
    public DateTime CreatedAt { get; set; }
}

// handler, this is where we actually get the profile
public class GetProfileQueryHandler : IRequestHandler<GetProfileQuery, GetProfileResult>
{
    private readonly ApplicationDbContext _context;
    
    public GetProfileQueryHandler(ApplicationDbContext context)
    {
        _context = context;
    }
    
    public async Task<GetProfileResult> Handle(GetProfileQuery request, CancellationToken cancellationToken)
    {
        // Get user with profile
        var user = await _context.Users
            .Include(u => u.UserProfile)
            .FirstOrDefaultAsync(u => u.Id == request.UserId, cancellationToken);
        
        if (user == null)
        {
            return new GetProfileResult
            {
                Success = false,
                Message = "User not found"
            };
        }
        
        // Build profile DTO
        var profileDto = new UserProfileDto
        {
            UserId = user.Id,
            Username = user.Username,
            Email = user.Email,
            Bio = user.UserProfile?.Bio ?? null,  // might be null if no profile created yet
            CreatedAt = user.CreatedAt
        };
        
        return new GetProfileResult
        {
            Success = true,
            Message = "Profile retrieved successfully",
            Profile = profileDto
        };
    }
}
