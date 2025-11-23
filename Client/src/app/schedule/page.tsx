'use client';

import api from '@/lib/api';
import RouteService from '@/services/RouteService';
import PageResult from '@/types/common/PageResult';
import { useEffect, useState } from 'react';

interface BusLine {
  id: number;
  lineNumber: string;
}

interface Route {
  id: number;
  busLineId: number;
  direction: number;
}

interface ScheduleCourse {
  id: number;
  routeId: number;
  departureTime: string;
}

interface RouteStop {
  id: number;
  routeId: number;
  busStopId?: number;
  stopOrder: number;
  estimatedMinutesFromStart: number;
}

interface BusStop {
  id: number;
  name: string;
}

export default function SchedulePage() {
  const [busLines, setBusLines] = useState<BusLine[]>([]);
  const [selectedLineId, setSelectedLineId] = useState<number | null>(null);
  const [selectedDirection, setSelectedDirection] = useState<number>(0);
  const [selectedDate, setSelectedDate] = useState<string>(
    new Date().toISOString().split('T')[0]
  );
  const [routes, setRoutes] = useState<Route[]>([]);
  const [courses, setCourses] = useState<ScheduleCourse[]>([]);
  const [routeStops, setRouteStops] = useState<RouteStop[]>([]);
  const [busStops, setBusStops] = useState<BusStop[]>([]);
  const [highlightedStop, setHighlightedStop] = useState<number | null>(null);
  const [highlightedCourse, setHighlightedCourse] = useState<number | null>(null);
  const [isLoading, setIsLoading] = useState(true);

  // Fetch bus lines on mount
  useEffect(() => {
    const fetchBusLines = async () => {
      try {
        const data = await api.get<PageResult<BusLine>>('/bus-lines');
        setBusLines(data.items);
        if (data.items.length > 0) {
          setSelectedLineId(data.items[0].id);
        }
      } catch (error) {
        console.error('Failed to fetch bus lines:', error);
      } finally {
        setIsLoading(false);
      }
    };

    fetchBusLines();
  }, []);

  // Fetch routes when line is selected
  useEffect(() => {
    if (!selectedLineId) return;

    const fetchRoutes = async () => {
      try {
        const data = await RouteService.read();
        const lineRoutes = data.items.filter((r: Route) => r.busLineId === selectedLineId);
        setRoutes(lineRoutes);
      } catch (error) {
        console.error('Failed to fetch routes:', error);
      }
    };

    fetchRoutes();
  }, [selectedLineId]);

  // Fetch courses when route is selected
  useEffect(() => {
    const selectedRoute = routes.find(r => r.direction === selectedDirection);
    if (!selectedRoute) return;

    const fetchCourses = async () => {
      try {
        const data = await api.get<ScheduleCourse[]>(`/schedules?routeId=${selectedRoute.id}`);
        setCourses(data);
      } catch (error) {
        console.error('Failed to fetch courses:', error);
      }
    };

    fetchCourses();
  }, [selectedDirection, routes]);

  // Fetch route stops when route is selected
  useEffect(() => {
    const selectedRoute = routes.find(r => r.direction === selectedDirection);
    if (!selectedRoute) return;

    const fetchRouteStops = async () => {
      try {
        const data = await api.get<RouteStop[]>(`/routes/${selectedRoute.id}/stops`);
        setRouteStops(data);
      } catch (error) {
        console.error('Failed to fetch route stops:', error);
      }
    };

    fetchRouteStops();
  }, [selectedDirection, routes]);

  // Fetch all bus stops
  useEffect(() => {
    const fetchBusStops = async () => {
      try {
        const data = await api.get<BusStop[]>('/bus-stops');
        setBusStops(data);
      } catch (error) {
        console.error('Failed to fetch bus stops:', error);
      }
    };

    fetchBusStops();
  }, []);

  const stopsForDisplay = routeStops
    .sort((a, b) => a.stopOrder - b.stopOrder)
    .map(rs => {
      const stop = busStops.find(s => s.id === rs.busStopId);
      return { ...rs, stop };
    });

  const formatTime = (timeStr: string) => {
    try {
      const date = new Date(timeStr);
      return date.toLocaleTimeString('en-US', { hour: '2-digit', minute: '2-digit', hour12: false });
    } catch {
      return timeStr;
    }
  };

  const calculateStopTime = (courseTime: string, minutesFromStart: number) => {
    try {
      const date = new Date(courseTime);
      date.setMinutes(date.getMinutes() + minutesFromStart);
      return date.toLocaleTimeString('en-US', { hour: '2-digit', minute: '2-digit', hour12: false });
    } catch {
      return '--:--';
    }
  };

  return (
    <div className="min-h-screen bg-white dark:bg-slate-950 flex flex-col">
      <div className="max-w-7xl mx-auto w-full px-4 sm:px-6 lg:px-8 py-6">
        <h1 className="text-4xl font-bold text-gray-900 dark:text-white mb-4">
          Bus Schedule
        </h1>
        <p className="text-gray-600 dark:text-gray-400 text-lg mb-8">
          View detailed schedules for all bus lines.
        </p>

        {/* Controls */}
        <div className="grid grid-cols-1 md:grid-cols-4 gap-4 mb-8">
          {/* Line Selector */}
          <div>
            <label className="block text-sm font-medium text-gray-700 dark:text-gray-300 mb-2">
              Bus Line
            </label>
            <select
              value={selectedLineId || ''}
              onChange={(e) => setSelectedLineId(parseInt(e.target.value))}
              className="w-full px-4 py-2 border border-gray-300 dark:border-slate-600 rounded-lg bg-white dark:bg-slate-800 text-gray-900 dark:text-white focus:outline-none focus:ring-2 focus:ring-blue-600"
            >
              {busLines.map(line => (
                <option key={line.id} value={line.id}>
                  Line {line.lineNumber}
                </option>
              ))}
            </select>
          </div>

          {/* Direction Buttons */}
          <div>
            <label className="block text-sm font-medium text-gray-700 dark:text-gray-300 mb-2">
              Direction
            </label>
            <div className="flex gap-2">
              {[0, 1].map(dir => (
                <button
                  key={dir}
                  onClick={() => setSelectedDirection(dir)}
                  className={`flex-1 px-3 py-2 rounded-lg font-semibold transition-colors text-sm ${selectedDirection === dir
                    ? 'bg-blue-600 text-white'
                    : 'bg-gray-200 dark:bg-slate-800 text-gray-900 dark:text-white hover:bg-gray-300 dark:hover:bg-slate-700'
                    }`}
                >
                  Dir {dir + 1}
                </button>
              ))}
            </div>
          </div>

          {/* Date Picker */}
          <div>
            <label className="block text-sm font-medium text-gray-700 dark:text-gray-300 mb-2">
              Date
            </label>
            <input
              type="date"
              value={selectedDate}
              onChange={(e) => setSelectedDate(e.target.value)}
              className="w-full px-4 py-2 border border-gray-300 dark:border-slate-600 rounded-lg bg-white dark:bg-slate-800 text-gray-900 dark:text-white focus:outline-none focus:ring-2 focus:ring-blue-600"
            />
          </div>
        </div>

        {/* Schedule Table */}
        <div className="bg-gray-50 dark:bg-slate-900 rounded-lg border border-gray-200 dark:border-slate-700 overflow-x-auto">
          <table className="w-full text-sm">
            <thead>
              <tr className="border-b border-gray-300 dark:border-slate-600 bg-gray-100 dark:bg-slate-800">
                <th className="text-left py-3 px-4 font-semibold text-gray-900 dark:text-white sticky left-0 bg-gray-100 dark:bg-slate-800 z-10">
                  Stop
                </th>
                {courses.map(course => (
                  <th
                    key={course.id}
                    className={`text-center py-3 px-2 font-semibold text-gray-900 dark:text-white whitespace-nowrap cursor-pointer transition-colors ${highlightedCourse === course.id
                      ? 'bg-blue-100 dark:bg-blue-900'
                      : 'hover:bg-gray-200 dark:hover:bg-slate-700'
                      }`}
                    onClick={() => setHighlightedCourse(highlightedCourse === course.id ? null : course.id)}
                  >
                    {formatTime(course.departureTime)}
                  </th>
                ))}
              </tr>
            </thead>
            <tbody>
              {stopsForDisplay.length === 0 ? (
                <tr>
                  <td colSpan={courses.length + 1} className="py-8 px-4 text-center text-gray-600 dark:text-gray-400">
                    No stops found for this route.
                  </td>
                </tr>
              ) : (
                stopsForDisplay.map((rs, idx) => (
                  <tr
                    key={rs.id}
                    className={`border-b border-gray-200 dark:border-slate-700 transition-colors ${highlightedStop === rs.id
                      ? 'bg-blue-50 dark:bg-blue-900/20'
                      : 'hover:bg-gray-100 dark:hover:bg-slate-800'
                      }`}
                  >
                    <td
                      className={`py-3 px-4 font-medium text-gray-900 dark:text-white sticky left-0 z-10 cursor-pointer transition-colors ${highlightedStop === rs.id
                        ? 'bg-blue-50 dark:bg-blue-900/20'
                        : 'bg-gray-50 dark:bg-slate-900 hover:bg-gray-100 dark:hover:bg-slate-800'
                        }`}
                      onClick={() => setHighlightedStop(highlightedStop === rs.id ? null : rs.id)}
                    >
                      <div className="flex items-center gap-2">
                        <span className="shrink-0 w-6 h-6 bg-blue-600 rounded-full flex items-center justify-center text-white text-xs font-bold">
                          {idx + 1}
                        </span>
                        <span className="truncate">{rs.stop?.name || 'Unknown'}</span>
                      </div>
                    </td>
                    {courses.map(course => (
                      <td
                        key={`${rs.id}-${course.id}`}
                        className={`text-center py-3 px-2 text-gray-900 dark:text-white transition-colors ${highlightedCourse === course.id
                          ? 'bg-blue-100 dark:bg-blue-900'
                          : ''
                          }`}
                      >
                        {calculateStopTime(course.departureTime, rs.estimatedMinutesFromStart)}
                      </td>
                    ))}
                  </tr>
                ))
              )}
            </tbody>
          </table>
        </div>

        {/* Legend */}
        <div className="mt-6 p-4 bg-blue-50 dark:bg-blue-900/20 border border-blue-200 dark:border-blue-700 rounded-lg">
          <p className="text-sm text-gray-700 dark:text-gray-300">
            <strong>Tip:</strong> Click on a stop name to highlight the row, or click on a departure time to highlight the column.
          </p>
        </div>
      </div>
    </div>
  );
}

