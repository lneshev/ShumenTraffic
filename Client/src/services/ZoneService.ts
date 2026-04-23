import {
    authorisedDeleteRequest,
    authorisedPostRequest,
    getQueryString,
    getRequest
} from "@/helpers/Request";
import env from "@/services/EnvService";
import ZoneModel from "@/types/ZoneModel";
import PageResult from "@/types/common/PageResult";
import Sort from "@/types/common/Sort";

export default {
    async read(filter: Record<string, any> = {}, sorts: Sort[] = [], pageNumber?: number, pageSize?: number): Promise<PageResult<ZoneModel>> {
        return await getRequest(env.getPublicWebApiBaseUrl() + `/api/zones${getQueryString(filter, sorts, pageNumber, pageSize)}`, result => {
            return result;
        });
    },
    async create(model: ZoneModel): Promise<ZoneModel> {
        return await authorisedPostRequest(env.getPublicWebApiBaseUrl() + `/api/zones`, model, result => {
            return result;
        });
    },
    async delete(id: number): Promise<ZoneModel> {
        return await authorisedDeleteRequest(env.getPublicWebApiBaseUrl() + `/api/zones/${id}`, result => {
            return result;
        });
    }
}