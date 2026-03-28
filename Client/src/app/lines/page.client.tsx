'use client';

import RouteBusStopConnector from '@/components/bus-lines/RouteBusStopConnector';
import RouteBusStopIndicator from '@/components/bus-lines/RouteBusStopIndicator';
import DirectionSelector from '@/components/DirectionSelector';
import { EntityDropdownLoader } from '@/components/EntityDropdown';
import MapLoader from '@/components/maps/MapLoader';
import StringUtility from '@/helpers/StringUtility';
import BusLinesLightService from '@/services/BusLinesLightService';
import RouteService from '@/services/RouteService';
import TimetablesService from '@/services/TimetablesService';
import BusLineLightModel from '@/types/BusLineLightModel';
import PageResult from '@/types/common/PageResult';
import RouteModel from '@/types/RouteModel';
import TimetableModel from '@/types/TimetableModel';
import { DateTime } from 'luxon';
import dynamic from 'next/dynamic';
import { usePathname, useRouter } from 'next/navigation';
import { use, useCallback, useEffect, useMemo, useState } from 'react';

// Dynamically import EntityDropdown to avoid SSR issues
const EntityDropdown = dynamic(() => import("@/components/EntityDropdown"), {
  ssr: false,
  loading: () => <EntityDropdownLoader />
});

// Dynamically import Map to avoid SSR issues
const RoutesMap = dynamic(() => import('@/components/maps/RoutesMap').then(mod => ({ default: mod.default })), {
  ssr: false,
  loading: () => <MapLoader />
});

export default function LinesPage({ searchParams }: { searchParams: Promise<{ [key: string]: string | string[] | undefined }> }) {
  const resolvedSearchParams = use(searchParams);
  const lineNumberUriComponent = resolvedSearchParams.lineNumber;
  const lineNumber = useMemo(() =>
    Array.isArray(lineNumberUriComponent) || StringUtility.isNullOrWhiteSpace(lineNumberUriComponent) ? undefined : decodeURIComponent(lineNumberUriComponent!),
    [lineNumberUriComponent]);
  const router = useRouter();
  const pathname = usePathname();

  const [userChangedBusLine, setUserChangedBusLine] = useState(false);
  const [selectedLineId, setSelectedLineId] = useState<number>(0);
  const [selectedDirection, setSelectedDirection] = useState<number>(1);
  const [selectedDate, setSelectedDate] = useState<string>(new Date().toISOString().split('T')[0]);
  const [timetable, setTimetable] = useState<TimetableModel | null>(null);
  const [routes, setRoutes] = useState<RouteModel[]>([]);
  const [isLoading, setIsLoading] = useState(true);
  const [currentTime, setCurrentTime] = useState<DateTime>(DateTime.now());

  // Fetch bus line by line number
  const fetchBusLine = useCallback(async () => {
    if (StringUtility.isNullOrWhiteSpace(lineNumber)) {
      setSelectedLineId(0);
      return;
    }

    if (userChangedBusLine) {
      setUserChangedBusLine(false);
      return;
    }

    try {
      setIsLoading(true);
      const data = await BusLinesLightService.read({ lineNumberEquals: lineNumber });
      const line = data.items[0];
      if (line) {
        setSelectedLineId(line.id);
      }
      else {
        setQueryString();
      }
    } catch (error) {
      console.error('Failed to fetch bus line:', error);
    } finally {
      setIsLoading(false);
    }
  }, [lineNumber]);

  useEffect(() => {
    fetchBusLine();
  }, [fetchBusLine]);


  const setQueryString = useCallback((lineNumber?: string) => {
    const params = new URLSearchParams();
    if (lineNumber) {
      params.set('lineNumber', lineNumber);
    }
    const query = params.toString();
    router.push(query ? `${pathname}?${query}` : pathname);
  }, []);

  // Update current time every second
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
    <div className="h-full bg-background flex flex-col">
      {/* Main Content - Two Column Layout */}
      <section className="flex-1 overflow-y-auto lg:overflow-hidden flex flex-col mx-auto w-full">
        <div className="flex flex-col lg:flex-row lg:flex-1 lg:min-h-0">
          {/* Left Pane - Controls and Stops List */}
          <div className="w-full lg:w-100 lg:shrink-0 bg-background flex flex-col lg:overflow-hidden border-b lg:border-b-0 lg:border-r border-border">
            {/* Controls */}
            <div className="p-6 border-b border-border">
              <div className="flex gap-2">
                {/* Line Selector */}
                <div className="flex-1">
                  <label className="label-standard">
                    Bus Line
                  </label>
                  <EntityDropdown
                    value={selectedLineId}
                    onChange={(e) => {
                      const newLineId = e ? Number(e.value) : 0;
                      setSelectedLineId(newLineId);
                      setUserChangedBusLine(true);
                      setQueryString(e?.data.lineNumber);
                    }}

                    placeholder="Select..."
                    url="/api/bus-lines-light"
                    sorts={[{ field: "LineNumber", dir: "asc" }]}
                    parseData={(data: PageResult<BusLineLightModel>) =>
                      data.items.map((item) => {
                        return {
                          value: item.id,
                          label: item.lineNumber,
                          data: item
                        };
                      })
                    }
                  />
                </div>

                {/* Direction Selector */}
                <div className="shrink-0">
                  <DirectionSelector
                    selectedDirection={selectedDirection}
                    onDirectionChange={setSelectedDirection}
                  />
                </div>
              </div>
            </div>

            {/* Stops List */}
            <div className="flex-1 overflow-y-auto p-6">
              {showEmptySelection && (
                <div className="text-center py-12">
                  <p className="text-text-muted">Please select a bus line and direction to view the route and schedule.</p>
                </div>
              )}

              {showNoTimetable && (
                <div className="text-center py-12">
                  <p className="text-text-muted">No timetable found.</p>
                </div>
              )}

              {showNoStops && (
                <div className="text-center py-12">
                  <p className="text-text-muted">No stops found for this route.</p>
                </div>
              )}

              {timetable && routes.length > 0 && (
                timetable.timetableRows.map((row, idx) => {
                  const nextDeparture = getNextDepartureTime(row.timesByVariant);
                  const isLastStop = idx === timetable.timetableRows.length - 1;

                  return (
                    <div key={row.busStop.id}>
                      {/* Stop marker and info */}
                      <div className="flex items-center gap-1">
                        <RouteBusStopIndicator
                          routes={routes}
                          busStopId={row.busStop.id}
                        />
                        <div className="flex-1 min-w-0 flex items-center justify-between gap-1">
                          <p className="text-sm text-foreground truncate" title={row.busStop.name || 'Unknown'}>
                            {row.busStop.name || 'Unknown'}
                          </p>
                          <p className="text-sm text-foreground shrink-0 w-8">
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
          </div>

          {/* Right Pane - Map */}
          <div className="w-full h-100 lg:h-auto lg:flex-1 lg:min-h-0 bg-background-secondary overflow-hidden">
            <RoutesMap routes={routes} />
          </div>
        </div>
      </section>
    </div>
  );
}