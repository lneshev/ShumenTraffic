export default function MapLoader() {
    return (
        <div className="w-full h-full flex items-center justify-center">
            <div className="text-center">
                <div className="inline-block animate-spin rounded-full h-12 w-12 border-b-2 border-blue-600 mb-4"></div>
                <p className="text-gray-600 dark:text-gray-400">Loading map...</p>
            </div>
        </div>
    );
}