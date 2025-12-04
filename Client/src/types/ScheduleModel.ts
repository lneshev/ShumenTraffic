
import ModelBase from "./common/ModelBase";
import ScheduleCourseModel from "./ScheduleCourseModel";

export default interface ScheduleModel extends ModelBase<number> {
    dayType: number;
    dayTypeText: string;
    startDate: string;
    endDate?: string;
    isActive: boolean;
    priority: number;
    busLineId: number;
    busLineNumber: string;
    direction: number;
    scheduleCourses: ScheduleCourseModel[];
}