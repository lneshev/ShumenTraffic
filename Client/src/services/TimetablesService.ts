import {
    getRequest
} from "@/helpers/Request";
import TimetableModel from "@/types/TimetableModel";

export default {
    async get(busLineId: number, direction: number, date: string): Promise<TimetableModel> {
        return await getRequest(process.env.NEXT_PUBLIC_WEB_API_BASE_URL + `/api/timetables?busLineId=${busLineId}&direction=${direction}&date=${date}`, result => {
            return result;
        });
    }
}