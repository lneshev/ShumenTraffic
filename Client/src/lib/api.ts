import ApiResponse from '@/types/common/ApiResponse';

const API_BASE_URL = process.env.NEXT_PUBLIC_API_URL || 'http://localhost:5000/api';

export class ApiError extends Error {
  constructor(
    message: string,
    public statusCode?: number,
    public errors?: string[]
  ) {
    super(message);
    this.name = 'ApiError';
  }
}

interface RequestOptions {
  method: 'GET' | 'POST' | 'PUT' | 'DELETE';
  body?: unknown;
  headers?: Record<string, string>;
}

async function request<T>(endpoint: string, options?: RequestOptions): Promise<T> {
  const url = `${API_BASE_URL}${endpoint}`;

  const headers: Record<string, string> = {
    'Content-Type': 'application/json',
    ...options?.headers,
  };

  const config: RequestInit = {
    method: options?.method || 'GET',
    headers,
    credentials: 'include',
  };

  if (options?.body) {
    config.body = JSON.stringify(options.body);
  }

  try {
    const response = await fetch(url, config);

    // Try to parse response as JSON
    let data: ApiResponse<T>;
    try {
      data = await response.json();
    } catch {
      // If JSON parsing fails, throw error with status
      throw new ApiError(
        `Request failed with status ${response.status}`,
        response.status
      );
    }

    // Check if the response was successful
    if (!response.ok || !data.success) {
      throw new ApiError(
        data.message || `Request failed with status ${response.status}`,
        response.status,
        data.errors
      );
    }

    return data.data;
  } catch (error) {
    // Re-throw ApiError as-is
    if (error instanceof ApiError) {
      throw error;
    }

    // Wrap other errors
    if (error instanceof Error) {
      throw new ApiError(error.message);
    }

    throw new ApiError('An unknown error occurred');
  }
}

// GET request
export async function get<T>(endpoint: string): Promise<T> {
  return request<T>(endpoint, { method: 'GET' });
}

// POST request
export async function post<T>(endpoint: string, body?: unknown): Promise<T> {
  return request<T>(endpoint, { method: 'POST', body });
}

// PUT request
export async function put<T>(endpoint: string, body?: unknown): Promise<T> {
  return request<T>(endpoint, { method: 'PUT', body });
}

// DELETE request
export async function del<T>(endpoint: string): Promise<T> {
  return request<T>(endpoint, { method: 'DELETE' });
}

// Export a default object with all methods
const api = {
  get,
  post,
  put,
  delete: del,
  ApiError,
};

export default api;

