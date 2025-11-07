export default interface PageResult<T = void> {
    items: T[];
    totalCount?: number;
    totalCountGet: boolean;
}