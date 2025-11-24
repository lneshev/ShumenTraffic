import { ProtectedRoute } from "@/components/ProtectedRoute";
import RouteDetails from "./page.client";

export default async function RouteDetailsPageWrapper({ params }: {
    params: Promise<{ id: string }>
}) {
    const id = parseInt((await params).id);
    return (
        <ProtectedRoute>
            <RouteDetails id={id} />
        </ProtectedRoute>
    );
}