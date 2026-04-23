import {
    getQueryString,
    getRequest
} from "@/helpers/Request";
import env from "@/services/EnvService";
import BusLineModel from "@/types/BusLineModel";
import PageResult from "@/types/common/PageResult";
import Sort from "@/types/common/Sort";

export default {
    async read(filter: Record<string, any> = {}, sorts: Sort[] = [], pageNumber?: number, pageSize?: number): Promise<PageResult<BusLineModel>> {
        return await getRequest(env.getPublicWebApiBaseUrl() + `/api/bus-lines-light${getQueryString(filter, sorts, pageNumber, pageSize)}`, result => {
            return result;
        });
    }
}