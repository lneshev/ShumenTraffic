import {
    authorisedGetRequest, authorisedPutRequest, getQueryString,
    getRequest
} from "@/helpers/Request";
import PageResult from "@/types/common/PageResult";
import Sort from "@/types/common/Sort";
import RouteModel from "@/types/RouteModel";

export default {
    async read(filter: Record<string, any> = {}, sorts: Sort[] = [], pageNumber?: number, pageSize?: number): Promise<PageResult<RouteModel>> {
        return await getRequest(process.env.NEXT_PUBLIC_WEB_API_BASE_URL + `/api/routes${getQueryString(filter, sorts, pageNumber, pageSize)}`, result => {
            return result;
        });
    },
    async get(id: number): Promise<RouteModel> {
        return await authorisedGetRequest(process.env.NEXT_PUBLIC_WEB_API_BASE_URL + `/api/routes/${id}`, result => {
            return result;
        });
    },
    async update(model: RouteModel): Promise<RouteModel> {
        return await authorisedPutRequest(process.env.NEXT_PUBLIC_WEB_API_BASE_URL + `/api/routes/${model.id}`, model, result => {
            return result;
        });
    }
}