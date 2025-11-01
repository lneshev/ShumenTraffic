# ShumenTraffic - Architecture Overview

## System Architecture

ShumenTraffic is a web application for tracking live buses in Shumen, Bulgaria. The system consists of:

- **Frontend**: Next.js with Leaflet.js for map visualization
- **Backend**: ASP.NET Core Web API
- **Database**: MSSQL Server
- **Version Control**: Git/GitHub

## Key Architectural Decisions

### 1. Route Representation with Waypoints

**Problem**: Bus routes don't follow straight lines between stops. They navigate through streets, making turns, etc.

**Solution**: 
- `RouteStop` table stores all points on a route (both bus stops and waypoints)
- `BusStopId` is nullable:
  - When NOT NULL: Actual bus stop (passengers board/alight)
  - When NULL: Waypoint (defines route path between stops)
- `StopOrder` is int (sequential ordering of all points in route)
- Each `RouteStop` has `Latitude` and `Longitude` coordinates

**Benefits**:
- Single table for all route points
- Accurate visual representation on map
- Supports complex route paths

### 2. Dual-Mode Bus Position System

**Problem**: Initially, we don't have real GPS data from buses. Later, we want to add it without major refactoring.

**Solution**: Server-side logic determines position mode:

**Mode 1: Calculated (Current)**
- Server calculates position based on schedule
- Returns progress percentage between two stops
- Client interpolates exact position along polyline using waypoints

**Mode 2: Real GPS (Future)**
- Server receives GPS coordinates from buses
- Returns exact latitude/longitude
- Client displays position directly

**API Response**:
```json
// Calculated mode
{
  "positionType": "calculated",
  "progressPercentage": 0.52,
  "currentStopOrder": 1,
  "nextStopOrder": 2
}

// Real GPS mode
{
  "positionType": "real",
  "latitude": 43.2805,
  "longitude": 26.4970
}
```

**Benefits**:
- Easy migration path to real GPS
- Server controls logic, client just renders
- No client code changes needed when switching modes

### 3. Many-to-Many Transportation Company ↔ Bus Line

**Problem**: A bus line can be operated by multiple companies, and a company can operate multiple lines.

**Solution**:
- `TransportationCompanyBusLine` junction table
- Supports flexible business relationships
- Future-ready for multi-company scenarios

### 4. Schedule Calculation

**Approach**:
- Primary: Calculate approximate times based on distance between stops
- Secondary: Allow admins to manually edit times
- Future: Aggregate real-time user data for refinement

**Implementation**:
- `EstimatedMinutesFromStart` stored only for actual bus stops
- Waypoints have NULL for this field
- Client uses this to calculate progress percentage

### 5. Route Variations

**Support for**:
- Weekday routes
- Saturday routes
- Sunday routes

**Implementation**:
- `Schedule.DayType` field: "Weekday", "Saturday", "Sunday"
- Same physical route can have different schedules for different day types
- Allows different timetables per day type while maintaining the same route structure

## Data Flow

### Live Bus Position Display

1. **Frontend requests**: `GET /api/routes/{routeId}/buses/{busId}/current-position`

2. **Backend processes**:
   - Checks if real GPS data available
   - If yes: Returns GPS coordinates
   - If no: Calculates position from schedule

3. **Frontend receives**:
   - Position data with mode flag
   - If calculated: Also receives progress percentage and stop info

4. **Frontend renders**:
   - If real GPS: Display pin at coordinates
   - If calculated: Fetch waypoints, interpolate position, display pin

### Route Display

1. **Frontend requests**: Route with all waypoints
2. **Backend returns**: All `RouteStop` records (stops + waypoints) ordered by `StopOrder`
3. **Frontend renders**: Polyline using all coordinates

## Database Schema Highlights

### RouteStop Table
- Stores both bus stops and waypoints
- `BusStopId` nullable (NULL = waypoint)
- `Latitude`, `Longitude` for all points
- `StopOrder` int for sequential ordering
- `EstimatedMinutesFromStart` nullable (only for stops)

### Indexes
- `IX_RouteStops_RouteId_StopOrder`: For efficient route waypoint retrieval
- `IX_RouteStops_BusStopId`: For stop lookups
- Other standard FK indexes

## Scalability Considerations

### For 1000+ Concurrent Users

1. **Stateless API**: All requests are independent
2. **Efficient queries**: Indexed lookups for routes and stops
3. **Client-side computation**: Position interpolation happens on client
4. **Caching**: Waypoints can be cached on client
5. **Polling**: Position updates polled every 5-10 seconds (not real-time)

### Future Optimizations

- WebSocket for real-time updates
- Redis caching for frequently accessed routes
- Database read replicas
- CDN for static assets

## Security

### Authentication & Authorization
- **User Management**: ASP.NET Core Identity (IdentityUser, IdentityRole)
- **Authentication**: Username/password with built-in hashing
- **Authorization**: Role-based access control (RBAC)
- **Password Security**: Built-in hashing and lockout policies
- **Database Integration**: Identity tables integrated into ShumenTrafficDbContext

### Data Protection
- Input validation: All API inputs validated
- SQL Injection prevention: EF Core parameterized queries
- CORS: Configured for frontend domain
- HTTPS: Required in production

## Technology Stack

| Layer | Technology |
|-------|-----------|
| Frontend | Next.js, React, TypeScript, Leaflet.js, TailwindCSS |
| Backend | ASP.NET Core 9.0, C# |
| Database | MSSQL Server |
| ORM | Entity Framework Core 9.0.10 |
| Authentication | ASP.NET Core Identity |
| Version Control | Git/GitHub |

## File Structure

```
ShumenTraffic/
├── Client/                    # Next.js frontend
├── Server/                    # ASP.NET Core backend
│   ├── ShumenTraffic.WebAPI/  # Web API project
│   └── ShumenTraffic.Data/    # Data access layer
├── Docs/                      # Documentation
│   ├── DATABASE_SCHEMA.md
│   ├── BUS_POSITION_SYSTEM.md
│   ├── ARCHITECTURE.md
│   └── ...
└── .git/                      # Git repository
```

## Development Workflow

1. **Task-by-task implementation**: Complete and test each task individually
2. **Consistent BE/FE contracts**: API responses match frontend expectations
3. **Database migrations**: Use EF Core migrations for schema changes
4. **Git commits**: Meaningful commit messages for each logical change
5. **Testing**: Unit tests for critical logic, integration tests for API

## Next Steps

See `PROJECT_PLAN.md` for detailed task list and phases.

