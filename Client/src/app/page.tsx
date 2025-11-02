'use client';

import Link from "next/link";
import dynamic from 'next/dynamic';
import { useState, useEffect } from 'react';
import { BusStopSearch } from '@/components/BusStopSearch';
import api from '@/lib/api';

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

export default function Home() {
  const [selectedStop, setSelectedStop] = useState<BusStop | null>(null);
  const [busStops, setBusStops] = useState<BusStop[]>([]);
  const [isLoading, setIsLoading] = useState(true);

  useEffect(() => {
    const fetchBusStops = async () => {
      try {
        const data = await api.get<BusStop[]>('/bus-stops');
        setBusStops(data);
      } catch (error) {
        console.error('Failed to fetch bus stops:', error);
      } finally {
        setIsLoading(false);
      }
    };

    fetchBusStops();
  }, []);

  return (
    <div className="bg-white dark:bg-slate-950 min-h-screen flex flex-col">
      {/* Main Content - Two Column Layout */}
      <section className="flex-1 max-w-7xl mx-auto w-full px-4 sm:px-6 lg:px-8 py-6">
        <div className="grid grid-cols-1 lg:grid-cols-3 gap-6 h-[600px]">
          {/* Left Pane - Search and Info */}
          <div className="lg:col-span-1 bg-gray-50 dark:bg-slate-900 rounded-lg border border-gray-200 dark:border-slate-700 p-6 overflow-y-auto">
            <h2 className="text-2xl font-bold text-gray-900 dark:text-white mb-6">
              Find a Bus Stop
            </h2>

            {/* Search Box */}
            <div className="mb-8">
              <BusStopSearch
                onSelectStop={setSelectedStop}
                selectedStopId={selectedStop?.id}
              />
            </div>

            {/* Selected Stop Info */}
            {selectedStop && (
              <div className="mb-8 p-4 bg-blue-50 dark:bg-blue-900/20 border border-blue-200 dark:border-blue-700 rounded-lg">
                <h3 className="font-semibold text-gray-900 dark:text-white mb-2">
                  Selected Stop
                </h3>
                <p className="text-lg font-medium text-blue-600 dark:text-blue-400 mb-2">
                  {selectedStop.name}
                </p>
                <p className="text-sm text-gray-600 dark:text-gray-400">
                  Coordinates: {selectedStop.latitude.toFixed(6)}, {selectedStop.longitude.toFixed(6)}
                </p>
              </div>
            )}

            {/* Quick Links */}
            <div className="space-y-3">
              <h3 className="font-semibold text-gray-900 dark:text-white mb-3">
                Quick Links
              </h3>
              <Link
                href="/lines"
                className="block w-full px-4 py-2 bg-blue-600 hover:bg-blue-700 text-white font-semibold rounded-lg transition-colors text-center"
              >
                View Bus Lines
              </Link>
              <Link
                href="/schedule"
                className="block w-full px-4 py-2 bg-gray-200 dark:bg-slate-800 hover:bg-gray-300 dark:hover:bg-slate-700 text-gray-900 dark:text-white font-semibold rounded-lg transition-colors text-center"
              >
                Check Schedule
              </Link>
              <Link
                href="/info"
                className="block w-full px-4 py-2 bg-gray-200 dark:bg-slate-800 hover:bg-gray-300 dark:hover:bg-slate-700 text-gray-900 dark:text-white font-semibold rounded-lg transition-colors text-center"
              >
                Information
              </Link>
            </div>
          </div>

          {/* Right Pane - Map */}
          <div className="lg:col-span-2 bg-gray-100 dark:bg-slate-800 rounded-lg border border-gray-200 dark:border-slate-700 overflow-hidden">
            {isLoading ? (
              <div className="w-full h-full flex items-center justify-center">
                <div className="text-center">
                  <div className="inline-block animate-spin rounded-full h-12 w-12 border-b-2 border-blue-600 mb-4"></div>
                  <p className="text-gray-600 dark:text-gray-400">Loading map...</p>
                </div>
              </div>
            ) : (
              <Map busStops={busStops} selectedStopId={selectedStop?.id} />
            )}
          </div>
        </div>
      </section>

      {/* Features Section */}
      <section className="bg-gray-50 dark:bg-slate-900 py-16">
        <div className="max-w-7xl mx-auto px-4 sm:px-6 lg:px-8">
          <h2 className="text-3xl font-bold text-gray-900 dark:text-white mb-12 text-center">
            Features
          </h2>
          <div className="grid grid-cols-1 md:grid-cols-3 gap-8">
            {[
              {
                icon: "🚌",
                title: "Real-time Tracking",
                description: "Track buses in real-time on an interactive map",
              },
              {
                icon: "📅",
                title: "Schedule Information",
                description: "View detailed schedules for all bus lines",
              },
              {
                icon: "📍",
                title: "Stop Locator",
                description: "Find nearby bus stops and their information",
              },
            ].map((feature, i) => (
              <div
                key={i}
                className="p-6 bg-white dark:bg-slate-800 rounded-lg border border-gray-200 dark:border-slate-700 hover:shadow-lg transition-shadow"
              >
                <div className="text-4xl mb-4">{feature.icon}</div>
                <h3 className="text-lg font-semibold text-gray-900 dark:text-white mb-2">
                  {feature.title}
                </h3>
                <p className="text-gray-600 dark:text-gray-400">
                  {feature.description}
                </p>
              </div>
            ))}
          </div>
        </div>
      </section>

      {/* Quick Links Section */}
      <section className="max-w-7xl mx-auto px-4 sm:px-6 lg:px-8 py-16">
        <h2 className="text-3xl font-bold text-gray-900 dark:text-white mb-12 text-center">
          Quick Links
        </h2>
        <div className="grid grid-cols-1 md:grid-cols-2 lg:grid-cols-4 gap-6">
          {[
            { label: "Bus Lines", href: "/lines", icon: "🚌" },
            { label: "Schedule", href: "/schedule", icon: "📅" },
            { label: "Information", href: "/info", icon: "ℹ️" },
            { label: "Admin", href: "/admin", icon: "⚙️" },
          ].map((link, i) => (
            <Link
              key={i}
              href={link.href}
              className="p-6 bg-gradient-to-br from-blue-50 to-blue-100 dark:from-blue-900/20 dark:to-blue-800/20 rounded-lg border border-blue-200 dark:border-blue-700 hover:shadow-lg transition-shadow text-center"
            >
              <div className="text-4xl mb-3">{link.icon}</div>
              <h3 className="text-lg font-semibold text-gray-900 dark:text-white">
                {link.label}
              </h3>
            </Link>
          ))}
        </div>
      </section>
    </div>
  );
}
