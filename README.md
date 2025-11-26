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