using Microsoft.EntityFrameworkCore;
using MoravianStar.Dao;
using ShumenTraffic.Common.Core.DTOs.Timetables;
using ShumenTraffic.Common.Core.Entities.BusStops;
using ShumenTraffic.Common.Core.Entities.Routes;
using ShumenTraffic.Common.Core.Entities.Schedules;
using ShumenTraffic.Common.Core.Enums.Routes;
using ShumenTraffic.Common.Core.Extensions;
using ShumenTraffic.Common.Core.Filters.Routes;
using ShumenTraffic.Common.Core.Filters.Schedules;
using ShumenTraffic.Common.Services.Interfaces.Timetables;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ShumenTraffic.Common.Services.Services.Timetables
{
    public class TimetableService : ITimetableService
    {
        public async Task<Timetable> Get(int busLineId, RouteDirection direction, DateOnly date)
        {
            Timetable result = null;

            var schedule = await GetExecutingSchedule(busLineId, direction, date);
            if (schedule == null)
            {
                return result;
            }

            var busStopsDict = schedule.ScheduleCourses.SelectMany(x => x.Route.RouteStops).Where(x => x.BusStopId.HasValue).DistinctBy(x => x.BusStopId).Select(x => x.BusStop).ToDictionary(x => x.Id, x => x);
            var routeVariants = FillRouteVariants(schedule);
            var mergedStops = BuildMergedStopOrder(routeVariants);

            result = new Timetable()
            {
                Schedule = schedule,
                TimetableRows = BuildMergedTimetable(mergedStops, routeVariants, busStopsDict)
            };

            return result;
        }

        private static async Task<Schedule> GetExecutingSchedule(int busLineId, RouteDirection direction, DateOnly date)
        {
            var filter = new ScheduleFilter()
            {
                BusLineId = busLineId,
                Direction = direction,
                IsActive = true,
                StartDateLE = date,
                EndDateGEOrNull = date,
                DaysOfWeekHasFlag = date.DayOfWeek.ToDaysOfWeek()
            };
            var sorts = new List<Sort>() { new Sort() { Field = "Priority", Dir = SortDirection.Desc } };
            Func<IQueryable<Schedule>, IQueryable<Schedule>> includes =
                (x) => x.Include(y => y.ScheduleCourses)
                            .ThenInclude(y => y.Route)
                                .ThenInclude(y => y.RouteStops)
                                    .ThenInclude(y => y.BusStop);
            var schedule = await Persistence.ForEntity<Schedule>().ReadQuery(filter, sorts, includes: includes, trackable: false).FirstOrDefaultAsync();
            return schedule;
        }

        private static List<RouteVariant> FillRouteVariants(Schedule schedule)
        {
            var result = new List<RouteVariant>();

            foreach (var scheduleCourse in schedule.ScheduleCourses.OrderBy(x => x.DepartureTime).ToList())
            {
                var stopTimes = new List<StopTime>();
                foreach (var stop in scheduleCourse.Route.RouteStops.OrderBy(x => x.StopOrder).ToList())
                {
                    if (stop.BusStopId.HasValue)
                    {
                        stopTimes.Add(new StopTime()
                        {
                            StopId = stop.BusStopId.Value,
                            Time = stop.EstimatedMinutesFromStart.HasValue ? scheduleCourse.DepartureTime.AddMinutes(stop.EstimatedMinutesFromStart.Value) : null
                        });
                    }
                }
                result.Add(new RouteVariant() { ScheduleCourseId = scheduleCourse.Id, Stops = stopTimes });
            }

            return result;
        }

        private static List<int> BuildMergedStopOrder(IEnumerable<RouteVariant> variants)
        {
            var graph = new Dictionary<int, HashSet<int>>();
            var indegree = new Dictionary<int, int>();
            var appearanceOrder = new Dictionary<int, int>();
            int appearanceIndex = 0;

            void Ensure(int stop)
            {
                if (!graph.ContainsKey(stop))
                {
                    graph[stop] = new HashSet<int>();
                }
                if (!indegree.ContainsKey(stop))
                {
                    indegree[stop] = 0;
                }
                if (!appearanceOrder.ContainsKey(stop))
                {
                    appearanceOrder[stop] = appearanceIndex++;
                }
            }

            foreach (var v in variants)
            {
                var stops = v.Stops.Select(s => s.StopId).ToList();

                foreach (var s in stops)
                {
                    Ensure(s);
                }

                for (int i = 0; i < stops.Count - 1; i++)
                {
                    if (graph[stops[i]].Add(stops[i + 1]))
                    {
                        indegree[stops[i + 1]]++;
                    }
                }
            }

            var queue = new PriorityQueue<int, int>();

            foreach (var kv in indegree)
            {
                if (kv.Value == 0)
                {
                    queue.Enqueue(kv.Key, appearanceOrder[kv.Key]);
                }
            }

            var result = new List<int>();

            while (queue.Count > 0)
            {
                var node = queue.Dequeue();
                result.Add(node);

                foreach (var next in graph[node])
                {
                    indegree[next]--;
                    if (indegree[next] == 0)
                    {
                        queue.Enqueue(next, appearanceOrder[next]);
                    }
                }
            }

            if (result.Count != indegree.Count)
            {
                throw new InvalidOperationException("Cannot merge routes: conflicting stop order detected.");
            }

            return result;
        }


        private static List<TimetableRow> BuildMergedTimetable(List<int> mergedStops, IEnumerable<RouteVariant> variants, Dictionary<int, BusStop> busStopsDict)
        {
            var variantLookup = variants.ToDictionary(
                v => v.ScheduleCourseId,
                v => v.Stops.ToDictionary(s => s.StopId, s => s.Time)
            );

            var result = new List<TimetableRow>();

            foreach (var stopId in mergedStops)
            {
                var row = new TimetableRow { BusStop = busStopsDict[stopId] };

                foreach (var variant in variantLookup)
                {
                    row.TimesByVariant[variant.Key] = variant.Value.TryGetValue(stopId, out var time) ? time : null;
                }

                result.Add(row);
            }

            return result;
        }
    }

    public class RouteVariant
    {
        public int ScheduleCourseId { get; set; }
        public List<StopTime> Stops { get; set; } = new List<StopTime>();
    }

    public class StopTime
    {
        public int StopId { get; set; }
        public TimeOnly? Time { get; set; }
    }
}