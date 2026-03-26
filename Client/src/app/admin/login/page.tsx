'use client';

import { useAuth } from '@/context/AuthContext';
import { ApiError } from '@/types/common/ApiError';
import Link from 'next/link';
import { useRouter } from 'next/navigation';
import { useState } from 'react';

export default function AdminLoginPage() {
  const [username, setUsername] = useState('');
  const [password, setPassword] = useState('');
  const [error, setError] = useState('');
  const [isLoading, setIsLoading] = useState(false);
  const router = useRouter();
  const { login } = useAuth();

  const handleSubmit = async (e: React.SubmitEvent) => {
    e.preventDefault();
    setError('');
    setIsLoading(true);

    try {
      await login(username, password);
      router.push('/admin');
    } catch (err) {
      setError(err instanceof ApiError ? err.message : 'Login failed');
      setIsLoading(false);
    }
  };

  return (
    <div className="min-h-full bg-linear-to-br from-background-secondary to-background-secondary flex items-center justify-center px-4">
      <div className="w-full max-w-md">
        {/* Logo Section */}
        <div className="text-center mb-8">
          <div className="inline-flex items-center justify-center w-16 h-16 bg-linear-to-br from-green-600 to-green-800 rounded-lg mb-4">
            <span className="text-white font-bold text-2xl">ST</span>
          </div>
          <h1 className="text-3xl font-bold text-foreground">
            Shumen Traffic
          </h1>
          <p className="text-text-muted mt-2">
            Admin Portal
          </p>
        </div>

        {/* Login Form */}
        <div className="bg-background rounded-lg shadow-lg p-8 border border-border">
          <h2 className="text-2xl font-bold text-foreground mb-6">
            Sign In
          </h2>

          {error && (
            <div className="mb-6 p-4 bg-red-50 dark:bg-red-900/20 border border-red-200 dark:border-red-700 rounded-lg">
              <p className="text-red-800 dark:text-red-200 text-sm">{error}</p>
            </div>
          )}

          <form onSubmit={handleSubmit} className="space-y-4">
            {/* Username Field */}
            <div>
              <label
                htmlFor="username"
                className="label-standard"
              >
                Username
              </label>
              <input
                id="username"
                type="text"
                value={username}
                onChange={(e) => setUsername(e.target.value)}
                placeholder="Enter your username"
                className="w-full px-4 py-2 border border-border rounded-lg bg-background text-foreground placeholder-text-muted"
                required
                disabled={isLoading}
              />
            </div>

            {/* Password Field */}
            <div>
              <label
                htmlFor="password"
                className="label-standard"
              >
                Password
              </label>
              <input
                id="password"
                type="password"
                value={password}
                onChange={(e) => setPassword(e.target.value)}
                placeholder="Enter your password"
                className="w-full px-4 py-2 border border-border rounded-lg bg-background text-foreground placeholder-text-muted"
                required
                disabled={isLoading}
              />
            </div>

            {/* Submit Button */}
            <button
              type="submit"
              disabled={isLoading}
              className="w-full py-2 px-4 bg-blue-600 hover:bg-blue-700 disabled:bg-blue-400 text-white font-semibold rounded-lg transition-colors mt-6"
            >
              {isLoading ? 'Signing in...' : 'Sign In'}
            </button>
          </form>

          {/* Footer */}
          <div className="mt-6 pt-6 border-t border-border">
            <p className="text-center text-text-muted text-sm">
              Back to{' '}
              <Link
                href="/"
                className="text-blue-600 dark:text-blue-400 hover:text-blue-700 dark:hover:text-blue-300 font-medium"
              >
                Home
              </Link>
            </p>
          </div>
        </div>
      </div>
    </div>
  );
}

