import ModelBase from "./common/ModelBase";

export default interface BusStop extends ModelBase<number> {
    name: string;
    latitude: number;
    longitude: number;
    zoneId?: number;
    isActive: boolean;
}