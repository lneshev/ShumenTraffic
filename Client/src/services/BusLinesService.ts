import {
    authorisedDeleteRequest,
    authorisedPostRequest,
    getQueryString,
    getRequest
} from "@/helpers/Request";
import env from "@/services/EnvService";
import BusLineModel from "@/types/BusLineModel";
import PageResult from "@/types/common/PageResult";
import Sort from "@/types/common/Sort";

export default {
    async read(filter: Record<string, any> = {}, sorts: Sort[] = [], pageNumber?: number, pageSize?: number): Promise<PageResult<BusLineModel>> {
        return await getRequest(env.getPublicWebApiBaseUrl() + `/api/bus-lines${getQueryString(filter, sorts, pageNumber, pageSize)}`, result => {
            return result;
        });
    },
    async count(): Promise<number> {
        return await getRequest(env.getPublicWebApiBaseUrl() + `/api/bus-lines/count`, result => {
            return result;
        });
    },
    async create(model: BusLineModel): Promise<BusLineModel> {
        return await authorisedPostRequest(env.getPublicWebApiBaseUrl() + `/api/bus-lines`, model, result => {
            return result;
        });
    },
    async delete(id: number): Promise<BusLineModel> {
        return await authorisedDeleteRequest(env.getPublicWebApiBaseUrl() + `/api/bus-lines/${id}`, result => {
            return result;
        });
    }
}