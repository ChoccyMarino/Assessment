using MediatR;
using FluentValidation;
using Assessment.Data;
using Assessment.Models;
using Microsoft.EntityFrameworkCore;
using BCrypt.Net;

namespace Assessment.Features.Auth;

    // Controller - Command - this is the DTO / data we are sending
    public class RegisterCommand : IRequest<RegisterResult>
    {
        public string Username { get; set; } = null!;
        public string Email { get; set; } = null!;
        public string Password { get; set; } = null!;
    }

    // result - this is the data we are getting back
    public class RegisterResult 
    {
        public bool Success { get; set; }
        public string Message { get; set; } = null!;
        public int? UserId { get; set; } //optional, will only be set if registration is successful
    }


    // Mediator - Validator - this is where we check if the data is valid or not
    //AI was used here because I am not familiar with the exact syntax for FluentValidation
    public class RegisterCommandValidator : AbstractValidator<RegisterCommand>
    {
        public RegisterCommandValidator()
        {
            //rules for username. usernames are required and must be between 3 and 50 characters long
            RuleFor(x => x.Username)
                .NotEmpty().WithMessage("Username is required")
                .MinimumLength(3).WithMessage("Username must be at least 3 characters long")
                .MaximumLength(50).WithMessage("Username must be at most 50 characters long");

            // rules for email. emails are required and must be a valid email format
            RuleFor(x => x.Email)
                .NotEmpty().WithMessage("Email is required")
                .EmailAddress().WithMessage("A valid email is required");

            // rules for password.
            RuleFor(x => x.Password)
                .NotEmpty().WithMessage("Password is required")
                .MinimumLength(8).WithMessage("Password must be at least 8 characters long")
                .Matches("[A-Z]").WithMessage("Password must contain at least one uppercase letter")
                .Matches("[a-z]").WithMessage("Password must contain at least one lowercase letter")
                .Matches("[0-9]").WithMessage("Password must contain at least one number")
                .Matches("[^a-zA-Z0-9]").WithMessage("Password must contain at least one special character");
        }
    }
    
    // Handler - this is where we process the command / work (DB calls, etc)
    public class RegisterCommandHandler : IRequestHandler<RegisterCommand, RegisterResult>
    {
        private readonly ApplicationDbContext _context;

        public RegisterCommandHandler(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<RegisterResult> Handle(RegisterCommand request, CancellationToken cancellationToken)
        {
            //check if the username already exists in the DB
            var existingUser = await _context.Users
                .FirstOrDefaultAsync(u => u.Username == request.Username, cancellationToken);

            if (existingUser != null)
            {
                return new RegisterResult
                {
                    Success = false,
                    Message = "Username already exists",
                    UserId = null
                };
            }


            //check if email already exists in the DB
            var existingEmail = await _context.Users
                .FirstOrDefaultAsync(u => u.Email == request.Email, cancellationToken);

            if (existingEmail != null)
            {
                return new RegisterResult
                {
                    Success = false,
                    Message = "Email already exists",
                    UserId = null
                };
            }

            //create a new user
            var user = new User
            {
                Username = request.Username,
                Email = request.Email,
                PasswordHash = BCrypt.Net.BCrypt.HashPassword(request.Password),
                CreatedAt = DateTime.UtcNow
            };

            _context.Users.Add(user);
            await _context.SaveChangesAsync(cancellationToken);

            return new RegisterResult
            {
                Success = true,
                Message = "User registered successfully",
                UserId = user.Id
            };
        }
    }