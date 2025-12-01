import { ProtectedRoute } from "@/components/ProtectedRoute";
import ScheduleDetails from "./page.client";

export default async function ScheduleDetailsPageWrapper({ params }: {
    params: Promise<{ id: string }>
}) {
    const id = parseInt((await params).id);
    return (
        <ProtectedRoute>
            <ScheduleDetails id={id} />
        </ProtectedRoute>
    );
}