'use client';

import { ProtectedRoute } from '@/components/ProtectedRoute';
import Link from 'next/link';
import { useState, useEffect } from 'react';
import api from '@/lib/api';

interface BusLine {
  id: number;
  lineNumber: string;
  transportationCompanyId: number;
  isActive: boolean;
}

function BusLinesPage() {
  const [busLines, setBusLines] = useState<BusLine[]>([]);
  const [isLoading, setIsLoading] = useState(true);
  const [error, setError] = useState('');
  const [showForm, setShowForm] = useState(false);
  const [formData, setFormData] = useState({ lineNumber: '', transportationCompanyId: 1 });

  useEffect(() => {
    fetchBusLines();
  }, []);

  const fetchBusLines = async () => {
    try {
      setIsLoading(true);
      const data = await api.get<BusLine[]>('/bus-lines');
      setBusLines(data);
    } catch (err) {
      setError(err instanceof api.ApiError ? err.message : 'Error loading bus lines');
    } finally {
      setIsLoading(false);
    }
  };

  const handleSubmit = async (e: React.FormEvent) => {
    e.preventDefault();
    try {
      setError('');
      await api.post('/bus-lines', formData);
      setFormData({ lineNumber: '', transportationCompanyId: 1 });
      setShowForm(false);
      fetchBusLines();
    } catch (err) {
      setError(err instanceof api.ApiError ? err.message : 'Error creating bus line');
    }
  };

  const handleDelete = async (id: number) => {
    if (!confirm('Are you sure?')) return;
    try {
      await api.delete(`/bus-lines/${id}`);
      fetchBusLines();
    } catch (err) {
      setError(err instanceof api.ApiError ? err.message : 'Error deleting bus line');
    }
  };

  return (
    <div className="min-h-screen bg-white dark:bg-slate-950">
      <div className="max-w-7xl mx-auto px-4 sm:px-6 lg:px-8 py-12">
        <div className="flex justify-between items-center mb-8">
          <h1 className="text-4xl font-bold text-gray-900 dark:text-white">
            Bus Lines
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

        {/* Add Bus Line Button */}
        <button
          onClick={() => setShowForm(!showForm)}
          className="mb-6 px-4 py-2 bg-blue-600 hover:bg-blue-700 text-white font-semibold rounded-lg transition-colors"
        >
          {showForm ? 'Cancel' : 'Add Bus Line'}
        </button>

        {/* Add Bus Line Form */}
        {showForm && (
          <form onSubmit={handleSubmit} className="mb-8 p-6 bg-gray-50 dark:bg-slate-900 rounded-lg border border-gray-200 dark:border-slate-700">
            <div className="mb-4">
              <label className="block text-sm font-medium text-gray-700 dark:text-gray-300 mb-2">
                Line Number
              </label>
              <input
                type="text"
                value={formData.lineNumber}
                onChange={(e) => setFormData({ ...formData, lineNumber: e.target.value })}
                className="w-full px-4 py-2 border border-gray-300 dark:border-slate-600 rounded-lg bg-white dark:bg-slate-800 text-gray-900 dark:text-white"
                required
              />
            </div>
            <button
              type="submit"
              className="px-4 py-2 bg-green-600 hover:bg-green-700 text-white font-semibold rounded-lg transition-colors"
            >
              Create Bus Line
            </button>
          </form>
        )}

        {/* Bus Lines List */}
        {isLoading ? (
          <div className="text-center py-12">
            <div className="inline-block animate-spin rounded-full h-12 w-12 border-b-2 border-blue-600"></div>
          </div>
        ) : (
          <div className="bg-gray-50 dark:bg-slate-900 rounded-lg border border-gray-200 dark:border-slate-700 overflow-hidden">
            <table className="w-full text-sm">
              <thead>
                <tr className="border-b border-gray-200 dark:border-slate-700 bg-gray-100 dark:bg-slate-800">
                  <th className="text-left py-3 px-4 font-semibold text-gray-900 dark:text-white">
                    Line Number
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
                {busLines.map((line) => (
                  <tr
                    key={line.id}
                    className="border-b border-gray-200 dark:border-slate-700 hover:bg-gray-100 dark:hover:bg-slate-800"
                  >
                    <td className="py-3 px-4 text-gray-900 dark:text-white">
                      {line.lineNumber}
                    </td>
                    <td className="py-3 px-4">
                      <span
                        className={`px-3 py-1 rounded-full text-xs font-medium ${line.isActive
                          ? 'bg-green-100 dark:bg-green-900 text-green-800 dark:text-green-200'
                          : 'bg-gray-100 dark:bg-gray-700 text-gray-800 dark:text-gray-200'
                          }`}
                      >
                        {line.isActive ? 'Active' : 'Inactive'}
                      </span>
                    </td>
                    <td className="py-3 px-4">
                      <button
                        onClick={() => handleDelete(line.id)}
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

export default function BusLinesPageWrapper() {
  return (
    <ProtectedRoute>
      <BusLinesPage />
    </ProtectedRoute>
  );
}

