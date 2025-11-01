using System;
using System.Collections.Generic;

namespace ShumenTraffic.WebAPI.Common
{
    /// <summary>
    /// Standard API response wrapper for all endpoints.
    /// </summary>
    public class ApiResponse<T>
    {
        /// <summary>
        /// Indicates if the request was successful.
        /// </summary>
        public bool Success { get; set; }

        /// <summary>
        /// Response message.
        /// </summary>
        public string Message { get; set; }

        /// <summary>
        /// Response data.
        /// </summary>
        public T Data { get; set; }

        /// <summary>
        /// List of errors if the request failed.
        /// </summary>
        public List<string> Errors { get; set; } = new List<string>();

        /// <summary>
        /// Timestamp of the response.
        /// </summary>
        public DateTimeOffset Timestamp { get; set; } = DateTimeOffset.UtcNow;

        /// <summary>
        /// Creates a successful response.
        /// </summary>
        public static ApiResponse<T> SuccessResponse(T data, string message = "Request successful")
        {
            return new ApiResponse<T>
            {
                Success = true,
                Message = message,
                Data = data,
                Timestamp = DateTimeOffset.UtcNow
            };
        }

        /// <summary>
        /// Creates a failed response.
        /// </summary>
        public static ApiResponse<T> ErrorResponse(string message, List<string> errors = null)
        {
            return new ApiResponse<T>
            {
                Success = false,
                Message = message,
                Data = default,
                Errors = errors ?? new List<string>(),
                Timestamp = DateTimeOffset.UtcNow
            };
        }

        /// <summary>
        /// Creates a failed response with a single error.
        /// </summary>
        public static ApiResponse<T> ErrorResponse(string message, string error)
        {
            return new ApiResponse<T>
            {
                Success = false,
                Message = message,
                Data = default,
                Errors = new List<string> { error },
                Timestamp = DateTimeOffset.UtcNow
            };
        }
    }

    /// <summary>
    /// Non-generic API response wrapper.
    /// </summary>
    public class ApiResponse
    {
        /// <summary>
        /// Indicates if the request was successful.
        /// </summary>
        public bool Success { get; set; }

        /// <summary>
        /// Response message.
        /// </summary>
        public string Message { get; set; }

        /// <summary>
        /// List of errors if the request failed.
        /// </summary>
        public List<string> Errors { get; set; } = new List<string>();

        /// <summary>
        /// Timestamp of the response.
        /// </summary>
        public DateTimeOffset Timestamp { get; set; } = DateTimeOffset.UtcNow;

        /// <summary>
        /// Creates a successful response.
        /// </summary>
        public static ApiResponse SuccessResponse(string message = "Request successful")
        {
            return new ApiResponse
            {
                Success = true,
                Message = message,
                Timestamp = DateTimeOffset.UtcNow
            };
        }

        /// <summary>
        /// Creates a failed response.
        /// </summary>
        public static ApiResponse ErrorResponse(string message, List<string> errors = null)
        {
            return new ApiResponse
            {
                Success = false,
                Message = message,
                Errors = errors ?? new List<string>(),
                Timestamp = DateTimeOffset.UtcNow
            };
        }

        /// <summary>
        /// Creates a failed response with a single error.
        /// </summary>
        public static ApiResponse ErrorResponse(string message, string error)
        {
            return new ApiResponse
            {
                Success = false,
                Message = message,
                Errors = new List<string> { error },
                Timestamp = DateTimeOffset.UtcNow
            };
        }
    }
}

