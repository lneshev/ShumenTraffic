'use client';

import BusStopService from '@/services/BusStopService';
import BusStopModel from '@/types/BusStopModel';
import { useEffect, useRef, useState } from 'react';

interface BusStopSearchProps {
  onSelectStop: (stop: BusStopModel) => void;
  selectedStopId?: number;
}

export function BusStopSearch({ onSelectStop, selectedStopId }: BusStopSearchProps) {
  const [searchTerm, setSearchTerm] = useState('');
  const [busStops, setBusStops] = useState<BusStopModel[]>([]);
  const [filteredStops, setFilteredStops] = useState<BusStopModel[]>([]);
  const [isOpen, setIsOpen] = useState(false);
  const [isLoading, setIsLoading] = useState(false);
  const searchRef = useRef<HTMLDivElement>(null);

  // Fetch all bus stops on mount
  useEffect(() => {
    fetchBusStops();
  }, []);

  // Filter stops based on search term
  useEffect(() => {
    if (searchTerm.trim() === '') {
      setFilteredStops([]);
      setIsOpen(false);
    } else {
      const filtered = busStops.filter((stop) =>
        stop.name.toLowerCase().includes(searchTerm.toLowerCase())
      );
      setFilteredStops(filtered);
      setIsOpen(filtered.length > 0);
    }
  }, [searchTerm, busStops]);

  // Close dropdown when clicking outside
  useEffect(() => {
    const handleClickOutside = (event: MouseEvent) => {
      if (searchRef.current && !searchRef.current.contains(event.target as Node)) {
        setIsOpen(false);
      }
    };

    document.addEventListener('mousedown', handleClickOutside);
    return () => document.removeEventListener('mousedown', handleClickOutside);
  }, []);

  const fetchBusStops = async () => {
    try {
      setIsLoading(true);
      const data = await BusStopService.read();
      setBusStops(data.items);
    } catch (error) {
      console.error('Failed to fetch bus stops:', error);
    } finally {
      setIsLoading(false);
    }
  };

  const handleSelectStop = (stop: BusStopModel) => {
    onSelectStop(stop);
    setSearchTerm(stop.name);
    setIsOpen(false);
  };

  return (
    <div ref={searchRef} className="relative">
      <div className="relative">
        <input
          type="text"
          placeholder="Search bus stops..."
          value={searchTerm}
          onChange={(e) => setSearchTerm(e.target.value)}
          onFocus={() => searchTerm && setIsOpen(true)}
          className="w-full px-4 py-2 border border-gray-300 dark:border-slate-600 rounded-lg bg-white dark:bg-slate-800 text-gray-900 dark:text-white placeholder-gray-500 dark:placeholder-gray-400 focus:outline-none focus:ring-2 focus:ring-blue-600 focus:border-transparent"
        />
        {isLoading && (
          <div className="absolute right-3 top-2.5">
            <div className="animate-spin rounded-full h-5 w-5 border-b-2 border-blue-600"></div>
          </div>
        )}
      </div>

      {/* Dropdown */}
      {isOpen && filteredStops.length > 0 && (
        <div className="absolute top-full left-0 right-0 mt-1 bg-white dark:bg-slate-800 border border-gray-300 dark:border-slate-600 rounded-lg shadow-lg z-50 max-h-64 overflow-y-auto">
          {filteredStops.map((stop) => (
            <button
              key={stop.id}
              onClick={() => handleSelectStop(stop)}
              className={`w-full text-left px-4 py-2 hover:bg-blue-50 dark:hover:bg-blue-900/20 transition-colors ${selectedStopId === stop.id
                ? 'bg-blue-100 dark:bg-blue-900/30 border-l-4 border-blue-600'
                : ''
                }`}
            >
              <p className="font-medium text-gray-900 dark:text-white">{stop.name}</p>
              <p className="text-xs text-gray-600 dark:text-gray-400">
                {stop.location.latitude}, {stop.location.longitude}
              </p>
            </button>
          ))}
        </div>
      )}

      {/* No results message */}
      {isOpen && filteredStops.length === 0 && searchTerm && (
        <div className="absolute top-full left-0 right-0 mt-1 bg-white dark:bg-slate-800 border border-gray-300 dark:border-slate-600 rounded-lg shadow-lg z-50 p-4">
          <p className="text-gray-600 dark:text-gray-400 text-sm">
            No bus stops found matching "{searchTerm}"
          </p>
        </div>
      )}
    </div>
  );
}