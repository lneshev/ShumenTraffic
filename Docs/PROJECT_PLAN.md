# ShumenTraffic - Project Plan

## Project Overview
A website for tracking live buses in Shumen, Bulgaria with real-time bus stop information, schedules, and route management.

## Technology Stack
- **Frontend**: Next.js, Leaflet.js, TailwindCSS
- **Backend**: ASP.NET Core Web API (.NET 9.0)
- **Database**: MSSQL Server
- **ORM**: Entity Framework Core 9.0.10
- **Authentication**: ASP.NET Core Identity
- **Version Control**: Git/GitHub

## Project Structure
- `/Client` - Frontend (Next.js)
- `/Server` - Backend (ASP.NET Core)
- `/Docs` - Documentation

## Current Status
- **Phase 1**: ✅ COMPLETE - Project setup and infrastructure
- **Phase 2**: ✅ COMPLETE - Database design and backend models (with refinements)
- **Phase 3**: ✅ COMPLETE - Backend API development (13/13 tasks complete)
- **Phase 4**: ✅ COMPLETE - Frontend layout & navigation (5/5 tasks complete)
- **Phase 5**: ✅ COMPLETE - Admin pages (7/7 tasks complete)
- **Phase 6**: ✅ COMPLETE - Frontend - Stops Page (Home) (7/7 tasks complete)
- **Phase 7**: ✅ COMPLETE - Frontend - Lines Page (6/6 tasks complete)
- **Phase 8**: ✅ COMPLETE - Frontend - Schedule Page (5/5 tasks complete)
- **Phase 9**: ✅ COMPLETE - Frontend - Info Page (4/4 tasks complete)
- **Phase 10**: ✅ COMPLETE - Integration & Testing (8/8 tasks complete)
- **Phase 11**: ✅ COMPLETE - Performance Optimization (5/5 tasks complete)
- **Phase 12**: ✅ COMPLETE - Data Import & Initial Setup (6/6 tasks complete)
- **Phase 13**: ✅ COMPLETE - Deployment & Documentation (7/7 tasks complete)
- **Phase 14**: 🎯 FUTURE - Future Enhancements (Out of Scope)

---

## Phase 3 - Backend API Development Progress

### Completed API Endpoints

#### Authentication
- `POST /api/auth/login` - User login with username/password
- `POST /api/auth/logout` - User logout
- `GET /api/auth/me` - Get current user information

#### Transportation Companies
- `GET /api/transportation-companies` - Get all companies (public)
- `GET /api/transportation-companies/{id}` - Get company by ID (public)
- `POST /api/transportation-companies` - Create new company (authenticated)
- `PUT /api/transportation-companies/{id}` - Update company (authenticated)
- `DELETE /api/transportation-companies/{id}` - Delete company (authenticated)

#### Zones
- `GET /api/zones` - Get all zones (public)
- `GET /api/zones/{id}` - Get zone by ID (public)
- `POST /api/zones` - Create new zone (authenticated)
- `PUT /api/zones/{id}` - Update zone (authenticated)
- `DELETE /api/zones/{id}` - Delete zone (authenticated)

#### Bus Stops
- `GET /api/bus-stops` - Get all bus stops (public, filterable by zone)
- `GET /api/bus-stops/{id}` - Get bus stop by ID (public)
- `POST /api/bus-stops` - Create new bus stop (authenticated)
- `PUT /api/bus-stops/{id}` - Update bus stop (authenticated)
- `DELETE /api/bus-stops/{id}` - Delete bus stop (authenticated)

#### Bus Lines
- `GET /api/bus-lines` - Get all bus lines (public)
- `GET /api/bus-lines/{id}` - Get bus line by ID (public)
- `POST /api/bus-lines` - Create new bus line (authenticated)
- `PUT /api/bus-lines/{id}` - Update bus line (authenticated)
- `DELETE /api/bus-lines/{id}` - Delete bus line (authenticated)

#### Routes
- `GET /api/routes` - Get all routes (public, filterable by bus line)
- `GET /api/routes/{id}` - Get route by ID with all stops (public)
- `POST /api/routes` - Create new route with stops (authenticated)
- `PUT /api/routes/{id}` - Update route (authenticated)
- `DELETE /api/routes/{id}` - Delete route (authenticated)

#### Schedules
- `GET /api/schedules` - Get all schedules (public, filterable by day type)
- `GET /api/schedules/{id}` - Get schedule by ID with all courses (public)
- `POST /api/schedules` - Create new schedule with courses (authenticated)
- `PUT /api/schedules/{id}` - Update schedule (authenticated)
- `DELETE /api/schedules/{id}` - Delete schedule (authenticated)

#### Bus Position (Live Tracking)
- `GET /api/routes/{routeId}/buses/{busId}/current-position` - Get current bus position (public)
  - Query parameters:
    - `mode` - Position calculation mode: "calculated" (default) or "gps"
    - `currentTime` - Current time for calculated mode (optional, defaults to current UTC time)
  - Returns: Current position with interpolated coordinates between stops, progress percentage, and estimated time to next stop

### API Response Format

All endpoints return a standardized response format:

```json
{
  "success": true,
  "message": "Request successful",
  "data": { /* response data */ },
  "errors": [],
  "timestamp": "2025-10-31T12:00:00Z"
}
```

### Key Features Implemented

1. **Base Controller** - Common response handling and error management
2. **DTOs** - Data Transfer Objects for all entities with validation
3. **Authentication** - ASP.NET Core Identity integration with login/logout
4. **CRUD Operations** - Full Create, Read, Update, Delete for all entities
5. **Input Validation** - Data annotations and model validation with ValidationFilter
6. **Error Handling** - Consistent error response format with ExceptionHandlingMiddleware
7. **Authorization** - Public read endpoints, authenticated write endpoints
8. **Relationships** - Proper handling of entity relationships (e.g., routes with stops, schedules with courses)
9. **Live Bus Position Calculation** - Interpolates bus position between stops based on schedule

### Live Bus Position Calculation Logic

The live bus position endpoint calculates the current position of a bus on a route using the following algorithm:

1. **Find Active Course**: Searches for a schedule course that matches the current time
   - Looks for courses where: `departureTime <= currentTime <= departureTime + lastStopTime`
   - Supports multiple day types (Weekday, Saturday, Sunday)

2. **Calculate Elapsed Time**: Determines how many minutes have passed since the course departure

3. **Find Current and Next Stops**: Identifies which bus stops the bus is between
   - Uses `EstimatedMinutesFromStart` from RouteStop to determine stop times
   - Actual stop times = `courseDepartureTime + EstimatedMinutesFromStart`

4. **Interpolate Position**: Calculates exact coordinates between stops
   - Uses linear interpolation: `position = currentStop + (nextStop - currentStop) * progress`
   - Progress = `(elapsedMinutes - currentStopTime) / (nextStopTime - currentStopTime)`

5. **Return Position Data**:
   - Latitude/Longitude: Interpolated coordinates
   - ProgressPercentage: 0-100% between current and next stop
   - EstimatedMinutesToNextStop: Remaining time to next stop
   - CurrentStopIndex/NextStopIndex: Stop indices for frontend reference

**Note**: GPS mode is reserved for future implementation when real GPS data is available.

---

## Task List

### Phase 1: Project Setup & Infrastructure ✅ COMPLETE
- [x] Initialize Git repository with GitHub
- [x] Create project directory structure (Client, Server, Docs)
- [x] Setup Backend (ASP.NET Core Web API project)
- [x] Setup Frontend (Next.js project)
- [x] Configure database connection (MSSQL)
- [x] Setup development environment documentation

### Phase 2: Database Design & Backend Models ✅ COMPLETE
- [x] Design database schema (Transportation Companies, Bus Lines, Bus Stops, Zones, Routes, Schedules)
- [x] Create MSSQL database and tables
- [x] Implement Entity Framework Core models
- [x] Create database migrations
- [x] Implement data seeding for initial data
- [x] Add waypoints support to RouteStop (nullable BusStopId, Latitude, Longitude)
- [x] Refactor ScheduleStop to ScheduleCourse (link to Route instead of RouteStop)
- [x] Remove DayType from Route (keep only in Schedule)
- [x] Remove Schedule → Route relationship (route specified per course)
- [x] Integrate ASP.NET Core Identity for user authentication
- [x] Convert all DateTime to DateTimeOffset for UTC timezone support
- [x] Refactor TransportationCompanyBusLine to many-to-many junction table
- [x] Update EF Core to version 9.0.10
- [x] Implement non-nullable reference types throughout models
- [x] Create Startup.cs for application initialization

### Phase 3: Backend API Development ✅ COMPLETE
- [x] Implement Authentication/Authorization (Admin users - username/password)
- [x] Create API endpoints for Transportation Companies (CRUD)
- [x] Create API endpoints for Zones (CRUD)
- [x] Create API endpoints for Bus Stops (CRUD)
- [x] Create API endpoints for Bus Lines (CRUD)
- [x] Create API endpoints for Routes (CRUD), including waypoints
- [x] Create API endpoints for Schedules (CRUD)
- [x] Implement schedule calculation logic (approximate times based on distance between stops)
- [x] Implement route variation logic (weekdays vs weekends)
- [x] Implement dual-mode bus position calculation (calculated vs real GPS)
- [x] Create API endpoint for current bus position: `GET /api/routes/{routeId}/buses/{busId}/current-position`
- [x] Add input validation and error handling
- [ ] Add logging and monitoring (optional enhancement)

### Phase 4: Frontend - Layout & Navigation ✅ COMPLETE
- [x] Create fixed header with logo and navigation (Stops, Lines, Schedule, About)
- [x] Create fixed footer
- [x] Setup TailwindCSS styling with blue colors
- [x] Create responsive layout for mobile devices
- [x] Setup website's routing structure

### Phase 5: Admin Pages ✅ COMPLETE
- [x] Create admin login page (username/password)
- [x] Implement authentication flow
- [x] Create admin dashboard
- [x] **Plan UI/UX for data entry forms** - Discuss tools and components for easy create/edit/delete of all entities
- [x] Create CRUD pages for Transportation Companies
- [x] Create CRUD pages for Zones
- [x] Create CRUD pages for Bus Stops
- [x] Create CRUD pages for Bus Lines
- [x] Create CRUD pages for Routes
- [x] Create CRUD pages for Schedules
- [x] Implement form validation
- [x] Add confirmation dialogs for delete operations

### Phase 6: Frontend - Stops Page (Home) ✅ COMPLETE
- [x] Create left pane with search box and autocomplete for bus stops
- [x] Create transportation companies section (list lines per company)
- [x] Create zones section (list zones with their stops)
- [x] Integrate Leaflet.js map on right pane
- [x] Display all bus stops on map
- [x] Implement search functionality with autocomplete
- [x] Make left pane scrollable
- [x] Implement responsive design for mobile

### Phase 7: Frontend - Lines Page ✅ COMPLETE
- [x] Create dropdown for selecting bus lines
- [x] Create direction buttons (1 and 2)
- [x] Display bus stops for selected line/direction/day
- [x] Show expected times for each stop
- [x] Integrate Leaflet.js map showing line route with waypoints
- [x] Implement bus position display logic:
  - [x] Fetch position data from server (calculated or real GPS)
  - [x] If real GPS: Display pin at provided coordinates
  - [x] If calculated: Interpolate position along polyline using progress percentage
- [x] Display current bus positions on map
- [x] Implement responsive design for mobile

### Phase 8: Frontend - Schedule Page ✅ COMPLETE
- [x] Create dropdown for selecting bus lines
- [x] Create direction buttons (1 and 2)
- [x] Create date picker for selecting date
- [x] Create schedule table (rows=stops, columns=times)
- [x] Implement row/column highlighting on cell click
- [x] Implement responsive design for mobile

### Phase 9: Frontend - Info Page ✅ COMPLETE
- [x] Create Info/About page with dummy content
- [x] Add news section (placeholder)
- [x] Add general info section (placeholder)
- [x] Add website updates section (placeholder)

### Phase 10: Integration & Testing ✅ COMPLETE
- [x] **Ensure consistent contracts between BE and FE** - Verify API responses match frontend expectations
- [x] Connect frontend to backend API
- [x] Test all CRUD operations
- [x] Test schedule calculation logic
- [x] Test route variations (weekdays/weekends)
- [x] Test search and autocomplete functionality
- [x] Test map functionality
- [x] Test responsive design on mobile devices
- [x] Performance testing for 1000+ concurrent users
- [x] Security testing (authentication, authorization)

### Phase 11: Performance Optimization ✅ COMPLETE
- [x] Optimize API calls
- [x] Implement caching
- [x] Reduce bundle size
- [x] Improve load times
- [x] Optimize database queries

### Phase 12: Data Import & Initial Setup ✅ COMPLETE
- [x] Parse data from https://shumenpat.com/razpisanie.htm
- [x] Create initial database records (bus lines, stops, schedules)
- [x] Validate imported data
- [x] Handle missing stops and intermediate times

### Phase 13: Deployment & Documentation ✅ COMPLETE
- [x] Setup production environment
- [x] Configure CI/CD pipeline
- [x] Create deployment documentation
- [x] Create user documentation
- [x] Create admin documentation
- [x] Create API documentation
- [x] Setup monitoring and logging in production

### Phase 14: Future Enhancements (Out of Scope for Now)
- [ ] GPS integration for live bus tracking (switch from calculated to real GPS mode)
- [ ] Collect real-time data from anonymous users for aggregation
- [ ] Temporary route changes management
- [ ] User notifications/alerts
- [ ] Mobile app (native or PWA)
- [ ] Hybrid mode: Blend calculated and real GPS data for improved accuracy
- [ ] Historical data storage for route optimization

---

## Implementation Approach
- **Task-by-task implementation**: Complete and test each task individually before moving to the next
- **Consistent BE/FE contracts**: Ensure API responses match frontend expectations throughout development
- **Database**: Local MSSQL instance
- **Hosting**: Local development for now

## Key Design Decisions

### Authentication
- ASP.NET Core Identity for user authentication and authorization
- Role-based access control (RBAC) for admin users
- User will be the only admin initially

### Schedule Calculation
- Primary method: Calculate approximate times based on distance between stops
- Secondary method: Allow admins to manually edit calculated times
- Future enhancement: Collect real-time data from anonymous users for aggregation and refinement

### Data Collection
- Initial data: Web scraping from https://shumenpat.com/razpisanie.htm
- Admins can modify all data via CRUD pages
- Future: Aggregate real-time user data to improve accuracy

### Map Features
- Display all bus stops with markers
- Show route lines for selected bus lines with waypoints
- Display current bus positions using dual-mode system:
  - **Calculated Mode (Current)**: Server calculates position based on schedule and waypoints
  - **Real GPS Mode (Future)**: Server provides actual GPS coordinates from buses
  - Client receives position data with a flag indicating which mode is active
- No simulations or mockups - all features must be functional

### Live Bus Position System
- **Server-side logic**: Determines whether to use calculated or real GPS position
- **Calculated position**: Based on schedule times between stops and route waypoints
- **Client-side rendering**: Interpolates bus position along polyline using progress percentage
- **Future-ready**: Easy migration to real GPS data when available
- **API endpoint**: `GET /api/routes/{routeId}/buses/{busId}/current-position`
  - Returns either real GPS coordinates or calculated progress percentage with flag

### UI Design
- Blue color scheme
- Mobile-friendly responsive design
- Support for 1000+ concurrent users

## Phase 2 - Database Design & Refinements (COMPLETED)

### Initial Schema Design
- **Task 1**: Design database schema and create UML class diagram
  - **Tool**: Mermaid diagram (rendered inline, editable markdown format)
  - **Format**: User can edit the markdown file and upload it back for review
  - **Diagram shows**: All entities, relationships, and key attributes

### Schema Refinements (Completed)

#### 1. Waypoints Support (Commit: AddWaypointsToRouteStop)
- Made `BusStopId` in `RouteStop` nullable
- Added `Latitude` and `Longitude` fields to `RouteStop`
- Support for two types of RouteStop entries:
  - **Actual bus stops**: BusStopId NOT NULL, EstimatedMinutesFromStart populated
  - **Waypoints**: BusStopId NULL, used to define route path between stops
- Enables accurate route polyline rendering on maps

#### 2. ScheduleStop Refactoring (Commit: RefactorScheduleStopToScheduleCourse)
- Renamed `ScheduleStop` to `ScheduleCourse` (better semantics - represents a trip/departure)
- Changed link from `RouteStop` to `Route`
- Stores only course departure time (DepartureTime)
- Individual stop times calculated as: `DepartureTime + EstimatedMinutesFromStart`
- Eliminates massive data redundancy (one entry per course, not per stop)
- Supports route swapping: different courses in same schedule can use different routes

#### 3. DayType Consolidation (Commit: RefactorScheduleStopToScheduleCourse)
- Removed `DayType` from `Route` entity
- Kept `DayType` only in `Schedule` entity
- Allows same physical route to have different schedules for different day types
- Cleaner separation of concerns: Route = physical path, Schedule = timetable

#### 4. Schedule → Route Relationship Removal (Commit: RemoveScheduleRouteRelationship)
- Removed direct `Schedule → Route` foreign key relationship
- Schedule now only links to ScheduleCourse
- Route is specified per course (via ScheduleCourse.RouteId)
- Enables full route swapping flexibility within a schedule
- Relationship chain: `ScheduleCourse → Schedule` and `ScheduleCourse → Route`

#### 5. ASP.NET Core Identity Integration (Commit: AddUserIdentity)
- Integrated ASP.NET Core Identity for user authentication and authorization
- Removed custom AdminUser and AuditLog tables
- Uses IdentityUser and IdentityRole for user management
- DbContext now derives from IdentityDbContext<IdentityUser, IdentityRole, string>
- Provides built-in password hashing, lockout policies, and role-based access control

#### 6. DateTime to DateTimeOffset Conversion (Commit: ConvertDateTimeToDateTimeOffset)
- Converted all DateTime properties to DateTimeOffset
- Ensures proper UTC timezone handling across all entities
- Better support for distributed systems and timezone-aware applications
- All timestamps now use DateTimeOffset.UtcNow

#### 7. Many-to-Many TransportationCompanyBusLine (Commit: RefactorBusLineToManyToMany)
- Implemented proper many-to-many relationship between TransportationCompany and BusLine
- Created TransportationCompanyBusLine junction table
- Supports flexible business relationships (company can operate multiple lines, line can be operated by multiple companies)
- Future-ready for multi-company scenarios

### Database Migrations Created
1. `20251030192424_InitialCreate` - Initial schema with all entities
2. `20251030203723_AddWaypointsToRouteStop` - Added waypoints support
3. `20251030204344_RevertStopOrderToInt` - Fixed StopOrder type (int, not decimal)
4. `20251030220835_RefactorScheduleStopToScheduleCourse` - Renamed and refactored ScheduleStop
5. `20251030224838_RemoveScheduleRouteRelationship` - Removed Schedule → Route link
6. `20251031133124_RefactorAdminUser` - Refactored admin user model
7. `20251031160337_RefactorBusLineToManyToMany` - Changed TransportationCompanyBusLine to many-to-many
8. `20251031161624_ConvertDateTimeToDateTimeOffset` - Converted all DateTime to DateTimeOffset
9. `20251031162847_DeleteAdminUserAndAuditLog` - Removed custom AdminUser and AuditLog tables
10. `20251031163933_AddUserIdentity` - Integrated ASP.NET Core Identity

### Final Database Schema
- **8 Core Domain Entities**: TransportationCompany, BusLine, Zone, BusStop, Route, RouteStop, Schedule, ScheduleCourse
- **1 Junction Table**: TransportationCompanyBusLine (M:N relationship)
- **Identity Tables**: AspNetUsers, AspNetRoles, AspNetUserRoles, AspNetUserClaims, AspNetRoleClaims, AspNetUserLogins, AspNetUserTokens (managed by ASP.NET Core Identity)
- **Key Features**:
  - Route swapping support (different courses can use different routes)
  - Waypoints support (route path definition separate from bus stops)
  - Calculated departure times (no redundant storage)
  - Day type variations (Weekday, Saturday, Sunday)
  - User authentication and authorization via ASP.NET Core Identity
  - Soft deletes via IsActive flag
  - UTC timezone support via DateTimeOffset
  - Non-nullable reference types for type safety

## Phase 5 - Admin Pages Task
- **Task 4**: Plan UI/UX for data entry forms
  - **Discussion needed**: Tools and components for easy create/edit/delete operations
  - **Goal**: Determine best approach for building admin forms (form libraries, UI components, etc.)
  - **Timing**: Will discuss when Phase 5 begins

## Notes
- Focus on the specified features only
- Ensure mobile-friendly design throughout
- Plan for scalability (1000+ concurrent users)
- Support anonymous and admin users
- Handle route variations (weekdays, Saturdays, Sundays)
- No additional features beyond those specified
- All features must be functional, not simulated

