using MediatR;
using FluentValidation;
using Assessment.Data;
using Assessment.Models;
using Microsoft.EntityFrameworkCore;
using BCrypt.Net;
using System.Data;

namespace Assessment.Features.Auth;

//command - this is the DTO / data we are sending
public class LoginCommand : IRequest<LoginResult>
{
    public string Email { get; set; } = null!;
    public string Password { get; set; } = null!;
}

// result - this is the data we are getting back
public class LoginResult
{
    public bool Success { get; set; }
    public string Message { get; set; } = null!;
    public int? UserId { get; set; } //optional, will only be set if login is successful
    public string? Token { get; set; } //will be used later for JWT authn
}

//validator , this is where we check if the data is valid or not
public class LoginCommandValidator : AbstractValidator<LoginCommand>
{
    public LoginCommandValidator()
    {
        RuleFor(x => x.Email)
            .NotEmpty().WithMessage("Email is required")
            .EmailAddress().WithMessage("A valid email is required");

        RuleFor(x => x.Password)
            .NotEmpty().WithMessage("Password is required");
    }
}

//handler - this is where the logic goes (DB calls, etc)
public class LoginCommandHandler : IRequestHandler<LoginCommand, LoginResult>
{
    private readonly ApplicationDbContext _context;

    public LoginCommandHandler(ApplicationDbContext context)
    {
        _context = context;
    }

    public async Task<LoginResult> Handle(LoginCommand request, CancellationToken cancellationToken)
    {
        //find user by email
        var user = await _context.Users
            .FirstOrDefaultAsync(u => u.Email == request.Email, cancellationToken);

        // if user is not found, return failure 
        if (user ==null)
        {
            return new LoginResult
            {
                Success = false,
                Message = "Invalid email or password",
                UserId = null,
                Token = null
            };
        }
        //check if password is correct
        var passwordValid = BCrypt.Net.BCrypt.Verify(request.Password, user.PasswordHash);
        if (!passwordValid)
        {
            return new LoginResult
            {
                Success = false,
                Message = "Invalid email or password",
                UserId = null,
                Token = null
            };
        }

        // success - check password
        return new LoginResult
        {
            Success = true,
            Message = "Login successful",
            UserId = user.Id,
            Token = null // will be used later for JWT authn
        };
    }
}