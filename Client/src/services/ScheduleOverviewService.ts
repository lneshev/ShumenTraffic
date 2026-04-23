import {
    authorisedPostRequest,
    getQueryString,
    getRequest
} from "@/helpers/Request";
import env from "@/services/EnvService";
import PageResult from "@/types/common/PageResult";
import Sort from "@/types/common/Sort";
import ScheduleOverviewModel from "@/types/ScheduleOverviewModel";

export default {
    async read(filter: Record<string, any> = {}, sorts: Sort[] = [], pageNumber?: number, pageSize?: number): Promise<PageResult<ScheduleOverviewModel>> {
        return await getRequest(env.getPublicWebApiBaseUrl() + `/api/schedules-overview${getQueryString(filter, sorts, pageNumber, pageSize)}`, result => {
            return result;
        });
    },
    async create(model: ScheduleOverviewModel): Promise<ScheduleOverviewModel> {
        return await authorisedPostRequest(env.getPublicWebApiBaseUrl() + `/api/schedules-overview`, model, result => {
            return result;
        });
    }
}