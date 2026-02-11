'use client';

import EntityDropdown from '@/components/EntityDropdown';
import MapLoader from '@/components/maps/MapLoader';
import { ROUTE_COLORS } from '@/constants/RouteColors';
import RouteService from '@/services/RouteService';
import TimetablesService from '@/services/TimetablesService';
import BusLineLightModel from '@/types/BusLineLightModel';
import PageResult from '@/types/common/PageResult';
import RouteModel from '@/types/RouteModel';
import TimetableModel from '@/types/TimetableModel';
import { DateTime } from 'luxon';
import dynamic from 'next/dynamic';
import { useEffect, useState } from 'react';

// Dynamically import Map to avoid SSR issues
const RoutesMap = dynamic(() => import('@/components/maps/RoutesMap').then(mod => ({ default: mod.default })), {
  ssr: false,
  loading: () => <MapLoader />
});

export default function LinesPage() {
  const [selectedLineId, setSelectedLineId] = useState<number>(0);
  const [selectedDirection, setSelectedDirection] = useState<number>(1);
  const [selectedDate, setSelectedDate] = useState<string>(new Date().toISOString().split('T')[0]);
  const [timetable, setTimetable] = useState<TimetableModel | null>(null);
  const [routes, setRoutes] = useState<RouteModel[]>([]);
  const [isLoading, setIsLoading] = useState(true);
  const [currentTime, setCurrentTime] = useState<DateTime>(DateTime.now());

  useEffect(() => {
    const interval = setInterval(() => {
      setCurrentTime(DateTime.now());
    }, 1000);

    return () => {
      clearInterval(interval);
    };
  }, []);

  // Fetch timetable when a line and direction are selected
  useEffect(() => {
    if (!selectedLineId || !selectedDirection || !selectedDate) {
      setTimetable(null);
      return;
    }
    fetchTimetable();
  }, [selectedLineId, selectedDirection, selectedDate]);

  // Fetch routes when a timetable is fetched
  useEffect(() => {
    if (!timetable) {
      setRoutes([]);
      return;
    }
    fetchRoutes();
  }, [timetable]);

  const fetchTimetable = async () => {
    try {
      setIsLoading(true);
      const data = await TimetablesService.get(selectedLineId, selectedDirection, selectedDate);
      setTimetable(data);
    } catch (error) {
      console.error('Failed to fetch timetable:', error);
    }
    finally {
      setIsLoading(false);
    }
  };

  const fetchRoutes = async () => {
    try {
      setIsLoading(true);
      const data = await RouteService.read({ scheduleId: timetable!.schedule.id });
      setRoutes(data.items);
    } catch (error) {
      console.error('Failed to fetch routes:', error);
    }
    finally {
      setIsLoading(false);
    }
  };

  // Finds the next departure time for a bus stop
  const getNextDepartureTime = (timesByVariant: { [key: string]: string | null }): string | null => {
    if (!timesByVariant) {
      return null;
    }

    // Extract all departure times and filter out nulls
    const departureTimes = Object.values(timesByVariant)
      .filter((time): time is string => time !== null)
      .map(timeStr => {
        // Parse time string (format: "HH:mm:ss" or "HH:mm")
        const [hours, minutes] = timeStr.split(':').map(Number);
        return DateTime.now().set({ hour: hours, minute: minutes, second: 0, millisecond: 0 });
      })
      .sort((a, b) => a.toMillis() - b.toMillis()); // Sort by time

    if (departureTimes.length === 0) {
      return null;
    }

    // Find the next departure time after current time
    const nextDeparture = departureTimes.find(time => time >= currentTime);

    if (!nextDeparture) {
      // No more departures today
      return null;
    }

    // Return formatted time
    return nextDeparture.toFormat('HH:mm');
  };

  return (
    <div className="min-h-screen bg-white dark:bg-slate-950 flex flex-col">
      <div className="max-w-7xl mx-auto w-full px-4 sm:px-6 lg:px-8 py-6">
        <h1 className="text-4xl font-bold text-gray-900 dark:text-white mb-4">
          Bus Lines
        </h1>
        <p className="text-gray-600 dark:text-gray-400 text-lg mb-8">
          Select a bus line and direction to view the route and schedule.
        </p>

        {/* Controls */}
        <div className="grid grid-cols-1 md:grid-cols-3 gap-4 mb-8">
          {/* Line Selector */}
          <div>
            <label className="block text-sm font-medium text-gray-700 dark:text-gray-300 mb-2">
              Bus Line
            </label>
            <EntityDropdown
              value={selectedLineId}
              onChange={(e) => setSelectedLineId(e ? Number(e.value) : 0)}
              placeholder="Select..."
              url="/api/bus-lines-light"
              sorts={[{ field: "LineNumber", dir: "asc" }]}
              parseData={(data: PageResult<BusLineLightModel>) =>
                data.items.map((item) => {
                  return {
                    value: item.id,
                    label: item.lineNumber
                  };
                })
              }
            />
          </div>

          {/* Direction Buttons */}
          <div>
            <label className="block text-sm font-medium text-gray-700 dark:text-gray-300 mb-2">
              Direction
            </label>
            <div className="flex gap-2">
              {[1, 2].map(dir => (
                <button
                  key={dir}
                  onClick={() => setSelectedDirection(dir)}
                  className={`flex-1 px-3 py-2 rounded-lg font-semibold transition-colors text-sm ${selectedDirection === dir
                    ? 'bg-blue-600 text-white'
                    : 'bg-gray-200 dark:bg-slate-800 text-gray-900 dark:text-white hover:bg-gray-300 dark:hover:bg-slate-700'
                    }`}
                >
                  Dir {dir}
                </button>
              ))}
            </div>
          </div>
        </div>

        {/* Main Content */}
        <div className="grid grid-cols-1 lg:grid-cols-3 gap-6 h-[600px]">
          {/* Left Pane - Stops List */}
          <div className="lg:col-span-1 bg-gray-50 dark:bg-slate-900 rounded-lg border border-gray-200 dark:border-slate-700 p-6 overflow-y-auto">
            {!selectedLineId || !selectedDirection || !selectedDate ?
              <div className="text-center py-12">
                <p className="text-gray-600 dark:text-gray-400">Please select a bus line and direction to view the route and schedule.</p>
              </div>
              :
              !timetable && !isLoading && (
                <div className="text-center py-12">
                  <p className="text-gray-600 dark:text-gray-400">No timetable found.</p>
                </div>
              )}

            {timetable && (
              timetable?.timetableRows.length === 0 ? (
                <div className="text-center py-12">
                  <p className="text-gray-600 dark:text-gray-400">No stops found for this route.</p>
                </div>
              ) : (
                <div>
                  {timetable?.timetableRows.map((rs, idx) => {
                    const nextDeparture = getNextDepartureTime(rs.timesByVariant);
                    const isLastStop = idx === timetable.timetableRows.length - 1;
                    return (
                      <div
                        key={rs.busStop.id}
                        className="relative"
                      >
                        {/* Stop marker and info */}
                        <div className="flex items-start gap-2">
                          {routes.map((route, index) => {
                            const hasRouteStop = route.stops.some(x => x.busStopId === rs.busStop.id);
                            const routeColor = ROUTE_COLORS[index % ROUTE_COLORS.length];
                            if (hasRouteStop) {
                              return (
                                <div key={index} className="shrink-0 w-6 h-6 rounded-full flex items-center justify-center z-10 relative" style={{ backgroundColor: routeColor }}>
                                  <div className="w-3 h-3 rounded-full bg-white"></div>
                                </div>
                              )
                            }
                            else {
                              return (
                                <div key={index} className="w-6 h-6">
                                  <div className="border-l-4 h-full ml-2.5" style={{ borderColor: routeColor }}></div>
                                </div>
                              )
                            }
                          })}
                          <div className="flex-1 min-w-0 flex items-center justify-between gap-2">
                            <p className="font-medium text-gray-900 dark:text-white truncate" title={rs.busStop.name || 'Unknown'}>
                              {rs.busStop.name || 'Unknown'}
                            </p>
                            <p className="font-medium text-gray-900 dark:text-white shrink-0 w-10">
                              {nextDeparture}
                            </p>
                          </div>
                        </div>

                        {/* Connecting line to next stop */}
                        {!isLastStop && (
                          <div className="flex items-start gap-2 h-6">
                            {routes.map((route, index) => {
                              const routeColor = ROUTE_COLORS[index % ROUTE_COLORS.length];
                              return (
                                <div key={index} className="w-6 h-full flex justify-center">
                                  <div className="w-1 h-full" style={{ backgroundColor: routeColor }}></div>
                                </div>
                              )
                            })}
                          </div>
                        )}
                      </div>
                    );
                  })}
                </div>)
            )}
          </div>

          {/* Right Pane - Map */}
          <div className="lg:col-span-2 bg-gray-100 dark:bg-slate-800 rounded-lg border border-gray-200 dark:border-slate-700 overflow-hidden">
            <RoutesMap routes={routes} />
          </div>
        </div>
      </div>
    </div>
  );
}