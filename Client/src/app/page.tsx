'use client';

import MapLoader from '@/components/maps/MapLoader';
import BusStopService from "@/services/BusStopService";
import BusStopModel from "@/types/BusStopModel";
import PageResult from '@/types/common/PageResult';
import dynamic from 'next/dynamic';
import { useEffect, useState } from 'react';

// Dynamically import Map to avoid SSR issues
const BusStopMap = dynamic(() => import('@/components/maps/BusStopMap').then(mod => ({ default: mod.default })), {
  ssr: false,
  loading: () => <MapLoader />
});

// Dynamically import EntityDropdown to avoid SSR issues
const EntityDropdown = dynamic(() => import("@/components/EntityDropdown"), { ssr: false });

export default function Home() {
  const [selectedStop, setSelectedStop] = useState<BusStopModel | null>(null);
  const [busStops, setBusStops] = useState<BusStopModel[]>([]);
  const [isLoading, setIsLoading] = useState(true);

  useEffect(() => {
    fetchBusStops();
  }, []);

  const fetchBusStops = async () => {
    try {
      const data = await BusStopService.read();
      setBusStops(data.items);
    } catch (error) {
      console.error('Failed to fetch bus stops:', error);
    } finally {
      setIsLoading(false);
    }
  };

  return (
    <div className="bg-white dark:bg-slate-950 min-h-screen flex flex-col">
      {/* Main Content - Two Column Layout */}
      <section className="flex-1 max-w-7xl mx-auto w-full px-4 sm:px-6 lg:px-8 py-6">
        <div className="grid grid-cols-1 lg:grid-cols-3 gap-6 h-[800px]">
          {/* Left Pane - Search and Info */}
          <div className="lg:col-span-1 bg-gray-50 dark:bg-slate-900 rounded-lg border border-gray-200 dark:border-slate-700 p-6 overflow-y-auto">
            <h2 className="text-2xl font-bold text-gray-900 dark:text-white mb-6">
              Find a Bus Stop
            </h2>
            {/* Search Box */}
            <div className="mb-8">
              <EntityDropdown
                value={selectedStop?.id || 0}
                onChange={(e) => setSelectedStop(e ? e.data : null)}
                placeholder="Search bus stops..."
                url="/api/bus-stops"
                sorts={[
                  { field: "name", dir: "asc" }
                ]}
                parseData={(data: PageResult<BusStopModel>) =>
                  data.items.map((item) => {
                    return {
                      value: item.id,
                      label: item.name,
                      data: item
                    };
                  })
                }
              />
            </div>
          </div>

          {/* Right Pane - Map */}
          <div className="lg:col-span-2 bg-gray-100 dark:bg-slate-800 rounded-lg border border-gray-200 dark:border-slate-700 overflow-hidden">
            <BusStopMap
              busStops={busStops}
              selectedStopId={selectedStop?.id}
            />
          </div>
        </div>
      </section>
    </div>
  );
}