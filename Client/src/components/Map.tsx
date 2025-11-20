'use client';

import MapMode from '@/enums/MapMode';
import BusStopModel from '@/types/BusStopModel';
import L, { LeafletEventHandlerFnMap, PopupEvent } from 'leaflet';
import 'leaflet/dist/leaflet.css';
import { useEffect, useRef, useState } from 'react';
import { MapContainer, Marker, Popup, TileLayer, Tooltip, useMap, useMapEvents } from 'react-leaflet';
import MapLoader from './MapLoader';

// Extend Leaflet Marker to support custom data property
declare module 'leaflet' {
  interface MarkerOptions {
    data?: any;
  }
}

interface MapProps {
  busStops: BusStopModel[];
  selectedStopId?: number;
  mode?: MapMode;
  eventHandlers?: LeafletEventHandlerFnMap;
  onBusStopDragEnd?: (stop: BusStopModel, newLat: number, newLng: number) => void;
  onBusStopNameChange?: (stop: BusStopModel, newName: string) => void;
  onBusStopSave?: (stop: BusStopModel, e: Event) => void;
  onBusStopDelete?: (stop: BusStopModel, e: Event) => void;
  onBusStopCancel?: (stop: BusStopModel) => void;
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

// Helper component to expose map instance
function MapInstanceProvider({ onMapReady }: { onMapReady: (map: L.Map) => void }) {
  const map = useMap();
  const mapEvents = useMapEvents({
    contextmenu: (e) => {
      console.log("Context menu", e);
    }
  });
  useEffect(() => {
    onMapReady(map);
  }, [map, onMapReady]);
  return null;
}

export function Map({
  busStops,
  selectedStopId,
  mode = MapMode.View,
  onBusStopDragEnd,
  onBusStopNameChange,
  onBusStopSave,
  onBusStopDelete,
  onBusStopCancel
}: MapProps) {
  const [mounted, setMounted] = useState(false);
  const mapRef = useRef<L.Map | null>(null);

  useEffect(() => {
    setMounted(true);
  }, []);

  // Default center: Shumen, Bulgaria
  const defaultCenter: [number, number] = [43.27109895944945, 26.935763019161463];

  const markerEventHandlers = {
    dragend: (e: PopupEvent) => {
      const marker = e.target;
      const position = marker.getLatLng();
      const stopData = marker.options.data as BusStopModel;
      if (stopData) {
        onBusStopDragEnd?.(stopData, position.lat, position.lng);
      }
    },
    popupclose: (e: PopupEvent) => {
      const stop = e.target.options.data as BusStopModel;
      onBusStopCancel?.(stop);
    }
  };

  const handleButtonSaveClick = async (stop: BusStopModel) => {
    const e = new Event("handleButtonSaveClick", { cancelable: true });
    await onBusStopSave?.(stop, e);
    if (!e.defaultPrevented) {
      mapRef.current?.closePopup();
    }
  }

  const handleButtonDeleteClick = async (stop: BusStopModel) => {
    const e = new Event("handleButtonDeleteClick", { cancelable: true });
    await onBusStopDelete?.(stop, e);
    if (!e.defaultPrevented) {
      mapRef.current?.closePopup();
    }
  }

  const handleButtonCancelClick = (stop: BusStopModel) => {
    mapRef.current?.closePopup();
  }

  return (
    <>
      {!mounted ? (
        <MapLoader />
      ) : (
        <MapContainer
          center={defaultCenter}
          zoom={14}
          style={{ width: '100%', height: '100%' }}
          className="rounded-lg"
        >
          <MapInstanceProvider onMapReady={(map) => { mapRef.current = map; }} />
          <TileLayer
            url="https://{s}.tile.openstreetmap.org/{z}/{x}/{y}.png"
            attribution='&copy; <a href="https://www.openstreetmap.org/copyright">OpenStreetMap</a> contributors'
          />
          {busStops.map((stop) => {
            return (
              <Marker
                key={stop.id}
                data={stop}
                position={[stop.location.latitude, stop.location.longitude]}
                icon={selectedStopId === stop.id ? selectedIcon : defaultIcon}
                draggable={mode === MapMode.Edit}
                eventHandlers={markerEventHandlers}
              >
                <Tooltip offset={[10, 0]} opacity={1} direction={'right'}>{stop.name}</Tooltip>
                <Popup>
                  <form onSubmit={(e) => { e.preventDefault(); handleButtonSaveClick(stop); }} className="text-sm">
                    <p className="font-semibold text-gray-900">
                      {mode === MapMode.View && (<>{stop.name}</>)}
                      {mode === MapMode.Edit && (
                        <input
                          type="text"
                          name="busStopName"
                          value={stop.name}
                          onChange={(e) => onBusStopNameChange?.(stop, e.target.value)}
                          className="px-4 py-2 border border-gray-300 dark:border-slate-600 rounded-lg bg-white dark:bg-slate-800 text-gray-900 dark:text-white"
                          required
                          maxLength={255}
                        />
                      )}
                    </p>
                    <p className="text-gray-600 text-xs">
                      {stop.location.latitude}, {stop.location.longitude}
                    </p>
                    {mode == MapMode.Edit && (
                      <div>
                        <button
                          type="submit"
                          className="mb-6 px-4 py-2 bg-blue-600 hover:bg-blue-700 text-white font-semibold rounded-lg transition-colors"
                        >
                          Save
                        </button>
                        <button
                          type="button"
                          onClick={() => handleButtonDeleteClick(stop)}
                          className="mb-6 px-4 py-2 bg-red-600 hover:bg-red-700 text-white font-semibold rounded-lg transition-colors"
                        >
                          Delete
                        </button>
                        <button
                          type="button"
                          onClick={() => handleButtonCancelClick(stop)}
                          className="mb-6 px-4 py-2 bg-blue-600 hover:bg-blue-700 text-white font-semibold rounded-lg transition-colors"
                        >
                          Cancel
                        </button>
                      </div>
                    )}
                  </form>
                </Popup>
              </Marker>
            );
          })}
        </MapContainer >
      )}
    </>
  );
}