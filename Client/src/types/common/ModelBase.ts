import Id from "./Id";

export default interface ModelBase<TId extends Id> {
    id: TId;
}