'use client';

import TimetablesService from '@/services/TimetablesService';
import BusLineLightModel from '@/types/BusLineLightModel';
import PageResult from '@/types/common/PageResult';
import TimetableModel from '@/types/TimetableModel';
import dynamic from 'next/dynamic';
import { useEffect, useState } from 'react';

// Dynamically import EntityDropdown to avoid SSR issues
const EntityDropdown = dynamic(() => import("@/components/EntityDropdown"), { ssr: false });

export default function SchedulePage() {
  const [selectedLineId, setSelectedLineId] = useState<number>(0);
  const [selectedDirection, setSelectedDirection] = useState<number>(1);
  const [selectedDate, setSelectedDate] = useState<string>(new Date().toISOString().split('T')[0]);
  const [timetable, setTimetable] = useState<TimetableModel | null>(null);
  const [highlightedStop, setHighlightedStop] = useState<number | null>(null);
  const [highlightedCourse, setHighlightedCourse] = useState<number | null>(null);
  const [isLoading, setIsLoading] = useState(false);

  // Fetch routes when line is selected
  useEffect(() => {
    if (!selectedLineId || !selectedDirection || !selectedDate) {
      setTimetable(null);
      return;
    }
    fetchTimetable();
  }, [selectedLineId, selectedDirection, selectedDate]);

  const fetchTimetable = async () => {
    try {
      setIsLoading(true);
      const data = await TimetablesService.get(selectedLineId, selectedDirection, selectedDate);
      setTimetable(data);
    } catch (error) {
      console.error('Failed to fetch timetable:', error);
    }
    finally {
      setIsLoading(false);
    }
  };

  return (
    <div className="min-h-screen bg-white dark:bg-slate-950 flex flex-col">
      <div className="max-w-7xl mx-auto w-full px-4 sm:px-6 lg:px-8 py-6">
        <h1 className="text-4xl font-bold text-gray-900 dark:text-white mb-4">
          Bus Schedule
        </h1>
        <p className="text-gray-600 dark:text-gray-400 text-lg mb-8">
          View detailed schedules for all bus lines.
        </p>

        {/* Controls */}
        <div className="grid grid-cols-1 md:grid-cols-4 gap-4 mb-8">
          {/* Line Selector */}
          <div>
            <label className="block text-sm font-medium text-gray-700 dark:text-gray-300 mb-2">
              Bus Line
            </label>
            <EntityDropdown
              value={selectedLineId}
              onChange={(e) => setSelectedLineId(e ? Number(e.value) : 0)}
              placeholder="Select..."
              url="/api/bus-lines-light"
              sorts={[{ field: "LineNumber", dir: "asc" }]}
              parseData={(data: PageResult<BusLineLightModel>) =>
                data.items.map((item) => {
                  return {
                    value: item.id,
                    label: item.lineNumber
                  };
                })
              }
            />
          </div>

          {/* Direction Buttons */}
          <div>
            <label className="block text-sm font-medium text-gray-700 dark:text-gray-300 mb-2">
              Direction
            </label>
            <div className="flex gap-2">
              {[1, 2].map(dir => (
                <button
                  key={dir}
                  onClick={() => setSelectedDirection(dir)}
                  className={`flex-1 px-3 py-2 rounded-lg font-semibold transition-colors text-sm ${selectedDirection === dir
                    ? 'bg-blue-600 text-white'
                    : 'bg-gray-200 dark:bg-slate-800 text-gray-900 dark:text-white hover:bg-gray-300 dark:hover:bg-slate-700'
                    }`}
                >
                  Dir {dir}
                </button>
              ))}
            </div>
          </div>

          {/* Date Picker */}
          <div>
            <label className="block text-sm font-medium text-gray-700 dark:text-gray-300 mb-2">
              Date
            </label>
            <input
              type="date"
              value={selectedDate}
              onChange={(e) => setSelectedDate(e.target.value)}
              className="w-full px-4 py-2 border border-gray-300 dark:border-slate-600 rounded-lg bg-white dark:bg-slate-800 text-gray-900 dark:text-white focus:outline-none focus:ring-2 focus:ring-blue-600"
            />
          </div>
        </div>

        {/* Schedule Table */}
        {!selectedLineId || !selectedDirection || !selectedDate ?
          <div className="text-center py-12">
            <p className="text-gray-600 dark:text-gray-400">Please select a bus line, direction and date to view the timetable.</p>
          </div>
          :
          !timetable && !isLoading && (
            <div className="text-center py-12">
              <p className="text-gray-600 dark:text-gray-400">No timetable found.</p>
            </div>
          )}

        {timetable && (
          <div className={`bg-gray-50 dark:bg-slate-900 rounded-lg border border-gray-200 dark:border-slate-700 overflow-x-auto ${isLoading ? 'opacity-50 pointer-events-none' : ''}`}>
            <table className="w-full text-sm border-separate border-spacing-0">
              <thead>
                <tr className="border-b-2 border-gray-300 dark:border-slate-600 bg-gray-100 dark:bg-slate-800">
                  <th className="text-left py-3 px-4 font-semibold text-gray-900 dark:text-white sticky left-0 bg-gray-100 dark:bg-slate-800 z-10">
                    Bus Stop
                  </th>
                  {timetable?.schedule.scheduleCourses.map(course => (
                    <th
                      key={course.id}
                      className={`text-center py-3 px-2 font-semibold text-gray-900 dark:text-white whitespace-nowrap transition-colors ${highlightedCourse === course.id
                        ? 'bg-blue-50 dark:bg-blue-900/20'
                        : ''
                        }`}

                    >
                      {course.departureTime.substring(0, 5)}
                    </th>
                  ))}
                </tr>
              </thead>
              <tbody>
                {timetable?.timetableRows.length === 0 ? (
                  <tr>
                    <td colSpan={timetable.schedule.scheduleCourses.length + 1} className="py-8 px-4 text-center text-gray-600 dark:text-gray-400">
                      No stops found for this route.
                    </td>
                  </tr>
                ) : (
                  timetable?.timetableRows.map((rs, idx) => (
                    <tr
                      key={rs.busStop.id}
                      className={`border-b border-gray-200 dark:border-slate-700 transition-colors ${highlightedStop === rs.busStop.id
                        ? 'bg-blue-50 dark:bg-blue-900/20'
                        : ''
                        }`}
                    >
                      <td
                        className={`py-3 px-4 font-medium text-gray-900 dark:text-white border-r border-gray-200 sticky left-0 z-10 transition-colors ${highlightedStop === rs.busStop.id
                          ? 'bg-blue-50 dark:bg-blue-900/20'
                          : 'bg-gray-50 dark:bg-slate-900'
                          }`}

                      >
                        <div className="flex items-center gap-2">
                          <span className="shrink-0 w-6 h-6 bg-blue-600 rounded-full flex items-center justify-center text-white text-xs font-bold">
                            {idx + 1}
                          </span>
                          <span className="truncate">{rs.busStop.name || 'Unknown'}</span>
                        </div>
                      </td>
                      {timetable.schedule.scheduleCourses.map(course => (
                        <td
                          key={`${rs.busStop.id}-${course.id}`}
                          className={`text-center py-3 px-2 text-gray-900 dark:text-white border-r border-gray-200 transition-colors cursor-pointer ${highlightedCourse === course.id
                            ? 'bg-blue-50 dark:bg-blue-900/20'
                            : ''
                            }`}
                          onClick={() => { setHighlightedStop(rs.busStop.id); setHighlightedCourse(course.id); }}
                        >
                          {rs.timesByVariant[course.id]}
                        </td>
                      ))}
                    </tr>
                  ))
                )}
              </tbody>
            </table>
          </div>
        )}
      </div>
    </div>
  );
}