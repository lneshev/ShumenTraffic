'use client';

import EntityDropdown from '@/components/EntityDropdown';
import EnumDropdown from '@/components/EnumDropdown';
import { ProtectedRoute } from '@/components/ProtectedRoute';
import { ApiError } from '@/lib/api';
import RouteOverviewService from '@/services/RouteOverviewService';
import RouteService from '@/services/RouteService';
import BusLineLightModel from '@/types/BusLineLightModel';
import PageResult from '@/types/common/PageResult';
import RouteOverviewModel from '@/types/RouteOverviewModel';
import Link from 'next/link';
import { useEffect, useState } from 'react';

function RoutesPage() {
  const initialFormData = {
    id: 0,
    name: '',
    direction: 0,
    directionText: '',
    isActive: true,
    busLineId: 0,
    busLineNumber: ''
  };

  const [routes, setRoutes] = useState<RouteOverviewModel[]>([]);
  const [isLoading, setIsLoading] = useState(true);
  const [error, setError] = useState('');
  const [showForm, setShowForm] = useState(false);
  const [formData, setFormData] = useState({ ...initialFormData });

  useEffect(() => {
    fetchRoutes();
  }, []);

  const fetchRoutes = async () => {
    try {
      setIsLoading(true);
      const data = await RouteOverviewService.read(undefined, [{ field: 'BusLineNumber', dir: 'asc' }, { field: 'DirectionText', dir: 'asc' }, { field: 'Name', dir: 'asc' }]);
      setRoutes(data.items);
    } catch (err) {
      setError(err instanceof ApiError ? err.message : 'Error loading routes');
    } finally {
      setIsLoading(false);
    }
  };

  const handleSubmit = async (e: React.FormEvent) => {
    e.preventDefault();
    try {
      setError('');
      await RouteOverviewService.create(formData);
      toggleShowForm(false);
      fetchRoutes();
    } catch (err) {
      setError(err instanceof ApiError ? err.message : 'Error creating route');
    }
  };

  const handleDelete = async (id: number) => {
    if (!confirm('Are you sure?')) return;
    try {
      setError('');
      await RouteService.delete(id);
      fetchRoutes();
    } catch (err) {
      setError(err instanceof ApiError ? err.message : 'Error deleting route');
    }
  };

  const toggleShowForm = (show: boolean) => {
    setError('');
    setFormData({ ...initialFormData });
    setShowForm(show);
  }

  return (
    <div className="min-h-screen bg-white dark:bg-slate-950">
      <div className="max-w-7xl mx-auto px-4 sm:px-6 lg:px-8 py-12">
        <div className="flex justify-between items-center mb-8">
          <h1 className="text-4xl font-bold text-gray-900 dark:text-white">
            Routes
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

        {/* Add Route Button */}
        <button
          onClick={() => toggleShowForm(!showForm)}
          className="mb-6 px-4 py-2 bg-blue-600 hover:bg-blue-700 text-white font-semibold rounded-lg transition-colors"
        >
          {showForm ? 'Cancel' : 'Add Route'}
        </button>

        {/* Add Route Form */}
        {showForm && (
          <form onSubmit={handleSubmit} className="mb-8 p-6 bg-gray-50 dark:bg-slate-900 rounded-lg border border-gray-200 dark:border-slate-700">
            <div className="grid grid-cols-1 lg:grid-cols-3 gap-4 mb-4">
              <div>
                <label className="block text-sm font-medium text-gray-700 dark:text-gray-300 mb-2">
                  Route Name
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
              <div>
                <label className="block text-sm font-medium text-gray-700 dark:text-gray-300 mb-2">
                  Bus Line
                </label>
                <EntityDropdown
                  value={formData.busLineId}
                  onChange={(e) => setFormData({ ...formData, busLineId: e ? e.value : 0 })}
                  placeholder="Select..."
                  url="/api/bus-lines-light"
                  sorts={[
                    { field: "LineNumber", dir: "asc" }
                  ]}
                  parseData={(data: PageResult<BusLineLightModel>) =>
                    data.items.map((item, i) => {
                      return {
                        value: item.id,
                        label: item.lineNumber
                      };
                    })
                  }
                  required
                />
              </div>
              <div>
                <label className="block text-sm font-medium text-gray-700 dark:text-gray-300 mb-2">
                  Direction
                </label>
                <EnumDropdown
                  enumName="RouteDirection"
                  value={formData.direction}
                  onChange={(e) => setFormData({ ...formData, direction: e ? e.value : 0 })}
                  required
                />
              </div>
            </div>
            <button
              type="submit"
              className="px-4 py-2 bg-green-600 hover:bg-green-700 text-white font-semibold rounded-lg transition-colors"
            >
              Create Route
            </button>
          </form>
        )}

        {/* Routes List */}
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
                    ID
                  </th>
                  <th className="text-left py-3 px-4 font-semibold text-gray-900 dark:text-white">
                    Name
                  </th>
                  <th className="text-left py-3 px-4 font-semibold text-gray-900 dark:text-white">
                    Bus Line
                  </th>
                  <th className="text-left py-3 px-4 font-semibold text-gray-900 dark:text-white">
                    Direction
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
                {routes.map((route) => (
                  <tr
                    key={route.id}
                    className="border-b border-gray-200 dark:border-slate-700 hover:bg-gray-100 dark:hover:bg-slate-800"
                  >
                    <td className="py-3 px-4 text-gray-900 dark:text-white">
                      {route.id}
                    </td>
                    <td className="py-3 px-4 text-gray-900 dark:text-white">
                      {route.name}
                    </td>
                    <td className="py-3 px-4 text-gray-900 dark:text-white">
                      {route.busLineNumber}
                    </td>
                    <td className="py-3 px-4 text-gray-900 dark:text-white">
                      {route.directionText}
                    </td>
                    <td className="py-3 px-4">
                      <span
                        className={`px-3 py-1 rounded-full text-xs font-medium ${route.isActive
                          ? 'bg-green-100 dark:bg-green-900 text-green-800 dark:text-green-200'
                          : 'bg-gray-100 dark:bg-gray-700 text-gray-800 dark:text-gray-200'
                          }`}
                      >
                        {route.isActive ? 'Active' : 'Inactive'}
                      </span>
                    </td>
                    <td className="py-3 px-4">
                      <Link
                        href={`/admin/routes/${route.id}`}
                        className="text-blue-600 dark:text-blue-400 hover:text-blue-700 dark:hover:text-blue-300 font-medium text-sm mr-4"
                      >
                        Edit
                      </Link>
                      <button
                        onClick={() => handleDelete(route.id)}
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

export default function RoutesPageWrapper() {
  return (
    <ProtectedRoute>
      <RoutesPage />
    </ProtectedRoute>
  );
}