import 'leaflet';

// Extend Leaflet Marker to support custom data property
declare module 'leaflet' {
    interface MarkerOptions {
        data?: any;
    }
}