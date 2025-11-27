import BusStopModel from "@/types/BusStopModel";
import RouteStopModel from "@/types/RouteStopModel";
import L, { LeafletEventHandlerFnMap, PopupEvent } from "leaflet";
import 'leaflet/dist/leaflet.css';
import { memo, useEffect, useRef, useState } from "react";
import { MapContainer, Marker, Popup, TileLayer, Tooltip, useMap, useMapEvents } from "react-leaflet";
import ArrowLine from "./ArrowLine";
import MapLoader from "./MapLoader";

interface RouteMapProps {
    busStops: BusStopModel[];
    routeStops: RouteStopModel[];
    newRouteStop?: RouteStopModel;
    onRouteStopAdd?: (lat: number, lng: number) => void;
    onRouteStopRemove?: (routeStop: RouteStopModel) => void;
    onRouteStopDragEnd?: (routeStop: RouteStopModel, newLat: number, newLng: number) => void;
    onBusStopAddToRoute?: (busStop: BusStopModel, e: Event) => void;
}

const busStopIcon = L.icon({
    iconUrl: '/icons/bus-stop-32x32.png',
    iconSize: [24, 24],
    iconAnchor: [12, 12],
    popupAnchor: [0, -12]
});

const selectedBusStopIcon = L.icon({
    iconUrl: '/icons/bus-stop-32x32.png',
    iconSize: [32, 32],
    iconAnchor: [16, 16],
    popupAnchor: [0, -16]
});

const routeStopIcon = L.icon({
    iconUrl: '/icons/route-stop-32x32.png',
    iconSize: [24, 24],
    iconAnchor: [12, 12],
    popupAnchor: [0, -12]
});

const addIcon = L.icon({
    iconUrl: '/icons/add-32x32.png',
    iconSize: [24, 24],
    iconAnchor: [12, 12],
    popupAnchor: [0, -12]
});

// Helper component to expose map instance
function RouteStopMapInstanceProvider({
    onMapReady,
    onContextMenu
}: {
    onMapReady: (map: L.Map) => void;
    onContextMenu?: (lat: number, lng: number) => void;
}) {
    const map = useMap();

    useMapEvents({
        contextmenu: (e) => {
            onContextMenu?.(parseFloat(e.latlng.lat.toFixed(6)), parseFloat(e.latlng.lng.toFixed(6)));
        }
    });

    useEffect(() => {
        onMapReady(map);
    }, [map, onMapReady]);

    return null;
}

function RouteMap({
    busStops,
    routeStops,
    newRouteStop,
    onRouteStopAdd,
    onRouteStopRemove,
    onRouteStopDragEnd,
    onBusStopAddToRoute
}: RouteMapProps) {
    const [mounted, setMounted] = useState(false);
    const mapRef = useRef<L.Map>(null);
    const [newMarkerData, setNewMarkerData] = useState<{ lat: number; lng: number } | null>(null);
    const [showContextMenu, setShowContextMenu] = useState(false);
    const defaultCenter: [number, number] = [43.271098, 26.935763]; // Default center: Shumen, Bulgaria

    useEffect(() => {
        setMounted(true);
    }, []);

    // Close context menu when clicking elsewhere
    useEffect(() => {
        const handleClick = () => {
            setNewMarkerData(null);
            setShowContextMenu(false);
        };
        if (showContextMenu) {
            document.addEventListener('click', handleClick);
            return () => document.removeEventListener('click', handleClick);
        }
    }, [showContextMenu]);

    const routeStopMarkerEventHandlers: LeafletEventHandlerFnMap = {
        dragend: (e: PopupEvent) => {
            const marker = e.target;
            const position = marker.getLatLng();
            const routeStop = marker.options.data as RouteStopModel;
            if (routeStop) {
                onRouteStopDragEnd?.(routeStop, parseFloat(position.lat.toFixed(6)), parseFloat(position.lng.toFixed(6)));
            }
        }
    };

    const handleContextMenu = (lat: number, lng: number) => {
        setNewMarkerData({ lat, lng });
        setShowContextMenu(true);
        mapRef.current?.closePopup();
    };

    const handleAddRouteStop = () => {
        if (newMarkerData) {
            onRouteStopAdd?.(newMarkerData.lat, newMarkerData.lng);
            setShowContextMenu(false);
            setNewMarkerData(null);
        }
    };

    const handleButtonBusStopAddToRouteClick = async (stop: BusStopModel) => {
        const e = new Event("handleButtonBusStopAddToRouteClick", { cancelable: true });
        await onBusStopAddToRoute?.(stop, e);
        if (!e.defaultPrevented) {
            mapRef.current?.closePopup();
        }
    };

    return (
        <>
            {!mounted ? (
                <MapLoader />
            ) : (
                <div style={{ position: 'relative', width: '100%', height: '100%' }}>
                    <MapContainer
                        center={defaultCenter}
                        zoom={14}
                        style={{ width: '100%', height: '100%' }}
                        className="rounded-lg"
                    >
                        <RouteStopMapInstanceProvider
                            onMapReady={(map) => { mapRef.current = map; }}
                            onContextMenu={handleContextMenu}
                        />
                        <TileLayer
                            url="https://{s}.tile.openstreetmap.org/{z}/{x}/{y}.png"
                            attribution='&copy; <a href="https://www.openstreetmap.org/copyright">OpenStreetMap</a> contributors'
                        />
                        {busStops.map((busStop) => {
                            return (
                                <Marker
                                    key={busStop.id}
                                    data={stop}
                                    position={[busStop.location.latitude, busStop.location.longitude]}
                                    icon={routeStops.some(rs => rs.busStopId === busStop.id) ? selectedBusStopIcon : busStopIcon}
                                    draggable={false}
                                >
                                    <Tooltip offset={[10, 0]} opacity={1} direction={'right'}>{busStop.name}</Tooltip>
                                    <Popup>
                                        <div className="font-semibold text-gray-900">
                                            {busStop.name}<br />
                                            {busStop.zoneName}
                                        </div>
                                        <p className="text-gray-600 text-xs">
                                            {busStop.location.latitude}, {busStop.location.longitude}
                                        </p>
                                        <button
                                            type="button"
                                            onClick={() => handleButtonBusStopAddToRouteClick?.(busStop)}
                                            className="px-4 py-2 bg-blue-600 hover:bg-blue-700 text-white font-semibold rounded-lg transition-colors"
                                        >
                                            Add to Route
                                        </button>
                                    </Popup>
                                </Marker>
                            );
                        })}
                        {routeStops.filter(x => !!x.location).map((routeStop) => {
                            return (
                                <Marker
                                    key={routeStop.id}
                                    data={routeStop}
                                    position={[routeStop.location!.latitude, routeStop.location!.longitude]}
                                    icon={routeStopIcon}
                                    draggable={true}
                                    eventHandlers={routeStopMarkerEventHandlers}
                                >
                                    <Popup>
                                        <p className="text-gray-600 text-xs">
                                            {routeStop.location!.latitude}, {routeStop.location!.longitude}
                                        </p>
                                        <button
                                            type="button"
                                            onClick={() => onRouteStopRemove?.(routeStop)}
                                            className="px-4 py-2 bg-red-600 hover:bg-red-700 text-white font-semibold rounded-lg transition-colors"
                                        >
                                            Remove
                                        </button>
                                    </Popup>
                                </Marker>
                            );
                        })}
                        {newMarkerData && newRouteStop && (
                            <Marker
                                data={newRouteStop}
                                position={[newMarkerData.lat, newMarkerData.lng]}
                                icon={addIcon}
                                draggable={false}
                            />
                        )}
                        <ArrowLine
                            positions={routeStops.map(x => x.busStopId ? [x.busStopLocation!.latitude, x.busStopLocation!.longitude] : [x.location!.latitude, x.location!.longitude])}
                            pathOptions={{ color: 'red', weight: 2 }}
                        />
                    </MapContainer>
                    {showContextMenu && newMarkerData && mapRef.current && (
                        <div
                            style={{
                                position: 'absolute',
                                top: mapRef.current.latLngToContainerPoint([newMarkerData.lat, newMarkerData.lng]).y + 10,
                                left: mapRef.current.latLngToContainerPoint([newMarkerData.lat, newMarkerData.lng]).x + 10,
                                zIndex: 1000,
                                backgroundColor: 'white',
                                border: '1px solid #ccc',
                                borderRadius: '4px',
                                boxShadow: '0 2px 8px rgba(0,0,0,0.15)',
                                padding: '8px 0',
                                minWidth: '150px'
                            }}
                            onClick={(e) => e.stopPropagation()}
                        >
                            <button
                                onClick={handleAddRouteStop}
                                style={{
                                    width: '100%',
                                    padding: '8px 16px',
                                    border: 'none',
                                    background: 'none',
                                    textAlign: 'left',
                                    cursor: 'pointer',
                                    fontSize: '14px'
                                }}
                                onMouseEnter={(e) => e.currentTarget.style.backgroundColor = '#f0f0f0'}
                                onMouseLeave={(e) => e.currentTarget.style.backgroundColor = 'transparent'}
                            >
                                Add Route Stop
                            </button>
                        </div>
                    )}
                </div>
            )}
        </>
    );
}

export default memo(RouteMap);