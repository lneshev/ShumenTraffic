import ModelBase from "./common/ModelBase";

export default interface RouteModel extends ModelBase<number> {
    name: string;
    direction: number;
    directionText: string;
    isActive: boolean;
    busLineId: number;
    busLineNumber: string;
}