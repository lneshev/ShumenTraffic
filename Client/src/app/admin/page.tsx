'use client';

import { ProtectedRoute } from '@/components/ProtectedRoute';
import { useAuth } from '@/context/AuthContext';
import BusLinesService from '@/services/BusLinesService';
import BusStopService from '@/services/BusStopService';
import RouteService from '@/services/RouteService';
import ScheduleService from '@/services/ScheduleService';
import NumberFlow from '@number-flow/react';
import Link from 'next/link';
import { useRouter } from 'next/navigation';

import { useEffect, useState } from 'react';

function AdminDashboard() {
  const statStyles: Record<string, string> = {
    purple: "bg-purple-50 dark:bg-purple-900/20 border-purple-200 dark:border-purple-700",
    green: "bg-green-50 dark:bg-green-900/20 border-green-200 dark:border-green-700",
    blue: "bg-blue-50 dark:bg-blue-900/20 border-blue-200 dark:border-blue-700",
    orange: "bg-orange-50 dark:bg-orange-900/20 border-orange-200 dark:border-orange-700"
  };

  const initialStatistics = [
    { label: 'Total Schedules', value: "0", color: 'purple', isError: false },
    { label: 'Total Routes', value: "0", color: 'green', isError: false },
    { label: 'Total Bus Stops', value: "0", color: 'blue', isError: false },
    { label: 'Total Bus Lines', value: "0", color: 'orange', isError: false },
  ];

  const { user, logout, isLoading } = useAuth();
  const [statistics, setStatistics] = useState([...initialStatistics]);
  const router = useRouter();

  useEffect(() => {
    fetchStatistics();
  }, []);

  const fetchStatistics = () => {
    fetchStatistic(ScheduleService.count, 0);
    fetchStatistic(RouteService.count, 1);
    fetchStatistic(BusStopService.count, 2);
    fetchStatistic(BusLinesService.count, 3);
  }

  const fetchStatistic = async (service: () => Promise<number>, index: number) => {
    try {
      const count = await service();
      setStatistics((prev) => {
        prev[index].value = count.toString();
        prev[index].isError = false;
        return [...prev];
      });
    } catch (err) {
      setStatistics((prev) => {
        prev[index].value = "Error";
        prev[index].isError = true;
        return [...prev];
      });
    }
  };

  return (
    <div className="min-h-screen bg-white dark:bg-slate-950">
      <div className="max-w-7xl mx-auto px-4 sm:px-6 lg:px-8 py-12">
        {/* Header with user info and logout */}
        <div className="flex justify-between items-center mb-8">
          <div>
            <h1 className="text-4xl font-bold text-gray-900 dark:text-white mb-2">
              Admin Dashboard
            </h1>
            <p className="text-gray-600 dark:text-gray-400">
              Welcome, {!isLoading ? user?.username : '...'}!
            </p>
          </div>
          <button
            onClick={() => { logout(); router.push('/admin/login'); }}
            className="px-4 py-2 bg-red-600 hover:bg-red-700 text-white font-semibold rounded-lg transition-colors"
          >
            Logout
          </button>
        </div>

        <p className="text-gray-600 dark:text-gray-400 text-lg mb-12">
          Manage bus lines, routes, schedules, and other system data.
        </p>

        {/* Admin Navigation */}
        <div className="grid grid-cols-1 md:grid-cols-2 lg:grid-cols-4 gap-6 mb-12">
          {[
            {
              title: 'Schedules',
              description: 'Create and edit schedules',
              icon: '📅',
              href: '/admin/schedules',
            },
            {
              title: 'Routes',
              description: 'Manage routes and stops',
              icon: '🗺️',
              href: '/admin/routes',
            },
            {
              title: 'Bus Stops',
              description: 'Manage bus stop locations',
              icon: '📍',
              href: '/admin/bus-stops',
            },
            {
              title: 'Zones',
              description: 'Manage service zones',
              icon: '🎯',
              href: '/admin/zones',
            },
            {
              title: 'Bus Lines',
              description: 'Create and edit bus lines',
              icon: '🚌',
              href: '/admin/bus-lines',
            },
            {
              title: 'Transportation Companies',
              description: 'Manage bus companies',
              icon: '🏢',
              href: '/admin/companies',
            }
          ].map((item, i) => (
            <Link
              key={i}
              href={item.href}
              className="p-6 bg-linear-to-br from-blue-50 to-blue-100 dark:from-blue-900/20 dark:to-blue-800/20 rounded-lg border border-blue-200 dark:border-blue-700 hover:shadow-lg transition-shadow cursor-pointer block"
            >
              <div className="text-4xl mb-3">{item.icon}</div>
              <h3 className="text-lg font-semibold text-gray-900 dark:text-white mb-2">
                {item.title}
              </h3>
              <p className="text-gray-600 dark:text-gray-400 text-sm mb-4">
                {item.description}
              </p>
              <span className="text-blue-600 dark:text-blue-400 font-medium text-sm hover:text-blue-700 dark:hover:text-blue-300 transition-colors">
                Manage →
              </span>
            </Link>
          ))}
        </div>

        {/* Quick Stats */}
        <section>
          <h2 className="text-2xl font-bold text-gray-900 dark:text-white mb-6">
            System Statistics
          </h2>
          <div className="grid grid-cols-1 md:grid-cols-4 gap-6">
            {statistics.map((stat, i) => (
              <div
                key={i}
                className={`${statStyles[stat.color]} p-6 rounded-lg border`}
              >
                <p className="text-sm font-medium mb-2">
                  {stat.label}
                </p>
                <p className={`text-3xl font-bold ${stat.isError ? 'text-red-600 dark:text-red-400' : 'text-gray-900 dark:text-white'}`}>
                  {!stat.isError ? <NumberFlow value={parseInt(stat.value)} /> : stat.value}
                </p>
              </div>
            ))}
          </div>
        </section>
      </div>
    </div>
  );
}

export default function AdminPage() {
  return (
    <ProtectedRoute>
      <AdminDashboard />
    </ProtectedRoute>
  );
}