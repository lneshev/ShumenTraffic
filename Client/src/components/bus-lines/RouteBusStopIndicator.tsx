import { ROUTE_COLORS } from '@/constants/RouteColors';
import RouteModel from '@/types/RouteModel';

interface RouteBusStopIndicatorProps {
  routes: RouteModel[];
  busStopId: number;
}

// Component for the bus stop indicator
export default function RouteBusStopIndicator({
  routes,
  busStopId
}: RouteBusStopIndicatorProps) {
  return (
    <>
      {routes.map((route, index) => {
        const hasRouteBusStop = route.stops.some(stop => stop.busStopId === busStopId);
        const routeColor = ROUTE_COLORS[index % ROUTE_COLORS.length];

        if (hasRouteBusStop) {
          return (
            <div
              key={index}
              className="shrink-0 w-6 h-6 rounded-full flex items-center justify-center z-10 relative"
              style={{ backgroundColor: routeColor }}
            >
              <div className="w-3 h-3 rounded-full bg-white"></div>
            </div>
          );
        }

        return (
          <div key={index} className="w-6 h-6">
            <div className="border-l-4 h-full ml-2.5" style={{ borderColor: routeColor }}></div>
          </div>
        );
      })}
    </>
  );
}