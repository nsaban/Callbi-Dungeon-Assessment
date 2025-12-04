# Callbi Dungeon Assessment

## Project Overview
Technical assessment project for Callbi Senior Developer position. Full-stack application with C# .NET backend and React TypeScript frontend for dungeon map visualization and pathfinding.

## Project Structure
```
Callbi-Dungeon-Assessment/
│
├── README.md
├── docker-compose.yml
├── DungeonAssessment.sln
│
├── Dungeon.Api/                    # ASP.NET Core Web API
│   ├── Dungeon.Api.csproj
│   ├── Program.cs
│   ├── appsettings.json
│   ├── Dockerfile
│   ├── Endpoints/                  # FastEndpoints implementation
│   │   └── Maps/
│   │       ├── CreateMapEndpoint.cs
│   │       ├── GetAllMapsEndpoint.cs
│   │       ├── GetMapByIdEndpoint.cs
│   │       └── ComputePathEndpoint.cs
│   ├── Middleware/
│   │   └── ErrorHandlingMiddleware.cs
│   ├── Models/                     # Request/Response DTOs
│   │   ├── CreateMapRequest.cs
│   │   ├── CreateMapResponse.cs
│   │   ├── GetMapByIdRequest.cs
│   │   ├── MapResponse.cs
│   │   ├── PathResponse.cs
│   │   └── PointModel.cs
│   ├── Validators/                 # FluentValidation validators
│   │   ├── CreateMapRequestValidator.cs
│   │   ├── GetMapByIdRequestValidator.cs
│   │   └── PointModelValidator.cs
│   └── Migrations/                 # Entity Framework migrations
│
├── Dungeon.Application/            # Business Logic Layer
│   ├── Dungeon.Application.csproj
│   ├── Interfaces/
│   │   ├── IAStarPathfinder.cs
│   │   ├── IMapRepository.cs
│   │   ├── IMapService.cs
│   │   └── IPathfinderService.cs
│   ├── Models/                     # Domain models
│   │   └── Point.cs
│   ├── Services/
│   │   ├── MapService.cs
│   │   └── PathfinderService.cs
│   ├── Pathfinding/
│   │   ├── AStarPathfinder.cs
│   │   ├── Node.cs
│   │   └── GridHelpers.cs
│   └── Exceptions/
│       ├── MapNotFoundException.cs
│       └── InvalidMapException.cs
│
├── Dungeon.Infrastructure/         # Data Access Layer
│   ├── Dungeon.Infrastructure.csproj
│   ├── Database/
│   │   ├── DungeonDbContext.cs
│   │   └── Configurations/
│   ├── Entities/                   # Database entities
│   │   ├── DungeonMapEntity.cs
│   │   └── ObstacleEntity.cs
│   └── Repositories/
│       └── MapRepository.cs
│
├── Dungeon.Web/                    # React TypeScript Frontend
│   ├── package.json
│   ├── tsconfig.json
│   ├── index.html
│   ├── src/
│   │   ├── App.tsx                 # Main application component
│   │   ├── App.css                 # Application styling
│   │   └── main.tsx
│   └── public/
│
└── Tests/                          # Unit Tests
    ├── Dungeon.Api.UnitTests/          # API layer tests
    │   ├── Endpoints/
    │   │   └── Maps/
    │   │       ├── ComputePathEndpointTests.cs
    │   │       ├── CreateMapEndpointTests.cs
    │   │       ├── GetAllMapsEndpointTests.cs
    │   │       └── GetMapByIdEndpointTests.cs
    │   └── Validators/
    │       ├── CreateMapRequestValidatorTests.cs
    │       ├── GetMapByIdRequestValidatorTests.cs
    │       └── PointModelValidatorTests.cs
    └── Dungeon.Application.UnitTests/   # Business logic tests
        ├── Services/
        │   ├── MapServiceTests.cs
        │   └── PathfinderServiceTests.cs
        └── Pathfinding/
            ├── AStarPathfinderTests.cs
            └── GridHelpersTests.cs
```

## Setup Instructions

### Prerequisites
- .NET 8.0 SDK
- Npm
- Docker (optional, for containerized setup)

### Local Development Setup

1. **Clone the repository**
   ```bash
   git clone <repository-url>
   cd Callbi-Dungeon-Assessment
   ```

2. **Restore NuGet packages**
   ```bash
   dotnet restore DungeonAssessment.sln
   ```

3. **Run the application**
   ```bash
   dotnet run --project Dungeon.Api
   ```

4. **Run the web application**
   ```bash
   cd Dungeon.Web
   npm run dev


### Docker Setup (Alternative)

1. **Run with Docker Compose**
   ```bash
   docker-compose up -d
   ```

   This will start:
   - API container on port 5000

## API Endpoints

### Maps
- `POST /api/maps` - Create a new dungeon map
  - Request body: `{ "name": "Map Name", "width": 10, "height": 8, "startPosition": {"x": 0, "y": 0}, "goalPosition": {"x": 9, "y": 7}, "obstacles": [{"x": 2, "y": 3}] }`
- `GET /api/maps` - Get all maps
- `GET /api/maps/{id}` - Get map by ID

### Pathfinding
- `GET /api/maps/{id}/path` - Compute optimal path using stored map's start and goal positions
  - Uses A* algorithm to find shortest path avoiding obstacles
  - Returns: `{ "path": [{"x": 0, "y": 0}, ...], "distance": 12, "pathFound": true }`

## Usage

1. **Create a Map**: POST to `/api/maps` with map data
2. **Find Path**: GET from `/api/maps/{id}/path` to compute optimal path
3. **View Swagger Documentation**: Navigate to `/swagger` when running in development mode

## Testing

### Running Tests

**Run All Tests:**
```bash
dotnet test
```

**Run Specific Test Projects:**
```bash
# API Tests only
dotnet test Tests/Dungeon.Api.UnitTests/

# Application Tests only  
dotnet test Tests/Dungeon.Application.UnitTests/
```

**With Coverage:**
```bash
dotnet test --collect:"XPlat Code Coverage"
```

### Test Coverage

The solution includes comprehensive unit tests covering:

- **API Endpoints**: All FastEndpoints with various scenarios
- **Validators**: FluentValidation rules and edge cases
- **Services**: Business logic and exception handling
- **Pathfinding**: A* algorithm implementation and grid helpers
- **Repository Pattern**: Data access layer functionality

Tests use **NSubstitute** for mocking and **FluentAssertions** for readable assertions.

## Technologies Used

**Backend (C#/.NET 8.0)**
- ASP.NET Core 8.0 Web API
- FastEndpoints 5.31.0 with TypedResults
- Entity Framework Core with SQL Server
- FluentValidation for request validation
- A* Pathfinding Algorithm implementation

**Frontend (React/TypeScript)**
- React 18 with TypeScript
- Vite build tool
- Modern ES6+ features
- Responsive CSS Grid layout

**Architecture & Patterns**
- Clean Architecture with Domain-Driven Design
- Repository Pattern
- Dependency Injection
- CORS enabled for cross-origin requests
- Centralized error handling middleware

**Database Design**
- Normalized schema with separate obstacle entities
- Foreign key relationships for data integrity
- Entity Framework Core Code-First migrations

**Infrastructure**
- Docker & Docker Compose support
- SQL Server LocalDB for development
- Environment-specific configuration