import ModelBase from "./common/ModelBase";

export default interface ScheduleCourseModel extends ModelBase<number> {
    departureTime: string;
    routeId: number;
}