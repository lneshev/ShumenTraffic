import {
    authorisedDeleteRequest,
    authorisedPostRequest,
    getQueryString,
    getRequest
} from "@/helpers/Request";
import BusLineModel from "@/types/BusLineModel";
import PageResult from "@/types/common/PageResult";

export default {
    async read(filter: Record<string, any> = {}, sorts: { field: string; dir: number | "asc" | "desc" }[] = [], pageNumber?: number, pageSize?: number): Promise<PageResult<BusLineModel>> {
        return await getRequest(process.env.NEXT_PUBLIC_WEB_API_BASE_URL + `/api/bus-lines${getQueryString(filter, sorts, pageNumber, pageSize)}`, result => {
            return result;
        });
    },
    async create(model: BusLineModel): Promise<BusLineModel> {
        return await authorisedPostRequest(process.env.NEXT_PUBLIC_WEB_API_BASE_URL + `/api/bus-lines`, model, result => {
            return result;
        });
    },
    async delete(id: number): Promise<BusLineModel> {
        return await authorisedDeleteRequest(process.env.NEXT_PUBLIC_WEB_API_BASE_URL + `/api/bus-lines/${id}`, result => {
            return result;
        });
    }
}