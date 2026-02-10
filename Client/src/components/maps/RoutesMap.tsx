import { ROUTE_COLORS } from "@/constants/RouteColors";
import BusStopModel from "@/types/BusStopModel";
import RouteModel from "@/types/RouteModel";
import L from "leaflet";
import 'leaflet/dist/leaflet.css';
import { memo, useEffect, useState } from "react";
import { MapContainer, Marker, Popup, TileLayer, Tooltip } from "react-leaflet";
import ArrowLine from "./ArrowLine";
import MapLoader from "./MapLoader";

interface RoutesMapProps {
    routes: RouteModel[];
}

const busStopIcon = L.icon({
    iconUrl: '/icons/bus-stop-32x32.png',
    iconSize: [24, 24],
    iconAnchor: [12, 12],
    popupAnchor: [0, -12]
});

function RoutesMap({
    routes
}: RoutesMapProps) {
    const [mounted, setMounted] = useState(false);
    const [uniqueBusStops, setUniqueBusStops] = useState<BusStopModel[]>([]);
    const defaultCenter: [number, number] = [43.271098, 26.935763]; // Default center: Shumen, Bulgaria

    useEffect(() => {
        setMounted(true);
    }, []);

    useEffect(() => {
        setUniqueBusStops(
            routes
                .map(x => x.stops)
                .flat()
                .map(x => { return { id: x.busStopId, name: x.busStopName, location: x.busStopLocation } as BusStopModel })
                .filter((x, i, a) => x.id !== null && a.findIndex(y => y.id === x.id) === i));
    }, [routes]);

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
                        <TileLayer
                            url="https://{s}.tile.openstreetmap.org/{z}/{x}/{y}.png"
                            attribution='&copy; <a href="https://www.openstreetmap.org/copyright">OpenStreetMap</a> contributors'
                        />
                        {uniqueBusStops.map((busStop) => {
                            return (
                                <Marker
                                    key={busStop.id}
                                    data={stop}
                                    position={[busStop.location.latitude, busStop.location.longitude]}
                                    icon={busStopIcon}
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
                                    </Popup>
                                </Marker>
                            );
                        })}
                        {routes.map((route, index) => {
                            return (
                                <ArrowLine
                                    key={route.id}
                                    positions={route.stops.map(x => x.busStopId ? [x.busStopLocation!.latitude, x.busStopLocation!.longitude] : [x.location!.latitude, x.location!.longitude])}
                                    pathOptions={{ color: ROUTE_COLORS[index % ROUTE_COLORS.length], weight: 2 }}
                                />
                            );
                        })}
                    </MapContainer>
                </div>
            )}
        </>
    );
}

export default memo(RoutesMap);