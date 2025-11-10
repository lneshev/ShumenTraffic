import {
    authorisedDeleteRequest,
    authorisedPostRequest,
    getQueryString,
    getRequest
} from "@/helpers/Request";
import TransportationCompanyModel from "@/types/TransportationCompanyModel";
import PageResult from "@/types/common/PageResult";

export default {
    async read(filter: Record<string, any> = {}, sorts: { field: string; dir: number | "asc" | "desc" }[] = [], pageNumber?: number, pageSize?: number): Promise<PageResult<TransportationCompanyModel>> {
        return await getRequest(process.env.NEXT_PUBLIC_WEB_API_BASE_URL + `/api/transportation-companies${getQueryString(filter, sorts, pageNumber, pageSize)}`, result => {
            return result;
        });
    },
    async create(model: TransportationCompanyModel): Promise<TransportationCompanyModel> {
        return await authorisedPostRequest(process.env.NEXT_PUBLIC_WEB_API_BASE_URL + `/api/transportation-companies`, model, result => {
            return result;
        });
    },
    async delete(id: number): Promise<TransportationCompanyModel> {
        return await authorisedDeleteRequest(process.env.NEXT_PUBLIC_WEB_API_BASE_URL + `/api/transportation-companies/${id}`, result => {
            return result;
        });
    }
}