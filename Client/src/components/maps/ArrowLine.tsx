import L from "leaflet";
import "leaflet-polylinedecorator";
import { useEffect } from "react";
import { useMap } from "react-leaflet";

export default function ArrowLine({ positions, pathOptions }: { positions: [number, number][]; pathOptions: L.PolylineOptions }) {
    const map = useMap();

    useEffect(() => {
        if (!map) return;

        // Main polyline
        const polyline = L.polyline(positions, pathOptions).addTo(map);

        // Dynamic repeat percentage: clamp between 5% and 50%
        const pointCount = positions.length;
        const raw = 100 / pointCount;
        const repeatPercent = Math.min(Math.max(raw, 5), 50) + "%";

        // Arrowheads decorator
        const decorator = L.polylineDecorator(polyline, {
            patterns: [
                {
                    offset: "0%",      // start position along the line
                    repeat: repeatPercent,     // repeat arrows
                    symbol: L.Symbol.arrowHead({
                        pixelSize: 10,
                        polygon: false,
                        pathOptions: { stroke: true, color: pathOptions.color, weight: 2 }
                    })
                }
            ]
        }).addTo(map);

        // Cleanup on unmount
        return () => {
            map.removeLayer(polyline);
            map.removeLayer(decorator);
        };
    }, [map, positions, pathOptions]);

    return null; // nothing visually rendered by React component itself
}