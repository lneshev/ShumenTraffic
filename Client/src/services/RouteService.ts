import {
    authorisedDeleteRequest,
    authorisedPostRequest,
    getQueryString,
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
    async create(model: RouteModel): Promise<RouteModel> {
        return await authorisedPostRequest(process.env.NEXT_PUBLIC_WEB_API_BASE_URL + `/api/routes`, model, result => {
            return result;
        });
    },
    async delete(id: number): Promise<RouteModel> {
        return await authorisedDeleteRequest(process.env.NEXT_PUBLIC_WEB_API_BASE_URL + `/api/routes/${id}`, result => {
            return result;
        });
    }
}