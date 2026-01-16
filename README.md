# FAIR 

A comprehensive backend API system for managing athletic performance, coaching workflows, and team communication. Built with ASP.NET Core, the system provides role-based access control, real-time messaging, and performance analytics for coaches and players.

## Overview

FAIR (Functional Athletic Intelligence & Reports) is a backend service designed to streamline sports team management by connecting coaches and players through performance tracking and real-time communication. The system implements secure authentication, granular authorization, and scalable architecture patterns suitable for production environments.

**Primary Use Cases:**
- Performance report creation and analysis by coaches
- Player performance tracking and historical data access
- Real-time team communication with message persistence
- Player ranking and comparative analytics

## Features

### Core Functionality

- **Role-Based Access Control**: Distinct permissions for coaches and players with JWT-based authentication
- **Performance Management**: CRUD operations for player reports including scores, analysis, and timestamps
- **Analytics Engine**: Automated player ranking based on performance metrics
- **Real-Time Communication**: WebSocket-based chat with message history and read status tracking
- **Profile Management**: Self-service profile updates with authorization boundaries
- **Data Filtering**: Query reports by player, coach, date range, and performance thresholds

## User Roles & Authentication

### Authentication Flow

The system uses JWT (JSON Web Tokens) for stateless authentication:

1. User submits credentials to `/api/auth/login`
2. Server validates credentials and generates JWT with role claims
3. Client includes token in `Authorization: Bearer <token>` header
4. Middleware validates token signature and extracts claims for authorization

### Role Definitions

**Coach**
- Create, read, update, delete performance reports
- View all players and their historical data
- Access team-wide analytics and rankings
- Initiate chat conversations with any player

**Player**
- Read own performance reports
- View personal performance history and statistics
- Update own profile information
- Participate in chat conversations with assigned coaches

### Security Implementation

- Passwords hashed using BCrypt with configurable work factor
- Token expiration with refresh token support
- Role-based authorization policies on controller actions
- Resource-based authorization for profile updates (users can only modify their own data)

## Report Management

### Data Model
```
PerformanceReport
├── Id (Guid)
├── PlayerId (Guid, foreign key)
├── CoachId (Guid, foreign key)
├── Score (decimal)
├── Analysis (string)
├── Category (enum: Physical, Technical, Tactical, Mental)
├── CreatedAt (DateTime)
└── UpdatedAt (DateTime)
```

### Operations

**Create Report** - `POST /api/reports`
- Coach-only endpoint
- Validates player existence and score range
- Auto-timestamps creation

**Retrieve Reports** - `GET /api/reports`
- Filterable by playerId, coachId, dateRange, minScore
- Paginated responses (default 20 per page)
- Coaches see all reports, players see only their own

**Update Report** - `PUT /api/reports/{id}`
- Coach-only, must be original creator
- Partial updates supported via PATCH semantics
- Updates `UpdatedAt` timestamp automatically

**Delete Report** - `DELETE /api/reports/{id}`
- Soft delete implementation preserving audit trail
- Coach-only, creator validation required

### Ranking Algorithm

Players ranked by weighted average:
- Recent reports weighted higher (exponential decay)
- Category-specific rankings available
- Recalculated on report creation/update via background job
- Cached for performance (5-minute TTL)

## Real-Time Chat (SignalR)

### Implementation

SignalR hub (`ChatHub`) provides WebSocket connections with fallback to long polling. All connections authenticated via JWT passed in query string during handshake.

### Features

**Message Delivery**
- One-to-one messaging between coaches and players
- Message persistence to database for history
- Automatic timestamp and sender identification
- Delivery confirmation via SignalR acknowledgments

**Message History**
- `GET /api/chat/history/{userId}` retrieves paginated conversation history
- Ordered by timestamp descending
- Includes sender metadata (name, role)

**Unread Tracking**
- Read receipts stored per user per message
- `GET /api/chat/unread` returns count of unread messages
- `POST /api/chat/mark-read` marks messages as read by message IDs

**Connection Management**
- User-to-connection-ID mapping maintained in-memory (Redis for scale-out)
- Online/offline status exposed via hub methods
- Automatic reconnection handling with message replay

### Hub Methods
```csharp
SendMessage(string recipientId, string content)
GetOnlineUsers() -> List<UserStatus>
MarkTyping(string recipientId)
```

## Profile Management & Security

### Profile Operations

**View Profile** - `GET /api/users/{id}`
- Returns sanitized user data (no password hash)
- Players can view own profile, coaches can view all

**Update Profile** - `PUT /api/users/{id}`
- Updatable fields: name, email, phone, avatarUrl
- Authorization filter: `userId == authenticatedUserId`
- Email uniqueness validation

### Security Measures

- Input validation using FluentValidation
- SQL injection prevention via parameterized queries (Entity Framework)
- XSS protection through output encoding
- CORS configured for known client origins only
- Rate limiting on authentication endpoints (10 requests/minute)
- HTTPS enforced in production

## Architecture & Design

### Patterns & Principles

**Clean Architecture**
- Core domain models independent of infrastructure
- Use cases encapsulated in application layer
- Infrastructure (EF Core, SignalR) in outermost layer

**Repository Pattern**
- Abstraction over data access for testability
- Generic repository with specialized implementations

**Unit of Work**
- Transaction management across multiple repositories
- Ensures data consistency for complex operations

**Dependency Injection**
- Constructor injection throughout
- Service lifetimes: Singleton (caching), Scoped (DbContext), Transient (business logic)

### Project Structure
```
FAIR.API/
├── Controllers/          # HTTP endpoints
├── Hubs/                 # SignalR hubs
├── Middleware/           # Custom middleware (auth, logging)
├── Models/
│   ├── Entities/         # Domain models
│   └── DTOs/             # Data transfer objects
├── Services/             # Business logic
├── Repositories/         # Data access layer
├── Infrastructure/
│   ├── Data/             # DbContext, migrations
│   └── Config/           # Startup configuration
```

## Tech Stack

**Framework & Runtime**
- ASP.NET Core 8.0
- C# 12

**Database**
- Microsoft SQL Server
- Entity Framework Core 8.0 with migrations
- Npgsql provider

**Authentication & Authorization**
- JWT Bearer authentication (Microsoft.AspNetCore.Authentication.JwtBearer)
- BCrypt.Net for password hashing

**Real-Time Communication**
- SignalR Core

**Tooling**
- Swagger/OpenAPI (Swashbuckle) for documentation
- Serilog for structured logging

## Setup & Run

### Prerequisites

- .NET 8 SDK
- Microsoft SQL Server

### Configuration

Create `appsettings.Development.json`:
```json
{
  "ConnectionStrings": {
    "DefaultConnection": "Host=localhost;Database=fair_db;Username=postgres;Password=yourpassword"
  },
  "Jwt": {
    "Secret": "your-256-bit-secret-key-here-minimum-32-characters",
    "Issuer": "FAIR.API",
    "Audience": "FAIR.Clients",
    "ExpirationMinutes": 60
  },
  "Redis": {
    "Configuration": "localhost:6379"
  }
}
```

### Database Setup
```bash
# Apply migrations
dotnet ef database update

# Seed initial data (optional)
dotnet run --seed
```

### Running the Application
```bash
# Development
dotnet run --project FAIR.API

# Production
dotnet publish -c Release
dotnet FAIR.API.dll
```

### Docker
```bash
docker-compose up -d
```

API available at `https://localhost:5001`, Swagger UI at `https://localhost:5001/swagger`

## API Overview

### Authentication
```http
POST /api/auth/register
POST /api/auth/login
POST /api/auth/refresh-token
```

### Reports
```http
GET    /api/reports
GET    /api/reports/{id}
POST   /api/reports
PUT    /api/reports/{id}
DELETE /api/reports/{id}
GET    /api/reports/rankings
```

### Users
```http
GET /api/users/{id}
PUT /api/users/{id}
GET /api/users/profile
```

### Chat
```http
GET  /api/chat/history/{userId}
GET  /api/chat/unread
POST /api/chat/mark-read
```

### SignalR Hub

Connect to `/hubs/chat` with JWT in query string: `?access_token=<token>`

## Best Practices

### Code Quality

- **Async/Await**: All I/O operations are asynchronous to avoid thread blocking
- **Cancellation Tokens**: Passed through async call chains for graceful shutdown
- **Nullable Reference Types**: Enabled project-wide to prevent null reference exceptions
- **Error Handling**: Global exception handler with standardized error responses
- **Validation**: Input validated at controller level, business rules in service layer

### Database

- Indexes on foreign keys and frequently queried columns (PlayerId, CoachId, CreatedAt)
- Migrations version controlled for reproducible deployments
- Connection pooling configured for optimal throughput
- Query optimization using `AsNoTracking()` for read-only operations

### Security

- Secrets managed via User Secrets (dev) and environment variables (prod)
- Parameterized queries prevent SQL injection
- Authorization checked at both controller and service layers
- Sensitive data excluded from logs

### Performance

- Response caching for rankings and user profiles
- Database query result caching with appropriate invalidation
- SignalR message batching for high-throughput scenarios
- Pagination enforced on all collection endpoints


---

**License**: MIT  
**Maintainer**: Backend Engineering Team  
**API Version**: 1.0.0
