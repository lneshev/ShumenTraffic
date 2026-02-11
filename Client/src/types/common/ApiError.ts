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
