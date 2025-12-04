
import ModelBase from "./common/ModelBase";
import ScheduleCourseModel from "./ScheduleCourseModel";

export default interface ScheduleOverviewModel extends ModelBase<number> {
    dayType: number;
    dayTypeText: string;
    startDate: string;
    endDate?: string;
    isActive: boolean;
    priority: number;
    busLineId: number;
    busLineNumber: string;
    scheduleCourses: ScheduleCourseModel[];
}