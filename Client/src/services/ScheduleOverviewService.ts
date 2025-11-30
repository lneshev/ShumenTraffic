import {
    authorisedPostRequest,
    getQueryString,
    getRequest
} from "@/helpers/Request";
import PageResult from "@/types/common/PageResult";
import Sort from "@/types/common/Sort";
import ScheduleOverviewModel from "@/types/ScheduleOverviewModel";

export default {
    async read(filter: Record<string, any> = {}, sorts: Sort[] = [], pageNumber?: number, pageSize?: number): Promise<PageResult<ScheduleOverviewModel>> {
        return await getRequest(process.env.NEXT_PUBLIC_WEB_API_BASE_URL + `/api/schedules-overview${getQueryString(filter, sorts, pageNumber, pageSize)}`, result => {
            return result;
        });
    },
    async create(model: ScheduleOverviewModel): Promise<ScheduleOverviewModel> {
        return await authorisedPostRequest(process.env.NEXT_PUBLIC_WEB_API_BASE_URL + `/api/schedules-overview`, model, result => {
            return result;
        });
    }
}