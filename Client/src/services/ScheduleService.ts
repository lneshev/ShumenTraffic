import {
    authorisedDeleteRequest
} from "@/helpers/Request";
import ScheduleModel from "@/types/ScheduleModel";

export default {
    async delete(id: number): Promise<ScheduleModel> {
        return await authorisedDeleteRequest(process.env.NEXT_PUBLIC_WEB_API_BASE_URL + `/api/schedules/${id}`, result => {
            return result;
        });
    }
}