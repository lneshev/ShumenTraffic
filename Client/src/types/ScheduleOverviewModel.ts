
import ModelBase from "./common/ModelBase";

export default interface ScheduleOverviewModel extends ModelBase<number> {
    dayType: number;
    dayTypeText: string;
    startDate: string;
    endDate?: string;
    isActive: boolean;
    priority: number;
    priorityText: string;
    busLineId: number;
    busLineNumber: string;
}