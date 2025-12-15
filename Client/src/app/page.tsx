'use client';

import MapLoader from '@/components/maps/MapLoader';
import BusLinesService from '@/services/BusLinesService';
import BusStopService from "@/services/BusStopService";
import ZoneWithBusLinesService from '@/services/ZoneWithBusLinesService';
import BusLineModel from '@/types/BusLineModel';
import BusStopModel from "@/types/BusStopModel";
import PageResult from '@/types/common/PageResult';
import TransportationCompanyWithBusLinesModel from '@/types/TransportationCompanyWithBusLinesModel';
import ZoneWithBusLinesModel from '@/types/ZoneWithBusLinesModel';
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
  const [zonesWithBusLines, setZonesWithBusLines] = useState<ZoneWithBusLinesModel[]>([]);
  const [transportationCompanies, setTransportationCompanies] = useState<TransportationCompanyWithBusLinesModel[]>([]);
  const [isLoading, setIsLoading] = useState(true);

  useEffect(() => {
    fetchBusStops();
    fetchBusLines();
    fetchZonesWithBusLines();
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

  const fetchBusLines = async () => {
    try {
      const data = await BusLinesService.read(undefined, [{ field: 'LineNumber', dir: 'asc' }]);
      const transportationCompaniesByBusLines = groupBusLinesByCompany(data);
      setTransportationCompanies(transportationCompaniesByBusLines);
    } catch (error) {
      console.error('Failed to fetch bus lines:', error);
    }
  };

  const groupBusLinesByCompany = (data: PageResult<BusLineModel>) => {
    const result: TransportationCompanyWithBusLinesModel[] = [];
    let notServiced: TransportationCompanyWithBusLinesModel | undefined = undefined;

    data.items.forEach(line => {
      if (line.transportationCompanies.length === 0) {
        if (!notServiced) {
          notServiced = { id: 0, name: 'Not serviced', busLines: [] };
        }
        notServiced!.busLines.push(line);
      }

      line.transportationCompanies.forEach(company => {
        let c = result.find(x => x.id === company.id);
        if (!c) {
          result.push({ ...company, busLines: [] });
          c = result[result.length - 1];
        }
        c.busLines.push(line);
      });
    });

    result.sort((a, b) => a.name.localeCompare(b.name));
    if (notServiced) {
      result.push(notServiced!);
    }

    return result;
  }

  const fetchZonesWithBusLines = async () => {
    try {
      const data = await ZoneWithBusLinesService.read(undefined, [{ field: 'Name', dir: 'asc' }]);
      setZonesWithBusLines(data.items);
    } catch (error) {
      console.error('Failed to fetch zones:', error);
    }
  };

  return (
    <div className="bg-white dark:bg-slate-950 min-h-screen flex flex-col">
      {/* Main Content - Two Column Layout */}
      <section className="flex-1 max-w-7xl mx-auto w-full px-4 sm:px-6 lg:px-8 py-6">
        <div className="grid grid-cols-1 lg:grid-cols-3 gap-6 h-[800px]">
          {/* Left Pane - Search and Info */}
          <div className="lg:col-span-1 bg-gray-50 dark:bg-slate-900 rounded-lg border border-gray-200 dark:border-slate-700 p-6 overflow-y-auto">
            <h2 className="text-xl font-bold text-gray-900 dark:text-white mb-2">
              Find a Bus Stop
            </h2>
            {/* Search Box */}
            <div className="mb-6">
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
            <h2 className="text-xl font-bold text-gray-900 dark:text-white mb-2">
              Transportation Companies
            </h2>
            <ul className="mb-6">
              {transportationCompanies.map(company => (
                <li key={company.id} className="mb-1">
                  <h3 className="font-bold">{company.name}</h3>
                  {company.busLines.length > 0 ? company.busLines.map(line => line.lineNumber).join(', ') : "-"}
                </li>
              ))}
            </ul>
            <h2 className="text-xl font-bold text-gray-900 dark:text-white mb-2">
              Zones
            </h2>
            <ul>
              {zonesWithBusLines.map(zone => (
                <li key={zone.id} className="mb-1">
                  <h3 className="font-bold">{zone.name}</h3>
                  {zone.busLines.length > 0 ? zone.busLines.map(line => line.lineNumber).join(', ') : "-"}
                </li>
              ))}
            </ul>
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