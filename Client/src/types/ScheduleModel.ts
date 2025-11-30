
import ModelBase from "./common/ModelBase";
import ScheduleCourseModel from "./ScheduleCourseModel";

export default interface ScheduleOverviewModel extends ModelBase<number> {
    dayType: number;
    dayTypeText: string;
    startDate: string;
    endDate?: string;
    isActive: boolean;
    busLineId: number;
    busLineNumber: string;
    courses: ScheduleCourseModel[];
}