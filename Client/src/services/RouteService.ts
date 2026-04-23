import {
    authorisedDeleteRequest,
    authorisedGetRequest, authorisedPutRequest, getQueryString,
    getRequest
} from "@/helpers/Request";
import env from "@/services/EnvService";
import PageResult from "@/types/common/PageResult";
import Sort from "@/types/common/Sort";
import RouteModel from "@/types/RouteModel";

export default {
    async read(filter: Record<string, any> = {}, sorts: Sort[] = [], pageNumber?: number, pageSize?: number): Promise<PageResult<RouteModel>> {
        return await getRequest(env.getPublicWebApiBaseUrl() + `/api/routes${getQueryString(filter, sorts, pageNumber, pageSize)}`, result => {
            return result;
        });
    },
    async get(id: number): Promise<RouteModel> {
        return await authorisedGetRequest(env.getPublicWebApiBaseUrl() + `/api/routes/${id}`, result => {
            return result;
        });
    },
    async count(): Promise<number> {
        return await getRequest(env.getPublicWebApiBaseUrl() + `/api/routes/count`, result => {
            return result;
        });
    },
    async update(model: RouteModel): Promise<RouteModel> {
        return await authorisedPutRequest(env.getPublicWebApiBaseUrl() + `/api/routes/${model.id}`, model, result => {
            return result;
        });
    },
    async delete(id: number): Promise<RouteModel> {
        return await authorisedDeleteRequest(env.getPublicWebApiBaseUrl() + `/api/routes/${id}`, result => {
            return result;
        });
    }
}