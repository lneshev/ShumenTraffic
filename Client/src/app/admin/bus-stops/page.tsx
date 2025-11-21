'use client';

import EntityDropdown from '@/components/EntityDropdown';
import MapLoader from '@/components/maps/MapLoader';
import { ProtectedRoute } from '@/components/ProtectedRoute';
import MapMode from '@/enums/MapMode';
import { ApiError } from '@/lib/api';
import BusStopService from '@/services/BusStopService';
import BusStopModel from '@/types/BusStopModel';
import { GeoPoint } from '@/types/common/GeoJSON';
import PageResult from '@/types/common/PageResult';
import ZoneModel from '@/types/ZoneModel';
import dynamic from 'next/dynamic';
import Link from 'next/link';
import { useEffect, useRef, useState } from 'react';

// Dynamically import Map to avoid SSR issues
const Map = dynamic(() => import('@/components/maps/Map').then(mod => ({ default: mod.Map })), {
  ssr: false,
  loading: () => <MapLoader />
});

function BusStopsPage() {
  const initialFormData: BusStopModel = {
    id: 0,
    name: '',
    zoneId: 0,
    zoneName: '',
    location: new GeoPoint(43.270097, 26.924706),
    isActive: true
  };

  const [busStops, setBusStops] = useState<BusStopModel[]>([]);
  const cachedBusStops = useRef<BusStopModel[]>([]);
  const [isLoading, setIsLoading] = useState(true);
  const [error, setError] = useState('');
  const [showForm, setShowForm] = useState(false);
  const [formData, setFormData] = useState({ ...initialFormData });

  useEffect(() => {
    fetchBusStops();
  }, []);

  const fetchBusStops = async () => {
    try {
      setIsLoading(true);
      const data = await BusStopService.read(undefined, [{ field: 'Name', dir: 'asc' }]);
      cachedBusStops.current = data.items;
      setBusStops(cachedBusStops.current);
    } catch (err) {
      setError(err instanceof ApiError ? err.message : 'Error loading bus stops');
    } finally {
      setIsLoading(false);
    }
  };

  const handleSubmit = async (e: React.FormEvent) => {
    e.preventDefault();
    try {
      setError('');
      const createdStop = await BusStopService.create(formData);
      cachedBusStops.current = [createdStop, ...busStops];
      setBusStops(cachedBusStops.current);
      toggleShowForm(false);
    } catch (err) {
      setError(err instanceof ApiError ? err.message : 'Error creating bus stop');
    }
  };

  const handleDelete = async (id: number) => {
    if (!confirm('Are you sure?')) return;
    try {
      setError('');
      await BusStopService.delete(id);
      cachedBusStops.current = busStops.filter(x => x.id !== id);
      setBusStops(cachedBusStops.current);
    } catch (err) {
      setError(err instanceof ApiError ? err.message : 'Error deleting bus stop');
    }
  };

  const toggleShowForm = (show: boolean) => {
    setError('');
    setFormData({ ...initialFormData });
    setShowForm(show);
  }

  const handleBusStopDragEnd = async (stop: BusStopModel, newLat: number, newLng: number) => {
    try {
      setError('');
      let updatedStop = {
        ...stop,
        location: new GeoPoint(newLat, newLng)
      };
      updatedStop = await BusStopService.update(stop.id, updatedStop);
      cachedBusStops.current = busStops.map(x => x.id === stop.id ? updatedStop : x);
      setBusStops(cachedBusStops.current);
    } catch (err) {
      setBusStops(cachedBusStops.current);
      setError(err instanceof ApiError ? err.message : 'Error updating bus stop location');
    }
  }

  const handleBusStopNameChange = async (stop: BusStopModel, newName: string) => {
    if (stop.id === 0) {
      setFormData({ ...stop, name: newName });
    }
    else {
      const updatedStop = {
        ...stop,
        name: newName
      };
      setBusStops(prev => prev.map(x => x.id === stop.id ? updatedStop : x));
    }
  }

  const handleBusStopZoneIdChange = async (stop: BusStopModel, newZoneId: number) => {
    if (stop.id === 0) {
      setFormData({ ...stop, zoneId: newZoneId });
    }
    else {
      const updatedStop = {
        ...stop,
        zoneId: newZoneId
      };
      setBusStops(prev => prev.map(x => x.id === stop.id ? updatedStop : x));
    }
  }

  const handleBusStopSave = async (stop: BusStopModel, e: Event) => {
    try {
      setError('');
      if (stop.id === 0) {
        const createdStop = await BusStopService.create(stop);
        cachedBusStops.current = [createdStop, ...busStops];
        setBusStops(cachedBusStops.current);
      }
      else {
        const updatedStop = await BusStopService.update(stop.id, stop);
        cachedBusStops.current = busStops.map(x => x.id === stop.id ? updatedStop : x);
        setBusStops(cachedBusStops.current);
      }
    } catch (err) {
      e.preventDefault();
      setError(err instanceof ApiError ? err.message : 'Error updating bus stop');
    }
  }

  const handleBusStopDelete = async (stop: BusStopModel, e: Event) => {
    if (!confirm('Are you sure?')) {
      e.preventDefault();
      return;
    }
    try {
      setError('');
      await BusStopService.delete(stop.id);
      cachedBusStops.current = busStops.filter(x => x.id !== stop.id);
      setBusStops(cachedBusStops.current);
    } catch (err) {
      e.preventDefault();
      setError(err instanceof ApiError ? err.message : 'Error deleting bus stop');
    }
  }

  const handleBusStopCancel = async (stop: BusStopModel) => {
    setError('');
    setBusStops(cachedBusStops.current);
    setFormData({ ...initialFormData });
  }

  const handleMapRightClick = (lat: number, lng: number) => {
    setFormData({
      ...initialFormData,
      location: new GeoPoint(lat, lng)
    });
  }

  return (
    <div className="min-h-screen bg-white dark:bg-slate-950">
      <div className="max-w-7xl mx-auto px-4 sm:px-6 lg:px-8 py-12">
        <div className="flex justify-between items-center mb-8">
          <h1 className="text-4xl font-bold text-gray-900 dark:text-white">
            Bus Stops
          </h1>
          <Link
            href="/admin"
            className="px-4 py-2 bg-gray-600 hover:bg-gray-700 text-white font-semibold rounded-lg transition-colors"
          >
            Back to Dashboard
          </Link>
        </div>

        {error && (
          <div className="mb-6 p-4 bg-red-50 dark:bg-red-900/20 border border-red-200 dark:border-red-700 rounded-lg">
            <p className="text-red-800 dark:text-red-200">{error}</p>
          </div>
        )}

        {/* Add Bus Stop Button */}
        <button
          onClick={() => toggleShowForm(!showForm)}
          className="mb-6 mr-4 px-4 py-2 bg-blue-600 hover:bg-blue-700 text-white font-semibold rounded-lg transition-colors"
        >
          {showForm ? 'Cancel' : 'Add Bus Stop'}
        </button>

        <button
          onClick={() => fetchBusStops()}
          className="mb-6 px-4 py-2 bg-blue-600 hover:bg-blue-700 text-white font-semibold rounded-lg transition-colors float-right"
        >
          Refresh
        </button>

        {/* Add Bus Stop Form */}
        {showForm && (
          <form onSubmit={handleSubmit} className="mb-8 p-6 bg-gray-50 dark:bg-slate-900 rounded-lg border border-gray-200 dark:border-slate-700">
            <div className="mb-4">
              <label className="block text-sm font-medium text-gray-700 dark:text-gray-300 mb-2">
                Stop Name
              </label>
              <input
                type="text"
                value={formData.name}
                onChange={(e) => setFormData({ ...formData, name: e.target.value })}
                className="w-full px-4 py-2 border border-gray-300 dark:border-slate-600 rounded-lg bg-white dark:bg-slate-800 text-gray-900 dark:text-white"
                required
                maxLength={255}
              />
            </div>
            <div className="mb-4">
              <label className="block text-sm font-medium text-gray-700 dark:text-gray-300 mb-2">
                Zone
              </label>
              <EntityDropdown
                value={formData.zoneId}
                onChange={(e) => setFormData({ ...formData, zoneId: e ? e.value : 0 })}
                placeholder="Select..."
                url="/api/zones"
                sorts={[
                  { field: "name", dir: "asc" }
                ]}
                parseData={(data: PageResult<ZoneModel>) =>
                  data.items.map((item, i) => {
                    return {
                      value: item.id,
                      label: item.name
                    };
                  })
                }
                required
              />
            </div>
            <div className="grid grid-cols-2 gap-4 mb-4">
              <div>
                <label className="block text-sm font-medium text-gray-700 dark:text-gray-300 mb-2">
                  Latitude
                </label>
                <input
                  type="number"
                  step="0.000001"
                  value={formData.location.latitude}
                  onChange={(e) => setFormData((prev) => {
                    const updated = { ...prev };
                    updated.location.latitude = parseFloat(e.target.value);
                    return updated;
                  })}
                  className="w-full px-4 py-2 border border-gray-300 dark:border-slate-600 rounded-lg bg-white dark:bg-slate-800 text-gray-900 dark:text-white"
                  required
                />
              </div>
              <div>
                <label className="block text-sm font-medium text-gray-700 dark:text-gray-300 mb-2">
                  Longitude
                </label>
                <input
                  type="number"
                  step="0.000001"
                  value={formData.location.longitude}
                  onChange={(e) => setFormData((prev) => {
                    const updated = { ...prev };
                    updated.location.longitude = parseFloat(e.target.value);
                    return updated;
                  })}
                  className="w-full px-4 py-2 border border-gray-300 dark:border-slate-600 rounded-lg bg-white dark:bg-slate-800 text-gray-900 dark:text-white"
                  required
                />
              </div>
            </div>
            <button
              type="submit"
              className="px-4 py-2 bg-green-600 hover:bg-green-700 text-white font-semibold rounded-lg transition-colors"
            >
              Create Bus Stop
            </button>
          </form>
        )}

        <div className="grid grid-cols-1 gap-6 h-[600px] mb-6">
          <div className="lg:col-span-2 bg-gray-100 dark:bg-slate-800 rounded-lg border border-gray-200 dark:border-slate-700 overflow-hidden">
            <Map
              busStops={busStops}
              mode={MapMode.Edit}
              newBusStop={formData}
              onBusStopDragEnd={handleBusStopDragEnd}
              onBusStopNameChange={handleBusStopNameChange}
              onBusStopZoneIdChange={handleBusStopZoneIdChange}
              onBusStopSave={handleBusStopSave}
              onBusStopDelete={handleBusStopDelete}
              onBusStopCancel={handleBusStopCancel}
              onMapRightClick={handleMapRightClick}
            />
          </div>
        </div>

        {/* Bus Stops List */}
        {isLoading ? (
          <div className="text-center py-12">
            <div className="inline-block animate-spin rounded-full h-12 w-12 border-b-2 border-blue-600"></div>
          </div>
        ) : (
          <div className="bg-gray-50 dark:bg-slate-900 rounded-lg border border-gray-200 dark:border-slate-700 overflow-auto">
            <table className="w-full text-sm">
              <thead>
                <tr className="border-b border-gray-200 dark:border-slate-700 bg-gray-100 dark:bg-slate-800">
                  <th className="text-left py-3 px-4 font-semibold text-gray-900 dark:text-white">
                    Name
                  </th>
                  <th className="text-left py-3 px-4 font-semibold text-gray-900 dark:text-white">
                    Latitude
                  </th>
                  <th className="text-left py-3 px-4 font-semibold text-gray-900 dark:text-white">
                    Longitude
                  </th>
                  <th className="text-left py-3 px-4 font-semibold text-gray-900 dark:text-white">
                    Status
                  </th>
                  <th className="text-left py-3 px-4 font-semibold text-gray-900 dark:text-white">
                    Actions
                  </th>
                </tr>
              </thead>
              <tbody>
                {busStops.map((stop) => (
                  <tr
                    key={stop.id}
                    className="border-b border-gray-200 dark:border-slate-700 hover:bg-gray-100 dark:hover:bg-slate-800"
                  >
                    <td className="py-3 px-4 text-gray-900 dark:text-white">
                      {stop.name}
                    </td>
                    <td className="py-3 px-4 text-gray-900 dark:text-white text-xs">
                      {stop.location.latitude}
                    </td>
                    <td className="py-3 px-4 text-gray-900 dark:text-white text-xs">
                      {stop.location.longitude}
                    </td>
                    <td className="py-3 px-4">
                      <span
                        className={`px-3 py-1 rounded-full text-xs font-medium ${stop.isActive
                          ? 'bg-green-100 dark:bg-green-900 text-green-800 dark:text-green-200'
                          : 'bg-gray-100 dark:bg-gray-700 text-gray-800 dark:text-gray-200'
                          }`}
                      >
                        {stop.isActive ? 'Active' : 'Inactive'}
                      </span>
                    </td>
                    <td className="py-3 px-4">
                      <button
                        onClick={() => handleDelete(stop.id)}
                        className="text-red-600 dark:text-red-400 hover:text-red-700 dark:hover:text-red-300 font-medium text-sm"
                      >
                        Delete
                      </button>
                    </td>
                  </tr>
                ))}
              </tbody>
            </table>
          </div>
        )}
      </div>
    </div>
  );
}

export default function BusStopsPageWrapper() {
  return (
    <ProtectedRoute>
      <BusStopsPage />
    </ProtectedRoute>
  );
}