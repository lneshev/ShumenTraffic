'use client';

import RouteBusStopConnector from '@/components/bus-lines/RouteBusStopConnector';
import RouteBusStopIndicator from '@/components/bus-lines/RouteBusStopIndicator';
import DirectionSelector from '@/components/DirectionSelector';
import EntityDropdown from '@/components/EntityDropdown';
import MapLoader from '@/components/maps/MapLoader';
import RouteService from '@/services/RouteService';
import TimetablesService from '@/services/TimetablesService';
import BusLineLightModel from '@/types/BusLineLightModel';
import PageResult from '@/types/common/PageResult';
import RouteModel from '@/types/RouteModel';
import TimetableModel from '@/types/TimetableModel';
import { DateTime } from 'luxon';
import dynamic from 'next/dynamic';
import { useCallback, useEffect, useMemo, useState } from 'react';

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
  const fetchTimetable = useCallback(async () => {
    if (!selectedLineId || !selectedDirection || !selectedDate) {
      setTimetable(null);
      return;
    }

    try {
      setIsLoading(true);
      const data = await TimetablesService.get(selectedLineId, selectedDirection, selectedDate);
      setTimetable(data);
    } catch (error) {
      console.error('Failed to fetch timetable:', error);
      setTimetable(null);
    } finally {
      setIsLoading(false);
    }
  }, [selectedLineId, selectedDirection, selectedDate]);

  // Fetch routes when a timetable is fetched
  const fetchRoutes = useCallback(async () => {
    if (!timetable) {
      setRoutes([]);
      return;
    }

    try {
      setIsLoading(true);
      const data = await RouteService.read({ scheduleId: timetable.schedule.id });
      setRoutes(data.items);
    } catch (error) {
      console.error('Failed to fetch routes:', error);
      setRoutes([]);
    } finally {
      setIsLoading(false);
    }
  }, [timetable]);

  useEffect(() => {
    fetchTimetable();
  }, [fetchTimetable]);

  useEffect(() => {
    fetchRoutes();
  }, [fetchRoutes]);

  // Finds the next departure time for a bus stop
  const getNextDepartureTime = useCallback((timesByVariant: { [key: string]: string | null }): string | null => {
    if (!timesByVariant) {
      return null;
    }

    // Extract and parse all valid departure times
    const parsedTimes = Object.values(timesByVariant)
      .filter((time): time is string => time !== null)
      .map(timeStr => {
        const [hours, minutes] = timeStr.split(':').map(Number);
        return DateTime.now().set({ hour: hours, minute: minutes, second: 0, millisecond: 0 });
      })
      .sort((a, b) => a.toMillis() - b.toMillis());

    if (parsedTimes.length === 0) {
      return null;
    }

    // Find the next departure after current time
    const nextDeparture = parsedTimes.find(time => time >= currentTime);

    return nextDeparture ? nextDeparture.toFormat('HH:mm') : null;
  }, [currentTime]);

  // Determine if selection is complete
  const hasSelection = useMemo(() =>
    selectedLineId > 0 && selectedDirection > 0 && selectedDate != null,
    [selectedLineId, selectedDirection, selectedDate]
  );

  // Determine content state
  const showEmptySelection = !hasSelection;
  const showNoTimetable = hasSelection && !timetable && !isLoading;
  const showNoStops = timetable && timetable.timetableRows.length === 0;

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

          {/* Direction Selector */}
          <DirectionSelector
            selectedDirection={selectedDirection}
            onDirectionChange={setSelectedDirection}
          />
        </div>

        {/* Main Content */}
        <div className="grid grid-cols-1 lg:grid-cols-3 gap-6 h-[600px]">
          {/* Left Pane - Stops List */}
          <div className="lg:col-span-1 bg-gray-50 dark:bg-slate-900 rounded-lg border border-gray-200 dark:border-slate-700 p-6 overflow-y-auto">
            {showEmptySelection && (
              <div className="text-center py-12">
                <p className="text-gray-600 dark:text-gray-400">Please select a bus line and direction to view the route and schedule.</p>
              </div>
            )}

            {showNoTimetable && (
              <div className="text-center py-12">
                <p className="text-gray-600 dark:text-gray-400">No timetable found.</p>
              </div>
            )}

            {showNoStops && (
              <div className="text-center py-12">
                <p className="text-gray-600 dark:text-gray-400">No stops found for this route.</p>
              </div>
            )}

            {timetable && routes.length > 0 && (
              timetable.timetableRows.map((row, idx) => {
                const nextDeparture = getNextDepartureTime(row.timesByVariant);
                const isLastStop = idx === timetable.timetableRows.length - 1;

                return (
                  <div key={row.busStop.id}>
                    {/* Stop marker and info */}
                    <div className="flex items-start gap-2">
                      <RouteBusStopIndicator
                        routes={routes}
                        busStopId={row.busStop.id}
                      />
                      <div className="flex-1 min-w-0 flex items-center justify-between gap-2">
                        <p className="font-medium text-gray-900 dark:text-white truncate" title={row.busStop.name || 'Unknown'}>
                          {row.busStop.name || 'Unknown'}
                        </p>
                        <p className="font-medium text-gray-900 dark:text-white shrink-0 w-10">
                          {nextDeparture}
                        </p>
                      </div>
                    </div>

                    {/* Connecting line to next stop */}
                    {!isLastStop && <RouteBusStopConnector routes={routes} />}
                  </div>
                );
              })
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