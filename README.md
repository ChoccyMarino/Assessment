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