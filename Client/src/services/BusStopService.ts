import {
    authorisedDeleteRequest,
    authorisedPostRequest,
    authorisedPutRequest,
    getQueryString,
    getRequest
} from "@/helpers/Request";
import env from "@/services/EnvService";
import BusStopModel from "@/types/BusStopModel";
import PageResult from "@/types/common/PageResult";
import Sort from "@/types/common/Sort";

export default {
    async read(filter: Record<string, any> = {}, sorts: Sort[] = [], pageNumber?: number, pageSize?: number): Promise<PageResult<BusStopModel>> {
        return await getRequest(env.getPublicWebApiBaseUrl() + `/api/bus-stops${getQueryString(filter, sorts, pageNumber, pageSize)}`, result => {
            return result;
        });
    },
    async count(): Promise<number> {
        return await getRequest(env.getPublicWebApiBaseUrl() + `/api/bus-stops/count`, result => {
            return result;
        });
    },
    async create(model: BusStopModel): Promise<BusStopModel> {
        return await authorisedPostRequest(env.getPublicWebApiBaseUrl() + `/api/bus-stops`, model, result => {
            return result;
        });
    },
    async update(id: number, model: BusStopModel): Promise<BusStopModel> {
        return await authorisedPutRequest(env.getPublicWebApiBaseUrl() + `/api/bus-stops/${id}`, model, result => {
            return result;
        });
    },
    async delete(id: number): Promise<BusStopModel> {
        return await authorisedDeleteRequest(env.getPublicWebApiBaseUrl() + `/api/bus-stops/${id}`, result => {
            return result;
        });
    }
}