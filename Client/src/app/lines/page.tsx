'use client';

import dynamic from 'next/dynamic';
import { useState, useEffect } from 'react';

interface BusLine {
  id: number;
  lineNumber: string;
}

interface Route {
  id: number;
  busLineId: number;
  direction: number;
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
  latitude: number;
  longitude: number;
}

// Dynamically import Map to avoid SSR issues
const Map = dynamic(() => import('@/components/Map').then(mod => ({ default: mod.Map })), {
  ssr: false,
  loading: () => <div className="w-full h-full bg-gray-100 dark:bg-slate-800 flex items-center justify-center">Loading map...</div>,
});

export default function LinesPage() {
  const [busLines, setBusLines] = useState<BusLine[]>([]);
  const [selectedLineId, setSelectedLineId] = useState<number | null>(null);
  const [selectedDirection, setSelectedDirection] = useState<number>(0);
  const [routes, setRoutes] = useState<Route[]>([]);
  const [routeStops, setRouteStops] = useState<RouteStop[]>([]);
  const [busStops, setBusStops] = useState<BusStop[]>([]);
  const [isLoading, setIsLoading] = useState(true);

  // Fetch bus lines on mount
  useEffect(() => {
    const fetchBusLines = async () => {
      try {
        const response = await fetch('http://localhost:5000/api/bus-lines');
        if (response.ok) {
          const data = await response.json();
          setBusLines(data.data || []);
          if (data.data && data.data.length > 0) {
            setSelectedLineId(data.data[0].id);
          }
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
        const response = await fetch('http://localhost:5000/api/routes');
        if (response.ok) {
          const data = await response.json();
          const lineRoutes = (data.data || []).filter(
            (r: Route) => r.busLineId === selectedLineId
          );
          setRoutes(lineRoutes);
        }
      } catch (error) {
        console.error('Failed to fetch routes:', error);
      }
    };

    fetchRoutes();
  }, [selectedLineId]);

  // Fetch route stops when route is selected
  useEffect(() => {
    const selectedRoute = routes.find(r => r.direction === selectedDirection);
    if (!selectedRoute) return;

    const fetchRouteStops = async () => {
      try {
        const response = await fetch(`http://localhost:5000/api/routes/${selectedRoute.id}/stops`);
        if (response.ok) {
          const data = await response.json();
          setRouteStops(data.data || []);
        }
      } catch (error) {
        console.error('Failed to fetch route stops:', error);
      }
    };

    fetchRouteStops();
  }, [selectedDirection, routes]);

  // Fetch all bus stops for map
  useEffect(() => {
    const fetchBusStops = async () => {
      try {
        const response = await fetch('http://localhost:5000/api/bus-stops');
        if (response.ok) {
          const data = await response.json();
          setBusStops(data.data || []);
        }
      } catch (error) {
        console.error('Failed to fetch bus stops:', error);
      }
    };

    fetchBusStops();
  }, []);

  const selectedRoute = routes.find(r => r.direction === selectedDirection);
  const stopsForDisplay = routeStops
    .sort((a, b) => a.stopOrder - b.stopOrder)
    .map(rs => {
      const stop = busStops.find(s => s.id === rs.busStopId);
      return { ...rs, stop };
    });

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
              Select Bus Line
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
                  className={`flex-1 px-4 py-2 rounded-lg font-semibold transition-colors ${selectedDirection === dir
                      ? 'bg-blue-600 text-white'
                      : 'bg-gray-200 dark:bg-slate-800 text-gray-900 dark:text-white hover:bg-gray-300 dark:hover:bg-slate-700'
                    }`}
                >
                  Direction {dir + 1}
                </button>
              ))}
            </div>
          </div>
        </div>

        {/* Main Content */}
        <div className="grid grid-cols-1 lg:grid-cols-3 gap-6 h-[600px]">
          {/* Left Pane - Stops List */}
          <div className="lg:col-span-1 bg-gray-50 dark:bg-slate-900 rounded-lg border border-gray-200 dark:border-slate-700 p-6 overflow-y-auto">
            <h2 className="text-xl font-bold text-gray-900 dark:text-white mb-4">
              Stops
            </h2>
            {stopsForDisplay.length === 0 ? (
              <p className="text-gray-600 dark:text-gray-400">No stops found for this route.</p>
            ) : (
              <div className="space-y-2">
                {stopsForDisplay.map((rs, idx) => (
                  <div
                    key={rs.id}
                    className="p-3 bg-white dark:bg-slate-800 rounded border border-gray-200 dark:border-slate-700 hover:shadow-md transition-shadow"
                  >
                    <div className="flex items-start gap-3">
                      <div className="flex-shrink-0 w-8 h-8 bg-blue-600 rounded-full flex items-center justify-center">
                        <span className="text-white text-sm font-bold">{idx + 1}</span>
                      </div>
                      <div className="flex-1 min-w-0">
                        <p className="font-medium text-gray-900 dark:text-white truncate">
                          {rs.stop?.name || 'Unknown Stop'}
                        </p>
                        <p className="text-sm text-gray-600 dark:text-gray-400">
                          {rs.estimatedMinutesFromStart} min from start
                        </p>
                      </div>
                    </div>
                  </div>
                ))}
              </div>
            )}
          </div>

          {/* Right Pane - Map */}
          <div className="lg:col-span-2 bg-gray-100 dark:bg-slate-800 rounded-lg border border-gray-200 dark:border-slate-700 overflow-hidden">
            <Map busStops={busStops} />
          </div>
        </div>
      </div>
    </div>
  );
}

