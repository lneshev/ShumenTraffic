import { GeoPoint } from "./common/GeoJSON";
import ModelBase from "./common/ModelBase";

export default interface BusStopModel extends ModelBase<number> {
    name: string;
    zoneId: number;
    zoneName: string;
    location: GeoPoint;
    isActive: boolean;
}