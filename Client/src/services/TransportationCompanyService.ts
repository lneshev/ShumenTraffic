import {
    authorisedDeleteRequest,
    authorisedPostRequest,
    getQueryString,
    getRequest
} from "@/helpers/Request";
import env from "@/services/EnvService";
import TransportationCompanyModel from "@/types/TransportationCompanyModel";
import PageResult from "@/types/common/PageResult";
import Sort from "@/types/common/Sort";

export default {
    async read(filter: Record<string, any> = {}, sorts: Sort[] = [], pageNumber?: number, pageSize?: number): Promise<PageResult<TransportationCompanyModel>> {
        return await getRequest(env.getPublicWebApiBaseUrl() + `/api/transportation-companies${getQueryString(filter, sorts, pageNumber, pageSize)}`, result => {
            return result;
        });
    },
    async create(model: TransportationCompanyModel): Promise<TransportationCompanyModel> {
        return await authorisedPostRequest(env.getPublicWebApiBaseUrl() + `/api/transportation-companies`, model, result => {
            return result;
        });
    },
    async delete(id: number): Promise<TransportationCompanyModel> {
        return await authorisedDeleteRequest(env.getPublicWebApiBaseUrl() + `/api/transportation-companies/${id}`, result => {
            return result;
        });
    }
}