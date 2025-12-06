import {
    authorisedDeleteRequest,
    authorisedPostRequest,
    authorisedPutRequest,
    getQueryString,
    getRequest
} from "@/helpers/Request";
import BusStopModel from "@/types/BusStopModel";
import PageResult from "@/types/common/PageResult";
import Sort from "@/types/common/Sort";

export default {
    async read(filter: Record<string, any> = {}, sorts: Sort[] = [], pageNumber?: number, pageSize?: number): Promise<PageResult<BusStopModel>> {
        return await getRequest(process.env.NEXT_PUBLIC_WEB_API_BASE_URL + `/api/bus-stops${getQueryString(filter, sorts, pageNumber, pageSize)}`, result => {
            return result;
        });
    },
    async count(): Promise<number> {
        return await getRequest(process.env.NEXT_PUBLIC_WEB_API_BASE_URL + `/api/bus-stops/count`, result => {
            return result;
        });
    },
    async create(model: BusStopModel): Promise<BusStopModel> {
        return await authorisedPostRequest(process.env.NEXT_PUBLIC_WEB_API_BASE_URL + `/api/bus-stops`, model, result => {
            return result;
        });
    },
    async update(id: number, model: BusStopModel): Promise<BusStopModel> {
        return await authorisedPutRequest(process.env.NEXT_PUBLIC_WEB_API_BASE_URL + `/api/bus-stops/${id}`, model, result => {
            return result;
        });
    },
    async delete(id: number): Promise<BusStopModel> {
        return await authorisedDeleteRequest(process.env.NEXT_PUBLIC_WEB_API_BASE_URL + `/api/bus-stops/${id}`, result => {
            return result;
        });
    }
}