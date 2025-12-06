import {
    authorisedDeleteRequest,
    authorisedPutRequest,
    getRequest
} from "@/helpers/Request";
import ScheduleModel from "@/types/ScheduleModel";

export default {
    async get(id: number): Promise<ScheduleModel> {
        return await getRequest(process.env.NEXT_PUBLIC_WEB_API_BASE_URL + `/api/schedules/${id}`, result => {
            return result;
        });
    },
    async count(): Promise<number> {
        return await getRequest(process.env.NEXT_PUBLIC_WEB_API_BASE_URL + `/api/schedules/count`, result => {
            return result;
        });
    },
    async update(model: ScheduleModel): Promise<ScheduleModel> {
        return await authorisedPutRequest(process.env.NEXT_PUBLIC_WEB_API_BASE_URL + `/api/schedules/${model.id}`, model, result => {
            return result;
        });
    },
    async delete(id: number): Promise<ScheduleModel> {
        return await authorisedDeleteRequest(process.env.NEXT_PUBLIC_WEB_API_BASE_URL + `/api/schedules/${id}`, result => {
            return result;
        });
    }
}