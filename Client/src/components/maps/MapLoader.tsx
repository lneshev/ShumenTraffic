export default function MapLoader() {
    return (
        <div className="w-full h-full flex items-center justify-center">
            <div className="text-center">
                <div className="inline-block animate-spin rounded-full h-12 w-12 border-b-2 border-primary mb-4"></div>
                <p className="text-text-muted">Loading map...</p>
            </div>
        </div>
    );
}