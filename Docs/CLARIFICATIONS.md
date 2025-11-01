# ShumenTraffic - Project Clarifications

## User Answers to Initial Questions

### 1. Authentication
- **Decision**: Username/password authentication only
- **Scope**: Admin users only (user will be the only admin initially)
- **No OAuth**: Not needed at this stage

### 2. Bus Position Calculation
- **Primary Method**: Calculate based on distance between stops and average speed
- **Secondary Method**: Allow admins to manually edit calculated times
- **Future Enhancement**: Collect real-time data from anonymous users for aggregation and refinement at a later stage

### 3. Data Source
- **Initial Data**: Web scraping from https://shumenpat.com/razpisanie.htm
- **Parsing**: Scrape and parse data into respective database entities
- **Admin Modifications**: Admins can modify all data via CRUD pages

### 4. Map Features
- **All features required**: No simulations or mockups
- **Markers**: Display all bus stops with markers
- **Route Lines**: Show route lines for selected bus lines
- **Bus Positions**: Display current bus positions (calculated based on schedule)

### 5. Database
- **Setup**: Local MSSQL instance (user has it set up)

### 6. Deployment
- **Current Stage**: Local development only
- **Hosting**: Defer for later phases

---

## Implementation Guidelines

### 7. Implementation Approach
- **Task-by-task**: Complete and test each task individually before moving to the next
- **Not phase-by-phase**: This allows for easier testing and debugging
- **Progress tracking**: Each task will be marked as complete before moving to the next

### 8. UML Class Diagram (Phase 2, Task 1)
- **Tool**: Mermaid diagram
- **Format**: Rendered inline in markdown, editable markdown format
- **Editability**: User can edit the markdown file and upload it back for review
- **Content**: All entities, relationships, and key attributes

### 9. Phase Reorganization
- **New Order**: Phase 5 (Admin Pages) moved after Phase 4 (Frontend - Layout & Navigation)
- **Reason**: Admin pages are foundational for data management
- **Updated Phases**: All subsequent phases renumbered accordingly

### 10. Admin Pages - UI/UX Planning (Phase 5, Task 4)
- **Task**: Plan UI/UX for data entry forms
- **Goal**: Determine best tools and components for easy create/edit/delete operations
- **Timing**: Discussion will occur when Phase 5 begins
- **Scope**: Should cover all entities (Transportation Companies, Zones, Bus Stops, Bus Lines, Routes, Schedules)

### 11. Integration & Testing - BE/FE Contracts (Phase 10, Task 1)
- **Requirement**: Ensure consistent contracts between Backend and Frontend
- **Approach**: Verify API responses match frontend expectations throughout development
- **Timing**: Consider this from the start of backend API development
- **Goal**: Prevent integration issues and ensure smooth data flow

### 12. Extensions & MCP Servers
- **Status**: User will notify if access to extensions or MCP servers is needed for testing or other purposes

### 13. UI Color Scheme
- **Color scheme**: blue
---

## Project Constraints

### Scope
- **Only specified features**: No additional features beyond those outlined
- **All features functional**: No simulations or mockups
- **Mobile-friendly**: Responsive design required throughout

### Performance
- **Scalability**: Support for 1000+ concurrent users

### Data Handling
- **Route Variations**: Support different routes for weekdays, Saturdays, and Sundays
- **Multiple Companies**: Architecture should support multiple transportation companies (even if only one initially)
- **Temporary Changes**: Support for temporary route changes (holidays, road constructions, etc.)

---

## File Structure
- `/Client` - Frontend (Next.js)
- `/Server` - Backend (ASP.NET Core)
- `/Docs` - Documentation

---

## Next Steps
1. Await confirmation that all clarifications are understood
2. Begin Phase 1: Project Setup & Infrastructure
3. Proceed task-by-task with testing and debugging at each step

