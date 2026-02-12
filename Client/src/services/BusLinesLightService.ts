import {
    getQueryString,
    getRequest
} from "@/helpers/Request";
import BusLineModel from "@/types/BusLineModel";
import PageResult from "@/types/common/PageResult";
import Sort from "@/types/common/Sort";

export default {
    async read(filter: Record<string, any> = {}, sorts: Sort[] = [], pageNumber?: number, pageSize?: number): Promise<PageResult<BusLineModel>> {
        return await getRequest(process.env.NEXT_PUBLIC_WEB_API_BASE_URL + `/api/bus-lines-light${getQueryString(filter, sorts, pageNumber, pageSize)}`, result => {
            return result;
        });
    }
}