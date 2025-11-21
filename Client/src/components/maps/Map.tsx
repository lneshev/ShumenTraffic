import MapMode from '@/enums/MapMode';
import BusStopModel from '@/types/BusStopModel';
import L, { LeafletEventHandlerFnMap, PopupEvent } from 'leaflet';
import 'leaflet/dist/leaflet.css';
import { memo, useEffect, useRef, useState } from 'react';
import { MapContainer, TileLayer, Tooltip, useMap, useMapEvents } from 'react-leaflet';
import { BusStopMarker } from './BusStopMarker';
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
  newBusStop?: BusStopModel;
  mode?: MapMode;
  eventHandlers?: LeafletEventHandlerFnMap;
  onBusStopDragEnd?: (stop: BusStopModel, newLat: number, newLng: number) => void;
  onBusStopNameChange?: (stop: BusStopModel, newName: string) => void;
  onBusStopZoneIdChange?: (stop: BusStopModel, newZoneId: number) => void;
  onBusStopSave?: (stop: BusStopModel, e: Event) => void;
  onBusStopDelete?: (stop: BusStopModel, e: Event) => void;
  onBusStopCancel?: (stop: BusStopModel) => void;
  onMapRightClick?: (lat: number, lng: number) => void;
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

const newStopIcon = L.icon({
  iconUrl: 'https://raw.githubusercontent.com/pointhi/leaflet-color-markers/master/img/marker-icon-2x-green.png',
  shadowUrl: 'https://cdnjs.cloudflare.com/ajax/libs/leaflet/1.9.4/images/marker-shadow.png',
  iconSize: [25, 41],
  iconAnchor: [12, 41],
  popupAnchor: [1, -34],
  shadowSize: [41, 41],
});

L.Marker.prototype.options.icon = defaultIcon;

// Helper component to expose map instance
function MapInstanceProvider({
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

function Map({
  busStops,
  selectedStopId,
  newBusStop,
  mode = MapMode.View,
  onBusStopDragEnd,
  onBusStopNameChange,
  onBusStopZoneIdChange,
  onBusStopSave,
  onBusStopDelete,
  onBusStopCancel,
  onMapRightClick
}: MapProps) {
  const [mounted, setMounted] = useState(false);
  const mapRef = useRef<L.Map>(null);
  const newMarkerPopupRef = useRef<L.Marker>(null);
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

  const markerEventHandlers: LeafletEventHandlerFnMap = {
    dragend: (e: PopupEvent) => {
      const marker = e.target;
      const position = marker.getLatLng();
      const stopData = marker.options.data as BusStopModel;
      if (stopData) {
        onBusStopDragEnd?.(stopData, position.lat, position.lng);
      }
    },
    popupclose: (e: PopupEvent) => {
      setNewMarkerData(null);
      const stop = e.target.options.data as BusStopModel;
      onBusStopCancel?.(stop);
    }
  };

  const handleContextMenu = (lat: number, lng: number) => {
    if (mode === MapMode.Edit) {
      setNewMarkerData({ lat, lng });
      setShowContextMenu(true);
      mapRef.current?.closePopup();
    }
  };

  const handleAddBusStop = () => {
    if (newMarkerData) {
      onMapRightClick?.(newMarkerData.lat, newMarkerData.lng);
      setShowContextMenu(false);
      if (newMarkerPopupRef.current) {
        newMarkerPopupRef.current.openPopup();
      }
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
        <div style={{ position: 'relative', width: '100%', height: '100%' }}>
          <MapContainer
            center={defaultCenter}
            zoom={14}
            style={{ width: '100%', height: '100%' }}
            className="rounded-lg"
          >
            <MapInstanceProvider
              onMapReady={(map) => { mapRef.current = map; }}
              onContextMenu={handleContextMenu}
            />
            <TileLayer
              url="https://{s}.tile.openstreetmap.org/{z}/{x}/{y}.png"
              attribution='&copy; <a href="https://www.openstreetmap.org/copyright">OpenStreetMap</a> contributors'
            />
            {busStops.map((stop) => {
              return (
                <BusStopMarker
                  key={stop.id}
                  mode={mode}
                  busStop={stop}
                  position={[stop.location.latitude, stop.location.longitude]}
                  icon={selectedStopId === stop.id ? selectedIcon : defaultIcon}
                  draggable={mode === MapMode.Edit}
                  tooltip={<Tooltip offset={[10, 0]} opacity={1} direction={'right'}>{stop.name}</Tooltip>}
                  eventHandlers={markerEventHandlers}
                  onBusStopNameChange={onBusStopNameChange}
                  onBusStopZoneIdChange={onBusStopZoneIdChange}
                  onButtonSaveClick={handleButtonSaveClick}
                  onButtonDeleteClick={handleButtonDeleteClick}
                  onButtonCancelClick={handleButtonCancelClick}
                />
              );
            })}
            {newMarkerData && newBusStop && (
              <BusStopMarker
                mode={mode}
                busStop={newBusStop}
                position={[newMarkerData.lat, newMarkerData.lng]}
                icon={newStopIcon}
                draggable={false}
                eventHandlers={markerEventHandlers}
                onBusStopNameChange={onBusStopNameChange}
                onBusStopZoneIdChange={onBusStopZoneIdChange}
                onButtonSaveClick={handleButtonSaveClick}
                onButtonDeleteClick={handleButtonDeleteClick}
                onButtonCancelClick={handleButtonCancelClick}
                ref={newMarkerPopupRef}
              />
            )}
          </MapContainer>
          {showContextMenu && newMarkerData && (
            <div
              style={{
                position: 'absolute',
                top: mapRef.current?.latLngToContainerPoint([newMarkerData.lat, newMarkerData.lng]).y,
                left: mapRef.current?.latLngToContainerPoint([newMarkerData.lat, newMarkerData.lng]).x,
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
                onClick={handleAddBusStop}
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
                Add Bus Stop
              </button>
            </div>
          )}
        </div>
      )}
    </>
  );
};

export default memo(Map);