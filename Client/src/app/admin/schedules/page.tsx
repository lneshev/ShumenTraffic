'use client';

import EntityDropdown from '@/components/EntityDropdown';
import EnumDropdown from '@/components/EnumDropdown';
import FlagsEnumMultiselect from '@/components/FlagsEnumMultiselect';
import { ProtectedRoute } from '@/components/ProtectedRoute';
import ScheduleOverviewService from '@/services/ScheduleOverviewService';
import ScheduleService from '@/services/ScheduleService';
import BusLineLightModel from '@/types/BusLineLightModel';
import { ApiError } from '@/types/common/ApiError';
import PageResult from '@/types/common/PageResult';
import ServerEnums from '@/types/common/ServerEnums';
import ScheduleOverviewModel from '@/types/ScheduleOverviewModel';
import { DateTime } from 'luxon';
import Link from 'next/link';
import { useEffect, useState } from 'react';

function SchedulesPage() {
  const initialFormData: ScheduleOverviewModel = {
    id: 0,
    daysOfWeek: ServerEnums.DaysOfWeek.Weekdays,
    daysOfWeekText: '',
    startDate: DateTime.now().toISODate(),
    endDate: '',
    isActive: true,
    priority: ServerEnums.SchedulePriority.Normal,
    priorityText: '',
    busLineId: 0,
    busLineNumber: '',
    direction: 0,
    directionText: ''
  };

  const [schedules, setSchedules] = useState<ScheduleOverviewModel[]>([]);
  const [isLoading, setIsLoading] = useState(true);
  const [error, setError] = useState('');
  const [showForm, setShowForm] = useState(false);
  const [formData, setFormData] = useState({ ...initialFormData });

  useEffect(() => {
    fetchSchedules();
  }, []);

  const fetchSchedules = async () => {
    try {
      setIsLoading(true);
      const data = await ScheduleOverviewService.read(undefined, [
        { field: 'BusLineNumber', dir: 'asc' },
        { field: 'Direction', dir: 'asc' },
        { field: 'DaysOfWeek', dir: 'asc' },
        { field: 'StartDate', dir: 'asc' },
        { field: 'Priority', dir: 'asc' }
      ]);
      setSchedules(data.items);
    } catch (err) {
      setError(err instanceof ApiError ? err.message : 'Error loading schedules');
    } finally {
      setIsLoading(false);
    }
  };

  const handleSubmit = async (e: React.FormEvent) => {
    e.preventDefault();
    try {
      setError('');
      await ScheduleOverviewService.create(formData);
      toggleShowForm(false);
      fetchSchedules();
    } catch (err) {
      setError(err instanceof ApiError ? err.message : 'Error creating schedule');
    }
  };

  const handleDelete = async (id: number) => {
    if (!confirm('Are you sure?')) return;
    try {
      setError('');
      await ScheduleService.delete(id);
      fetchSchedules();
    } catch (err) {
      setError(err instanceof ApiError ? err.message : 'Error deleting schedule');
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
            Schedules
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

        {/* Add Schedule Button */}
        <button
          onClick={() => toggleShowForm(!showForm)}
          className="mb-6 px-4 py-2 bg-blue-600 hover:bg-blue-700 text-white font-semibold rounded-lg transition-colors"
        >
          {showForm ? 'Cancel' : 'Add Schedule'}
        </button>

        {/* Add Schedule Form */}
        {showForm && (
          <form onSubmit={handleSubmit} className="mb-8 p-6 bg-gray-50 dark:bg-slate-900 rounded-lg border border-gray-200 dark:border-slate-700">
            <div className="grid grid-cols-1 lg:grid-cols-3 gap-4 mb-4">
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
                    data.items.map((item) => {
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
              <div>
                <label className="block text-sm font-medium text-gray-700 dark:text-gray-300 mb-2">
                  Days of Week
                </label>
                <FlagsEnumMultiselect
                  enumName="DaysOfWeek"
                  exactEnumValues={[
                    ServerEnums.DaysOfWeek.Monday,
                    ServerEnums.DaysOfWeek.Tuesday,
                    ServerEnums.DaysOfWeek.Wednesday,
                    ServerEnums.DaysOfWeek.Thursday,
                    ServerEnums.DaysOfWeek.Friday,
                    ServerEnums.DaysOfWeek.Saturday,
                    ServerEnums.DaysOfWeek.Sunday
                  ]}
                  value={formData.daysOfWeek}
                  onChange={(e) => setFormData({ ...formData, daysOfWeek: e ? e : 0 })}
                  required
                />
              </div>
              <div>
                <label className="block text-sm font-medium text-gray-700 dark:text-gray-300 mb-2">
                  Start Date
                </label>
                <input
                  type="date"
                  value={formData.startDate}
                  onChange={(e) => setFormData({ ...formData, startDate: e.target.value })}
                  className="w-full px-4 py-2 border border-gray-300 dark:border-slate-600 rounded-lg bg-white dark:bg-slate-800 text-gray-900 dark:text-white"
                  required
                />
              </div>
              <div>
                <label className="block text-sm font-medium text-gray-700 dark:text-gray-300 mb-2">
                  End Date
                </label>
                <input
                  type="date"
                  value={formData.endDate}
                  onChange={(e) => setFormData({ ...formData, endDate: e.target.value })}
                  className="w-full px-4 py-2 border border-gray-300 dark:border-slate-600 rounded-lg bg-white dark:bg-slate-800 text-gray-900 dark:text-white"
                />
              </div>
              <div>
                <label className="block text-sm font-medium text-gray-700 dark:text-gray-300 mb-2">
                  Priority
                </label>
                <EnumDropdown
                  enumName="SchedulePriority"
                  value={formData.priority}
                  onChange={(e) => setFormData({ ...formData, priority: e ? e.value : 0 })}
                  isClearable={false}
                  required
                />
              </div>
            </div>
            <button
              type="submit"
              className="px-4 py-2 bg-green-600 hover:bg-green-700 text-white font-semibold rounded-lg transition-colors"
            >
              Create Schedule
            </button>
          </form>
        )}

        {/* Schedules List */}
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
                    Bus Line
                  </th>
                  <th className="text-left py-3 px-4 font-semibold text-gray-900 dark:text-white">
                    Direction
                  </th>
                  <th className="text-left py-3 px-4 font-semibold text-gray-900 dark:text-white">
                    Days of Week
                  </th>
                  <th className="text-left py-3 px-4 font-semibold text-gray-900 dark:text-white">
                    Start Date
                  </th>
                  <th className="text-left py-3 px-4 font-semibold text-gray-900 dark:text-white">
                    End Date
                  </th>
                  <th className="text-left py-3 px-4 font-semibold text-gray-900 dark:text-white">
                    Priority
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
                {schedules.map((schedule) => (
                  <tr
                    key={schedule.id}
                    className="border-b border-gray-200 dark:border-slate-700 hover:bg-gray-100 dark:hover:bg-slate-800"
                  >
                    <td className="py-3 px-4 text-gray-900 dark:text-white">
                      {schedule.id}
                    </td>
                    <td className="py-3 px-4 text-gray-900 dark:text-white">
                      {schedule.busLineNumber}
                    </td>
                    <td className="py-3 px-4 text-gray-900 dark:text-white">
                      {schedule.directionText}
                    </td>
                    <td className="py-3 px-4 text-gray-900 dark:text-white">
                      {schedule.daysOfWeekText}
                    </td>
                    <td className="py-3 px-4 text-gray-900 dark:text-white">
                      {new Date(schedule.startDate).toLocaleDateString("bg-BG")}
                    </td>
                    <td className="py-3 px-4 text-gray-900 dark:text-white">
                      {schedule.endDate ? new Date(schedule.endDate).toLocaleDateString("bg-BG") : '-'}
                    </td>
                    <td className="py-3 px-4 text-gray-900 dark:text-white">
                      {schedule.priorityText}
                    </td>
                    <td className="py-3 px-4">
                      <span
                        className={`px-3 py-1 rounded-full text-xs font-medium ${schedule.isActive
                          ? 'bg-green-100 dark:bg-green-900 text-green-800 dark:text-green-200'
                          : 'bg-gray-100 dark:bg-gray-700 text-gray-800 dark:text-gray-200'
                          }`}
                      >
                        {schedule.isActive ? 'Active' : 'Inactive'}
                      </span>
                    </td>
                    <td className="py-3 px-4">
                      <Link
                        href={`/admin/schedules/${schedule.id}`}
                        className="text-blue-600 dark:text-blue-400 hover:text-blue-700 dark:hover:text-blue-300 font-medium text-sm mr-4"
                      >
                        Edit
                      </Link>
                      <button
                        onClick={() => handleDelete(schedule.id)}
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

export default function SchedulesPageWrapper() {
  return (
    <ProtectedRoute>
      <SchedulesPage />
    </ProtectedRoute>
  );
}

