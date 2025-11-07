export default interface ApiResponse<T = void> {
    success: boolean;
    message: string;
    data: T;
    errors: string[];
    timestamp: string;
}