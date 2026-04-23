import {
    authorisedDeleteRequest,
    authorisedPutRequest,
    getRequest
} from "@/helpers/Request";
import env from "@/services/EnvService";
import ScheduleModel from "@/types/ScheduleModel";

export default {
    async get(id: number): Promise<ScheduleModel> {
        return await getRequest(env.getPublicWebApiBaseUrl() + `/api/schedules/${id}`, result => {
            return result;
        });
    },
    async count(): Promise<number> {
        return await getRequest(env.getPublicWebApiBaseUrl() + `/api/schedules/count`, result => {
            return result;
        });
    },
    async update(model: ScheduleModel): Promise<ScheduleModel> {
        return await authorisedPutRequest(env.getPublicWebApiBaseUrl() + `/api/schedules/${model.id}`, model, result => {
            return result;
        });
    },
    async delete(id: number): Promise<ScheduleModel> {
        return await authorisedDeleteRequest(env.getPublicWebApiBaseUrl() + `/api/schedules/${id}`, result => {
            return result;
        });
    }
}