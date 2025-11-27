import { GeoPoint } from "./common/GeoJSON";
import ModelBase from "./common/ModelBase";

export default interface RouteStopModel extends ModelBase<number> {
    busStopId?: number;
    busStopName?: string;
    busStopLocation?: GeoPoint;
    location?: GeoPoint;
    stopOrder: number;
    estimatedMinutesFromStart?: number;
}