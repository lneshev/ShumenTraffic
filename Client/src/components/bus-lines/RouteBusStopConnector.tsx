import { ROUTE_COLORS } from "@/constants/RouteColors";
import RouteModel from "@/types/RouteModel";

interface RouteBusStopConnectorProps {
    routes: RouteModel[];
}

// Component for the connecting line between stops
export default function RouteBusStopConnector({ routes }: RouteBusStopConnectorProps) {
    return (
        <div className="flex items-start gap-1 h-5">
            {routes.map((route, index) => {
                const routeColor = ROUTE_COLORS[index % ROUTE_COLORS.length];
                return (
                    <div key={index} className="w-6 h-full flex justify-center">
                        <div className="w-1 h-full" style={{ backgroundColor: routeColor }}></div>
                    </div>
                );
            })}
        </div>
    );
}