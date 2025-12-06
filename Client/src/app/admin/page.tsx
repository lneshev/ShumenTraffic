'use client';

import { ProtectedRoute } from '@/components/ProtectedRoute';
import { useAuth } from '@/context/AuthContext';
import Link from 'next/link';

function AdminDashboard() {
  const { user, logout } = useAuth();

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
              Welcome, {user?.username}!
            </p>
          </div>
          <button
            onClick={logout}
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
            {[
              { label: 'Total Bus Lines', value: '12', color: 'blue' },
              { label: 'Total Routes', value: '48', color: 'green' },
              { label: 'Bus Stops', value: '156', color: 'orange' },
              { label: 'Active Schedules', value: '24', color: 'purple' },
            ].map((stat, i) => (
              <div
                key={i}
                className={`p-6 bg-${stat.color}-50 dark:bg-${stat.color}-900/20 rounded-lg border border-${stat.color}-200 dark:border-${stat.color}-700`}
              >
                <p className={`text-${stat.color}-600 dark:text-${stat.color}-400 text-sm font-medium mb-2`}>
                  {stat.label}
                </p>
                <p className="text-3xl font-bold text-gray-900 dark:text-white">
                  {stat.value}
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

