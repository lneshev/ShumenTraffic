import BusStopModel from "./BusStopModel";

export default interface TimetableRowModel {
    busStop: BusStopModel;
    timesByVariant: { [key: string]: string | null };
}