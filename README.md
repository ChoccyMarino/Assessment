# Blog Management API

A RESTful API for managing blog posts, users, and tags built with ASP.NET Core 8, implementing CQRS
pattern with MediatR, FluentValidation, JWT authentication, Redis caching and PostgreSQL database. 

## Table of Contents
- [Technology Stack](#technology-stack)
- [Architecture](#architecture)
- [Prerequisites](#prerequisites)
- [Setup & Installation](#setup--installation)
- [Running the Application](#running-the-application)
- [API Endpoints](#api-endpoints)
- [Error Handling](#error-handling)
- [Caching Strategy](#caching-strategy)
- [Database Schema](#database-schema)
- [AI Usage Disclosure](#ai-usage-disclosure)

## Technology Stack
- **Framework**: ASP.NET Core 8
- **Database**: PostgreSQL 15
- **ORM**: Entity Framework Core
- **Architecture**: CQRS with MediatR
- **Authentication**: JWT Bearer
- **Caching**: Redis (StackExchange.Redis)
- **API Documentation**: Swagger (OpenAPI)
- **Containerization**: Docker &  Docker Compose
- **Password Hashing**: BCrypt
- **Validation**: FluentValidation

## Architecture

### CQRS Pattern
This project implements **Command Query Responsibility Segregation (CQRS)** using MediatR
- **Commands** - operations that modify states (create, update, Delete)
- **Queries** - operations that read data (get, list)

```txt
Assessment/
├── Controllers/        # API endpoints (thin layer)
├── Features/           # CQRS operations organized by domain
│   ├── Auth/           # Registration, Login
│   ├── Posts/          # Post CRUD operations
│   ├── Tags/           # Tag operations
│   └── Profiles/       # User profile operations
├── Models/             # Entity models
├── Data/               # DbContext
├── Services/           # JwtService, RedisService
├── Middleware/         # Global error handling
└── Behaviors/          # MediatR pipeline behaviors (validation)
```

## Key Design Decisions
### 1. Global Error Handling
- Implemented [ExceptionHandlingMiddleware](cci:1://file:///c:/Users/MSI/source/repos/Assessment/Middleware/ExceptionHandlingMiddleware.cs:12:4-16:5) to catch all exceptions and return a standardized error response.
- Returns consistent JSON error response
- Eliminates repetitive try-catch blocks in controllers

### 2. Redis Caching
- Tags are cached for 10 minutes (read-heavy, rarely changes)
- Cache is invalidated when a new tag is created
- Improves performance for frequently accessed data (such as tags)

### 3. Password Security
- Passwords are hashsed using BCrypt (industry standard)
- Never store passwords in plain text

### 4. JWT Authentication
- Stateless authentication using JWT tokens
- Tokens are short-lived (60 minutes)
- User ID and email stored in claims

## Prerequisites
- [.NET 8 SDK](https://dotnet.microsoft.com/download/dotnet/8.0)
- [Docker Desktop](https://www.docker.com/products/docker-desktop)
- [PostgreSQL](https://www.postgresql.org/download/) (if running locally without Docker)
- [Redis](https://redis.io/download) (if running locally without Docker)

## Setup & Installation

###  Option 1: Docker (Reccomended)

1. **Clone the repository**
```bash
git clone https://github.com/ChoccyMarino/Assessment
cd Assessment
```

2. **Run with Docker Compose**
```bash
docker-compose up --build -d
```
This will start:
- API on https://localhost:5200
- Postgres on localhost:5433
- Redis on localhost:6379

3. **Run database migrations**
```bash
docker exec assessment-api-1 dotnet ef database update

4. **Access Swagger UI**
-  Access Swagger UI at https://localhost:5200/swagger
```

### Option 2: Local Development
1. **Update connection strings in appsettings.json**
- Update the connection strings in [appsettings.json](cci:1://file:///c:/Users/MSI/source/repos/Assessment/appsettings.json:1:4-1:4) to point to your local Postgres and Redis databases.
```json
"ConnectionStrings": {
  "DefaultConnection": "Host=localhost;Port=5432;Database=BlogDB;Username=postgres;Password=yourpassword"
}
```

2. **Run migrations**
```bash
dotnet ef database update
```

3. **Start Redis (if using caching)**
```bash
redis-server
```

4. **Run the application**
```bash
dotnet run
```

- the API will be available at https://localhost:5200

## API Endpoints

### Authentication
#### Register User
```json
POST /api/auth/register
Content-Type: application/json

{
    "username": "johndoe",
    "email": "john@example.com",
    "password": "password123"
}
```

#### Response (200 OK)

```json
{
    "success": true,
    "message": "User created successfully",
    "userId": 1
}
```

#### Login

```json
POST /api/auth/login
Content-Type: application/json

{
    "email": "john@example.com",
    "password": "password123"
}
```

#### Response (200 OK)

```json
{
    "success": true,
    "message": "Login successful",
    "token": "eyJhbGciOiJ..."
}
```

### Posts (Require Authentication)
#### Create Post
```json
POST /api/posts
Authorization: Bearer {token}
Content-Type: application/json

{
    "title": "My First Post",
    "content": "This is my first post",
    "tagIds": [1, 2, 3]
}
```

#### List Posts
```json
GET /api/posts?userId=1
Authorization: Bearer {token}
```

#### Update Post
```json
PUT /api/posts/{postId}
Authorization: Bearer {token}
Content-Type: application/json

{
    "title": "This is my updated post",
    "content": "This is my updated post",
    "tagIds": [1, 3]
}
```

#### Delete Post
```json
DELETE /api/posts/{postId}
Authorization: Bearer {token}
```

### Tags
#### Create Tag
```json
POST /api/tags
Content-Type: application/json

{
    "name": "My Tag"
}
```
#### List All Tags (cached)
```json
GET /api/tags
```

#### Response (200 OK)
``` json
{
    "success": true,
    "tags": [
        { "id": 1, "name": "My Tag" },
        { "id": 2, "name": "My Tag 2" },
    ]
}
```

### User Profile
#### Get Profile
```json
GET /api/profile
Authorization: Bearer {token}
```

#### Update Profile
```json
PUT /api/profile
Authorization: Bearer {token}
Content-Type: application/json

{
    "bio": "This is my updated bio"
}
```

## Error Handling

All errors are handled by [ExceptionHandlingMiddleware](cci:2://file:///c:/Users/MSI/source/repos/Assessment/Middleware/ExceptionHandlingMiddleware.cs:7:0-75:1) and return a consistent JSON format:

```json
{
    "success": false,
    "message": "Error description."
    "errors": [ /* validation errors if applicable */ ]
}
```

### HTTP Status Codes
- 400 Bad Request - validation errors
- 401 Unauthorized - missing or invalid JWT token
- 404 Not Found - resource not found
- 500 Internal Server Error - Unexpected error

### Caching Strategy
Redis caching is implemented for the Tags endpoint:
- Cache key: `tags_list`
- Cache expiration: 10 minutes
- Invalidation: Cache is cleared when a new tag is created

### Why Tags Only
- Tags are read-heavy and rarely change
- Perfect  candidate for caching

### Database Schema
```txt
Users (1) ←→ (1) UserProfiles
  ↓ (1:N)
Posts (N) ←→ (N) Tags (via PostTags junction table)
```
Key Relationships:
- User <-> UserProfile: One to one relationship
- User -> Post: One to many relationship
- Posts <-> Tags: Many to many relationship (via PostTags junction table)

### Indexes & Rationale
- **Users.Email (Unique)**: Frequent lookups during login/registration; uniqueness ensures no duplicate accounts.
- **Users.Username (Unique)**: Ensures unique identity; used for public profile URLs (potential future feature).
- **Posts.UserId (Foreign Key)**: Optimizes "Get My Posts" queries by filtering on UserId.
- **PostTags.PostId, PostTags.TagId (Composite)**: Ensures a tag is only added once to a post; optimizes join queries.

## Logging & Monitoring

**Logging Framework**: Microsoft.Extensions.Logging (built-in)

**Whats Logged:**
- Unhandled exceptions (via ExceptionHandlingMiddleware)
- EF Core database queries (in Development mode)
- Application startup / shutdown events

## API Rate Limiting

**Current Status:** Not implemented

**Production Approach:**
- Use ASP.NET Core built-in rate limiting middleware (.NET 7+)
- Configure per-user limits (e.g., 100 requests/ minute per JWT token)
- Return '429 Too Many Requests' when limit exceeded

## AI Usage Disclosure
### Transparency Statement
I used AI assistants (Claude AI and Google Gemini) as learning tools and pair programming partners. AI helped with:
- Learning new patterns : MediatR CQRS pattern, FluentValidation pipeline, Redis caching strategies
- Code structure guidance: Suggested folder organization, middleware implementation, Docker configuration
- Debugging: Helped identify issues like HTTPS redirection in docker, port conflicts in docker-compose, and debugging middleware
- Best practices: JWT configuration, BCrypt password hashing, global error handling.

### What I Did:
- Typed every line of code myself. No copy-paste from AI responses
- Made all architectural decisions. Chose to use CQRS, Redis for tags, Docker for deployment
- Asked questions to understand. After each code snippet, I asked the AI to explain concepts to ensure I understood
- Debugged and tested. Ran the application, tested endpoints, fixed issues independently

### Why This Approach
- I'm a self taught developer learning advanced patterns (MediatR, Redis) for the first time.
- AI accelerated my learning curve while ensuring I understood each concept properly.
- This mirrors real - world development where developers use documentation, StackOverflow, and other resources to learn.

### Honest Assessment
- I understand the code I wrote
- I can explain the architecture and design decisions
- I'm ready to discuss any part of this project in detail

## Notes
- Password policy: Minimum 8 characters, at least 1 uppercase, 1 lowercase, 1 number, 1 special character (enforced by FluentValidation)
- JWT token expiry time: 60 minutes
- Swagger: available in all environments (including production for demo purposes)
- HTTPS Redirection: Disabled in Docker (handled by reverse proxy in production)


