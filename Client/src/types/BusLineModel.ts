import ModelBase from "./common/ModelBase";
import TransportationCompanyLightModel from "./TransportationCompanyLightModel";

export default interface BusLineModel extends ModelBase<number> {
    lineNumber: string;
    description?: string;
    isActive: boolean;
    transportationCompanies: TransportationCompanyLightModel[];
}