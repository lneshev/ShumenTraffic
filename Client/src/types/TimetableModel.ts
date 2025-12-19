import ScheduleModel from "./ScheduleModel";
import TimetableRowModel from "./TimetableRowModel";

export default interface TimetableModel {
    schedule: ScheduleModel;
    timetableRows: TimetableRowModel[];
}