import { Point } from "geojson";

export class GeoPoint implements Point {
    type: "Point" = "Point";
    coordinates: [number, number];

    constructor(lat: number, lon: number) {
        this.coordinates = [lon, lat];
    }

    get latitude() { return this.coordinates[1]; }
    set latitude(v) { this.coordinates[1] = v; }

    get longitude() { return this.coordinates[0]; }
    set longitude(v) { this.coordinates[0] = v; }
}

function enhancePoint(p: Point): GeoPoint {
    return new GeoPoint(p.coordinates[1], p.coordinates[0]);
}

export function enhanceGeoJSON<T extends object>(obj: T): T {
    if (Array.isArray(obj)) {
        return obj.map((item: any) => {
            if (Array.isArray(item)) {
                // recurse into nested array
                return enhanceGeoJSON(item);
            }

            if (item && typeof item === "object") {
                // recurse into objects
                return enhanceGeoJSON(item);
            }

            // primitive → copy as-is
            return item;
        }) as any;
    }

    const copy: any = { ...obj };
    for (const key in copy) {
        const value = copy[key];
        if (value && typeof value === "object") {
            if (value.type === "Point" && Array.isArray(value.coordinates)) {
                copy[key] = enhancePoint(value);
            } else {
                copy[key] = enhanceGeoJSON(value);
            }
        }
    }
    return copy;
}