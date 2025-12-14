import BusLineLightModel from "./BusLineLightModel";
import ModelBase from "./common/ModelBase";

export default interface ZoneWithBusLinesModel extends ModelBase<number> {
    name: string;
    busLines: BusLineLightModel[];
}