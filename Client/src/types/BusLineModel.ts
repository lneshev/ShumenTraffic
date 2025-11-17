import ModelBase from "./common/ModelBase";

export default interface BusLineModel extends ModelBase<number> {
    lineNumber: string;
    description?: string;
    transportationCompanyIds: number[];
    transportationCompanyNames: string[];
    isActive: boolean;
}