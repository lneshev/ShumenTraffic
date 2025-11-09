export default interface BusLineModel {
    id: number;
    lineNumber: string;
    description?: string;
    transportationCompanyIds: number[];
    transportationCompanyNames: string[];
    isActive: boolean;
}