# ShumenTraffic - Bus Position System Architecture

## Overview

The bus position system is designed to display the current location of buses on the map. It supports two modes:
1. **Calculated Mode** (Current): Position calculated server-side based on schedule
2. **Real GPS Mode** (Future): Actual GPS coordinates from buses

The system is designed for easy migration from calculated to real GPS data.

## Data Model

### RouteStop Table
Stores all points on a route (both bus stops and waypoints):

| Field | Type | Nullable | Purpose |
|-------|------|----------|---------|
| Id | int | No | Primary key |
| RouteId | int | No | Reference to route |
| BusStopId | int | **Yes** | Reference to bus stop (NULL = waypoint only) |
| Latitude | decimal(10,8) | No | GPS latitude coordinate |
| Longitude | decimal(11,8) | No | GPS longitude coordinate |
| StopOrder | int | No | Order in route (1-based) |
| EstimatedMinutesFromStart | int | **Yes** | Minutes from route start (only for actual stops) |
| CreatedAt | datetime | No | Creation timestamp |
| UpdatedAt | datetime | No | Last update timestamp |

**Key Points:**
- `BusStopId IS NOT NULL` → Actual bus stop (passengers board/alight)
- `BusStopId IS NULL` → Waypoint (defines route path between stops)
- `EstimatedMinutesFromStart` only populated for actual stops
- `StopOrder` maintains sequence of all points (stops and waypoints)

## Calculated Mode (Current Implementation)

### How It Works

1. **Server receives request**: `GET /api/routes/{routeId}/buses/{busId}/current-position`

2. **Server calculates position**:
   - Get current time
   - Find which two bus stops the bus is between
   - Calculate progress percentage: `(currentTime - stop1_time) / (stop2_time - stop1_time)`
   - Return progress data

3. **Server response**:
```json
{
  "busId": 5,
  "routeId": 1,
  "positionType": "calculated",
  "progressPercentage": 0.52,
  "currentStopOrder": 1,
  "nextStopOrder": 2,
  "currentStopId": 1,
  "nextStopId": 2,
  "timestamp": "2025-10-30T10:07:32Z"
}
```

4. **Client processes response**:
   - Fetch all waypoints between the two stops
   - Interpolate position along the polyline using progress percentage
   - Display bus pin at interpolated location

### Client-Side Interpolation

```typescript
function calculateBusPosition(waypoints, progressPercentage) {
  // waypoints: array of {latitude, longitude} between two stops
  // progressPercentage: 0.0 to 1.0
  
  // Calculate total polyline distance
  let totalDistance = 0;
  for (let i = 0; i < waypoints.length - 1; i++) {
    totalDistance += haversineDistance(waypoints[i], waypoints[i + 1]);
  }
  
  // Calculate target distance
  const targetDistance = totalDistance * progressPercentage;
  
  // Find position at target distance
  let currentDistance = 0;
  for (let i = 0; i < waypoints.length - 1; i++) {
    const segmentDistance = haversineDistance(waypoints[i], waypoints[i + 1]);
    if (currentDistance + segmentDistance >= targetDistance) {
      // Position is on this segment
      const segmentProgress = (targetDistance - currentDistance) / segmentDistance;
      return interpolatePoint(waypoints[i], waypoints[i + 1], segmentProgress);
    }
    currentDistance += segmentDistance;
  }
  
  return waypoints[waypoints.length - 1]; // End point
}
```

## Real GPS Mode (Future Implementation)

### How It Works

1. **Buses send GPS data** to server (via mobile app or device)

2. **Server stores latest position** for each bus

3. **Server receives request**: `GET /api/routes/{routeId}/buses/{busId}/current-position`

4. **Server returns GPS data**:
```json
{
  "busId": 5,
  "routeId": 1,
  "positionType": "real",
  "latitude": 43.2805,
  "longitude": 26.4970,
  "timestamp": "2025-10-30T10:07:32Z",
  "accuracy": 5.2
}
```

5. **Client processes response**:
   - Display bus pin at provided coordinates directly
   - No interpolation needed

## Client-Side Logic

```typescript
class BusPositionService {
  async getBusPosition(routeId: number, busId: number): Promise<BusPosition> {
    const response = await fetch(`/api/routes/${routeId}/buses/${busId}/current-position`);
    const data = await response.json();
    
    if (data.positionType === 'real') {
      // Real GPS - display directly
      return {
        latitude: data.latitude,
        longitude: data.longitude,
        source: 'gps'
      };
    } else if (data.positionType === 'calculated') {
      // Calculated - interpolate along polyline
      const waypoints = await this.getRouteWaypoints(
        routeId, 
        data.currentStopOrder, 
        data.nextStopOrder
      );
      const position = this.interpolateAlongPolyline(waypoints, data.progressPercentage);
      return {
        latitude: position.latitude,
        longitude: position.longitude,
        source: 'calculated'
      };
    }
  }
}
```

## Migration Path

### Phase 1 (Current)
- All buses use calculated mode
- Server always returns `positionType: "calculated"`

### Phase 2 (Future)
- Add GPS tracking infrastructure
- Server checks for real GPS data
- Returns `positionType: "real"` when available, falls back to calculated

### Phase 3 (Optional)
- Hybrid mode: Blend both sources
- Confidence scoring
- Historical data aggregation

## Benefits

✅ **Accurate visual representation** - Shows actual street path, not straight lines
✅ **Client-side computation** - No server load for position calculations
✅ **Smooth animations** - Can animate bus moving along polyline
✅ **Scalable** - Works for 1000+ concurrent users
✅ **Future-proof** - Easy migration to real GPS
✅ **Flexible** - Can adjust interpolation algorithm anytime
✅ **Fallback capability** - Can fall back to calculated if GPS unavailable

## API Endpoint Specification

### GET /api/routes/{routeId}/buses/{busId}/current-position

**Parameters:**
- `routeId` (int): Route ID
- `busId` (int): Bus ID

**Response (Calculated Mode):**
```json
{
  "busId": 5,
  "routeId": 1,
  "positionType": "calculated",
  "progressPercentage": 0.52,
  "currentStopOrder": 1,
  "nextStopOrder": 2,
  "currentStopId": 1,
  "nextStopId": 2,
  "timestamp": "2025-10-30T10:07:32Z"
}
```

**Response (Real GPS Mode):**
```json
{
  "busId": 5,
  "routeId": 1,
  "positionType": "real",
  "latitude": 43.2805,
  "longitude": 26.4970,
  "timestamp": "2025-10-30T10:07:32Z",
  "accuracy": 5.2
}
```

**Error Response:**
```json
{
  "error": "Bus not found or route not active",
  "statusCode": 404
}
```

## Notes

- All coordinates use WGS84 (EPSG:4326) for Leaflet.js compatibility
- Timestamps are in UTC
- Progress percentage is 0.0 to 1.0 (0% to 100%)
- Waypoints should be fetched once and cached on client
- Position updates can be polled every 5-10 seconds

