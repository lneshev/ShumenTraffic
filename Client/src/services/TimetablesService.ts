import {
    getRequest
} from "@/helpers/Request";
import env from "@/services/EnvService";
import TimetableModel from "@/types/TimetableModel";

export default {
    async get(busLineId: number, direction: number, date: string): Promise<TimetableModel> {
        return await getRequest(env.getPublicWebApiBaseUrl() + `/api/timetables?busLineId=${busLineId}&direction=${direction}&date=${date}`, result => {
            return result;
        });
    }
}