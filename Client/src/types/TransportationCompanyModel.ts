import ModelBase from "./common/ModelBase";

export default interface TransportationCompanyModel extends ModelBase<number> {
    name: string;
    description?: string;
    isActive: boolean;
}