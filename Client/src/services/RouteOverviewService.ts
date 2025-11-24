import {
    authorisedDeleteRequest,
    authorisedPostRequest,
    getQueryString,
    getRequest
} from "@/helpers/Request";
import PageResult from "@/types/common/PageResult";
import Sort from "@/types/common/Sort";
import RouteOverviewModel from "@/types/RouteOverviewModel";

export default {
    async read(filter: Record<string, any> = {}, sorts: Sort[] = [], pageNumber?: number, pageSize?: number): Promise<PageResult<RouteOverviewModel>> {
        return await getRequest(process.env.NEXT_PUBLIC_WEB_API_BASE_URL + `/api/routes-overview${getQueryString(filter, sorts, pageNumber, pageSize)}`, result => {
            return result;
        });
    },
    async create(model: RouteOverviewModel): Promise<RouteOverviewModel> {
        return await authorisedPostRequest(process.env.NEXT_PUBLIC_WEB_API_BASE_URL + `/api/routes-overview`, model, result => {
            return result;
        });
    },
    async delete(id: number): Promise<RouteOverviewModel> {
        return await authorisedDeleteRequest(process.env.NEXT_PUBLIC_WEB_API_BASE_URL + `/api/routes-overview/${id}`, result => {
            return result;
        });
    }
}