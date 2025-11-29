start with Visual Studio 2026

I used ASP. NET Core Web API
- .NET 8
- Auth type = None (because JWT auth is separate / libraries )
- Configure for HTTPS - tick
- enable container support - tick (Linux container, dockerfile)
- use controllers - tick
- OpenAPI support - tick (swagger, REST API Design)

---

Phase 1 (Foundation)
Phase 2 (CQRS/MediatR)
Phase 3 (Auth/Security)
Phase 4 (Redis Caching)
Phase 5 (Documentation)

---

Phase 1 
Foundation

I need Nuget packages:
- Sql? (Microsoft.EntityFrameworkCore.SqlServer)
- Microsoft.EntityFrameworkCore.Tools (add-migration, update-database)
- MediatR (CQRS- request, command, event, handler)
- FluentValidation (input validation)
- FluentValidation.AspNetCore (for ASP.NET Core model binding pipeline)
- Microsoft.AspNetCore.Authentication.JwtBearer (auth token, login)
- StackExchange.Redis (caching)

Models files
- User.cs
- UserProfile.cs
- Post.cs
- Tag.cs
- PostTag.cs


User is linked to UserProfile - User has collections of Post - Post has collection of PostTag - Tag has collection of PostTag

User.cs
- User.ID
- User.Username
- User.Email
- User.PasswordHash
- User.CreatedAt
- User is linked to UserProfile. One to one relationship.
- User has collection of post. One to many relationship.

UserProfile.cs
- UserProfile.ID
- UserProfile.Bio
- UserProfile is linked to User. One to one relationship.

Post.cs
- Post.ID
- Post.Title
- Post.Content
- Post.CreatedAt
- Post is linked to User. Many to one relationship.
- Post has collection of PostTag. One to many relationship.

Tag.cs
- Tag.ID
- Tag.Name
- Tag has collection of PostTag. one to many relationship.

PostTag.cs
- PostTag.PostID
- PostTag.Tag
- PostTag is linked to Post. Many to one relationship.
- PostTag is linked to Tag. many to one relationship.

---

Phase 2 
Database Setup

Set up ApplicationDbContext.cs
- namespace exposes the class to Program.cs
- DbSet<User> Users in DB
- OnModelCreating runs during the migration
- PostTags needs composite key because it is a junction / bridge table
- User Profile is dependent on User existing. One to one relationship.

Set up connection string in appsettings.json
Set up Program.cs to add ApplicationDbContext to services
Set up initial migration (Add-Migration InitialCreate)
Commited using Update-Database

---

Phase 3
CQRS/MediatR

Command Query Responsibility Segregation.
- Commands - change state / data = Create, Update, Delete
- Queries -  read data = Get, List

MediatR
- Separates controllers from business logic
- each operation is isolated and easy to test/understand

-- installed MediatR Nuget package
-- installed FluentValidation Nuget package
-- installed FluentValidation.DependencyInjectionExtensions Nuget package

setup folder structure
- Features
- - Auth (registration, login)
- - Posts (post operations)
- - Tags (tag operations)

for every operation, there will be 3 files.
- Command/Query file (the message)
- validator file (input validation)
- Handler file (does the work/logic)

created a Register.cs file in Auth folder which contains 

---

Registering MediatR 

- decided not to install MediatR.Extensions.FluentValidation.AspNetCore because it is deprecated
- set up my own ValidationBehavior.cs in Behaviors folder

ValidationBehavior.cs
- will run all validators
- if valid > continue to handler
- if invalid > throw error, stop here

created AuthController.cs
- has Register endpoint
- successfully tested and registered a user using Swagger UI
- created Login.cs file in Auth folder which contains LoginCommand, LoginResult, LoginCommandHandler
- created LoginValidator.cs in Auth folder
- created Login endpoint in AuthController.cs
- successfully tested and logged in a user using Swagger UI

---

JWT Auth (Json Web Token)

1. Install Nuget package Microsoft.AspNetCore.Authentication.JwtBearer
2. In Program.cs, configure JWT authentication
3. Update Login handler to generate JWT token upon successful login
4. testing

- JwtBearer setup complete
- add JWT settings to appsettings.json
- - store a secret key
- - store issuer
- - store audience
- - store token expiry time

---

Set up Features/Posts
- CreatePost.cs
- ListPosts.cs
- DeletePost.cs