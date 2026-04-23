import {
    getQueryString,
    getRequest
} from "@/helpers/Request";
import env from "@/services/EnvService";
import ZoneWithBusLinesModel from "@/types/ZoneWithBusLinesModel";
import PageResult from "@/types/common/PageResult";
import Sort from "@/types/common/Sort";

export default {
    async read(filter: Record<string, any> = {}, sorts: Sort[] = [], pageNumber?: number, pageSize?: number): Promise<PageResult<ZoneWithBusLinesModel>> {
        return await getRequest(env.getPublicWebApiBaseUrl() + `/api/zones-with-bus-lines${getQueryString(filter, sorts, pageNumber, pageSize)}`, result => {
            return result;
        });
    }
}