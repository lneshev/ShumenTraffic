'use client';

import BusStopModel from '@/types/BusStopModel';
import L from 'leaflet';
import 'leaflet/dist/leaflet.css';
import { useEffect, useState } from 'react';
import { MapContainer, Marker, Popup, TileLayer } from 'react-leaflet';
import MapLoader from './MapLoader';

interface MapProps {
  busStops: BusStopModel[];
  selectedStopId?: number;
}

// Fix for default marker icons in Next.js
const defaultIcon = L.icon({
  iconUrl: 'https://cdnjs.cloudflare.com/ajax/libs/leaflet/1.9.4/images/marker-icon.png',
  shadowUrl: 'https://cdnjs.cloudflare.com/ajax/libs/leaflet/1.9.4/images/marker-shadow.png',
  iconSize: [25, 41],
  iconAnchor: [12, 41],
  popupAnchor: [1, -34],
  shadowSize: [41, 41],
});

const selectedIcon = L.icon({
  iconUrl: 'https://cdnjs.cloudflare.com/ajax/libs/leaflet/1.9.4/images/marker-icon-2x.png',
  shadowUrl: 'https://cdnjs.cloudflare.com/ajax/libs/leaflet/1.9.4/images/marker-shadow.png',
  iconSize: [32, 51],
  iconAnchor: [16, 51],
  popupAnchor: [1, -34],
  shadowSize: [41, 41],
});

L.Marker.prototype.options.icon = defaultIcon;

export function Map({ busStops, selectedStopId }: MapProps) {
  const [mounted, setMounted] = useState(false);

  useEffect(() => {
    setMounted(true);
  }, []);

  if (!mounted) {
    return (
      <MapLoader />
    );
  }

  // Default center: Shumen, Bulgaria
  const defaultCenter: [number, number] = [43.27109895944945, 26.935763019161463];

  return (
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
      {busStops.map((stop) => (
        <Marker
          key={stop.id}
          position={[stop.location.latitude, stop.location.longitude]}
          icon={selectedStopId === stop.id ? selectedIcon : defaultIcon}
        >
          <Popup>
            <div className="text-sm">
              <p className="font-semibold text-gray-900">{stop.name}</p>
              <p className="text-gray-600 text-xs">
                {stop.location.latitude}, {stop.location.longitude}
              </p>
            </div>
          </Popup>
        </Marker>
      ))}
    </MapContainer>
  );
}