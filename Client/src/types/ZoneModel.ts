import ModelBase from "./common/ModelBase";

export default interface ZoneModel extends ModelBase<number> {
    name: string;
    description?: string;
    isActive: boolean;
}