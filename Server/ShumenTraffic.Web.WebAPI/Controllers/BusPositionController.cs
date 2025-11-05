using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using ShumenTraffic.Common.Core.Entities;
using ShumenTraffic.Persistence.DbContexts;
using ShumenTraffic.Web.Core.Models;
using System;
using System.Linq;
using System.Threading.Tasks;

namespace ShumenTraffic.Web.WebAPI.Controllers
{
    /// <summary>
    /// Controller for managing live bus positions.
    /// Supports both calculated positions (based on schedule) and real GPS positions.
    /// </summary>
    [Route("api/routes/{routeId}/buses")]
    [ApiController]
    [Authorize]
    public class BusPositionController : BaseController
    {
        private readonly AppDbContext _context;

        public BusPositionController(AppDbContext context)
        {
            _context = context;
        }

        /// <summary>
        /// Get current position of a bus on a route.
        /// Supports two modes:
        /// - "calculated": Position is calculated based on schedule and route geometry
        /// - "gps": Position is from real GPS data (requires GPS data to be stored)
        /// </summary>
        /// <param name="routeId">Route ID</param>
        /// <param name="busId">Bus ID (unique identifier for the bus)</param>
        /// <param name="mode">Position calculation mode: "calculated" or "gps" (default: "calculated")</param>
        /// <param name="currentTime">Current time for calculated mode (optional, defaults to current UTC time)</param>
        /// <returns>Current bus position</returns>
        [HttpGet("{busId}/current-position")]
        [AllowAnonymous]
        public async Task<IActionResult> GetCurrentPosition(int routeId, string busId, [FromQuery] string mode = "calculated", [FromQuery] TimeSpan? currentTime = null)
        {
            // Verify route exists
            var route = await _context.Routes
                .Include(r => r.BusLine)
                .Include(r => r.RouteStops)
                .ThenInclude(rs => rs.BusStop)
                .FirstOrDefaultAsync(r => r.Id == routeId);

            if (route == null)
            {
                return NotFound("Route not found", $"No route found with ID {routeId}");
            }

            // Validate mode
            if (mode != "calculated" && mode != "gps")
            {
                return BadRequest("Invalid mode", "Mode must be 'calculated' or 'gps'");
            }

            // For now, we'll implement the calculated mode
            // GPS mode would require storing real GPS data in the database
            if (mode == "gps")
            {
                return BadRequest("GPS mode not yet implemented", "Please use 'calculated' mode");
            }

            // Calculate position based on schedule
            var position = await CalculatePositionFromSchedule(route, busId, currentTime ?? TimeSpan.FromSeconds(DateTime.UtcNow.TimeOfDay.TotalSeconds));

            if (position == null)
            {
                return NotFound("Bus not found", $"No active course found for bus {busId} on route {routeId} at the current time");
            }

            return Ok(position, "Bus position retrieved successfully");
        }

        /// <summary>
        /// Calculate bus position based on schedule and route geometry.
        /// </summary>
        private async Task<BusPositionModel> CalculatePositionFromSchedule(Route route, string busId, TimeSpan currentTime)
        {
            // Get all schedules for today (we'll use all day types for now)
            var today = DateTimeOffset.UtcNow.Date;
            var schedules = await _context.Schedules
                .Include(s => s.ScheduleCourses)
                .Where(s => s.IsActive && s.EffectiveDate.Date <= today && (s.ExpiryDate == null || s.ExpiryDate.Value.Date >= today))
                .ToListAsync();

            // Find a course for this route that matches the current time
            var matchingCourse = schedules
                .SelectMany(s => s.ScheduleCourses)
                .Where(sc => sc.RouteId == route.Id)
                .FirstOrDefault(sc =>
                {
                    var departureTime = sc.DepartureTime.ToTimeSpan();
                    // Get the last stop's estimated time
                    var lastStop = route.RouteStops
                        .Where(rs => rs.BusStopId.HasValue)
                        .OrderByDescending(rs => rs.EstimatedMinutesFromStart)
                        .FirstOrDefault();

                    if (lastStop == null)
                        return false;

                    var endTime = departureTime.Add(TimeSpan.FromMinutes(lastStop.EstimatedMinutesFromStart.Value));
                    return currentTime >= departureTime && currentTime <= endTime;
                });

            if (matchingCourse == null)
            {
                return null;
            }

            // Calculate progress along the route
            var departureTime = matchingCourse.DepartureTime.ToTimeSpan();
            var elapsedTime = currentTime - departureTime;
            var elapsedMinutes = (int)elapsedTime.TotalMinutes;

            // Find current and next stops
            var stops = route.RouteStops
                .Where(rs => rs.BusStopId.HasValue)
                .OrderBy(rs => rs.EstimatedMinutesFromStart)
                .ToList();

            var currentStopIndex = -1;
            var nextStopIndex = -1;

            for (int i = 0; i < stops.Count; i++)
            {
                if (stops[i].EstimatedMinutesFromStart <= elapsedMinutes)
                {
                    currentStopIndex = i;
                }
                else if (nextStopIndex == -1)
                {
                    nextStopIndex = i;
                    break;
                }
            }

            // If no next stop, bus has reached the end
            if (nextStopIndex == -1)
            {
                nextStopIndex = stops.Count - 1;
            }

            // Calculate interpolated position between current and next stop
            var currentStop = stops[currentStopIndex >= 0 ? currentStopIndex : 0];
            var nextStop = stops[nextStopIndex];

            decimal latitude = currentStop.Latitude;
            decimal longitude = currentStop.Longitude;
            decimal progressPercentage = 0;
            int estimatedMinutesToNext = 0;

            if (currentStopIndex >= 0 && nextStopIndex > currentStopIndex)
            {
                var currentStopTime = currentStop.EstimatedMinutesFromStart.Value;
                var nextStopTime = nextStop.EstimatedMinutesFromStart.Value;
                var timeBetweenStops = nextStopTime - currentStopTime;

                if (timeBetweenStops > 0)
                {
                    var timeFromCurrentStop = elapsedMinutes - currentStopTime;
                    progressPercentage = (decimal)timeFromCurrentStop / timeBetweenStops * 100;

                    // Linear interpolation between stops
                    var progress = (decimal)timeFromCurrentStop / timeBetweenStops;
                    latitude = currentStop.Latitude + (nextStop.Latitude - currentStop.Latitude) * progress;
                    longitude = currentStop.Longitude + (nextStop.Longitude - currentStop.Longitude) * progress;

                    estimatedMinutesToNext = Math.Max(0, nextStopTime - elapsedMinutes);
                }
            }

            return new BusPositionModel
            {
                BusId = busId,
                RouteId = route.Id,
                BusLineNumber = route.BusLine.LineNumber,
                Direction = route.Direction,
                Latitude = latitude,
                Longitude = longitude,
                Timestamp = DateTimeOffset.UtcNow,
                Mode = "calculated",
                CurrentStopIndex = currentStopIndex >= 0 ? currentStopIndex : 0,
                NextStopIndex = nextStopIndex,
                ProgressPercentage = Math.Min(100, Math.Max(0, progressPercentage)),
                EstimatedMinutesToNextStop = estimatedMinutesToNext
            };
        }
    }
}