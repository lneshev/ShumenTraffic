'use client';

import EntityDropdown from "@/components/EntityDropdown";
import EnumDropdown from "@/components/EnumDropdown";
import MapLoader from "@/components/maps/MapLoader";
import { nullifyNegativeIds } from "@/helpers/Request";
import { ApiError } from "@/lib/api";
import BusStopService from "@/services/BusStopService";
import RouteService from "@/services/RouteService";
import BusLineLightModel from "@/types/BusLineLightModel";
import BusStopModel from "@/types/BusStopModel";
import { GeoPoint } from "@/types/common/GeoJSON";
import PageResult from "@/types/common/PageResult";
import RouteModel from "@/types/RouteModel";
import RouteStopModel from "@/types/RouteStopModel";
import dynamic from "next/dynamic";
import Link from "next/link";
import { useRouter } from "next/navigation";
import { useEffect, useRef, useState } from "react";

// Dynamically import Map to avoid SSR issues
const RouteMap = dynamic(() => import('@/components/maps/RouteMap').then(mod => ({ default: mod.default })), {
    ssr: false,
    loading: () => <MapLoader />
});

export default function RouteDetails({ id }: { id: number }) {
    const initialRouteStop: RouteStopModel = {
        id: 0,
        busStopId: undefined,
        busStopName: undefined,
        location: undefined,
        stopOrder: 0,
        estimatedMinutesFromStart: undefined
    };
    const router = useRouter();
    const formRef = useRef<HTMLFormElement>(null);
    const [route, setRoute] = useState<RouteModel | null>(null);
    const [busStops, setBusStops] = useState<BusStopModel[]>([]);
    const [newRouteStop, setNewRouteStop] = useState<RouteStopModel>({ ...initialRouteStop });
    const [error, setError] = useState('');
    let previousRouteStopEstMins = 0;

    useEffect(() => {
        setModel();
        fetchBusStops();
    }, []);

    const setModel = async (model?: RouteModel) => {
        if (model) {
            setRoute(model);
        } else {
            if (id) {
                try {
                    const model = await RouteService.get(id);
                    setRoute(model);
                } catch (err) {
                    router.replace('/not-found');
                }
            }
            else {
                router.replace('/not-found');
            }
        }
    }

    const fetchBusStops = async () => {
        try {
            const data = await BusStopService.read(undefined, [{ field: 'Name', dir: 'asc' }]);
            setBusStops(data.items);
        } catch (err) {
            setError(err instanceof ApiError ? err.message : 'Error loading bus stops');
        }
    };

    const handleRouteStopAdd = (lat: number, lng: number) => {
        const newRouteStop = {
            ...initialRouteStop,
            id: getNewMimimumId(),
            location: new GeoPoint(lat, lng),
            stopOrder: getNewStopOrder()
        };

        setRoute({
            ...route!,
            stops: [...route!.stops, newRouteStop]
        });
    }

    const getNewMimimumId = () => {
        const result = Math.min(0, Math.min(...route!.stops.map(x => x.id))) - 1;
        return result;
    }

    const getNewStopOrder = () => {
        const result = Math.max(0, Math.max(...route!.stops.map(x => x.stopOrder))) + 1;
        return result;
    }

    const handleRouteStopRemove = (routeStop: RouteStopModel) => {
        // Find the current version of the routeStop to get the correct stopOrder
        const currentRouteStop = route!.stops.find(x => x.id === routeStop.id);
        if (!currentRouteStop) {
            return;
        }

        setRoute({
            ...route!,
            stops: route!.stops
                .filter(x => x.id !== routeStop.id)
                .map(x => {
                    // Decrement stopOrder for all stops after the removed one
                    if (x.stopOrder > currentRouteStop.stopOrder) {
                        return { ...x, stopOrder: x.stopOrder - 1 };
                    }
                    return x;
                })
        });
    }

    const handleRouteStopDragEnd = async (routeStop: RouteStopModel, newLat: number, newLng: number) => {
        setRoute({
            ...route!,
            stops: route!.stops.map(x => {
                if (x.id === routeStop.id) {
                    // Use the current state version of the routeStop, not the stale one from the event
                    return {
                        ...x,
                        location: new GeoPoint(newLat, newLng)
                    };
                }
                return x;
            })
        });
    }

    const handleBusStopAddToRoute = (busStop: BusStopModel, e: Event) => {
        if (route!.stops.some(x => x.busStopId === busStop.id) && !confirm("Are you sure you want to add this bus stop again?")) {
            return;
        }

        const newRouteStop = {
            ...initialRouteStop,
            id: getNewMimimumId(),
            busStopId: busStop.id,
            busStopName: busStop.name,
            busStopLocation: busStop.location,
            location: undefined,
            stopOrder: getNewStopOrder()
        };

        setRoute({
            ...route!,
            stops: [...route!.stops, newRouteStop]
        });
    }

    const handleRouteStopEstMinsFromStartChange = (routeStop: RouteStopModel, value?: number) => {
        setRoute({
            ...route!,
            stops: route!.stops.map(x => x.id === routeStop.id ? { ...x, estimatedMinutesFromStart: value } : x)
        });
    }

    const handleRouteStopOrderUp = (routeStop: RouteStopModel) => {
        const otherRouteStop = route!.stops.find(x => x.stopOrder === routeStop.stopOrder - 1);
        if (!otherRouteStop) {
            return;
        }

        setRoute({
            ...route!,
            stops: route!.stops.map(x => {
                if (x.id === routeStop.id) {
                    return { ...x, stopOrder: x.stopOrder - 1 };
                }
                if (x.id === otherRouteStop.id) {
                    return { ...x, stopOrder: x.stopOrder + 1 };
                }
                return x;
            })
        });
    }

    const handleRouteStopOrderDown = (routeStop: RouteStopModel) => {
        const otherRouteStop = route!.stops.find(x => x.stopOrder === routeStop.stopOrder + 1);
        if (!otherRouteStop) {
            return;
        }

        setRoute({
            ...route!,
            stops: route!.stops.map(x => {
                if (x.id === routeStop.id) {
                    return { ...x, stopOrder: x.stopOrder + 1 };
                }
                if (x.id === otherRouteStop.id) {
                    return { ...x, stopOrder: x.stopOrder - 1 };
                }
                return x;
            })
        });
    }

    const handleSubmit = async (e: React.FormEvent) => {
        e.preventDefault();
        try {
            setError('');
            const routeToSend: RouteModel = { ...route!, stops: nullifyNegativeIds(route!.stops) };
            const model = await RouteService.update(routeToSend);
            setModel(model);
        } catch (err) {
            setError(err instanceof ApiError ? err.message : 'Error updating route');
        }
    };

    if (!route) {
        return null;
    }

    return (
        <div className="min-h-screen bg-white dark:bg-slate-950">
            <div className="max-w-7xl mx-auto px-4 sm:px-6 lg:px-8 py-12">
                <div className="flex justify-between items-center mb-8">
                    <h1 className="text-4xl font-bold text-gray-900 dark:text-white">
                        Route: {route.name}
                    </h1>
                    <div className="flex gap-2">
                        <button
                            onClick={() => formRef.current?.requestSubmit()}
                            className="px-4 py-2 bg-green-600 hover:bg-green-700 text-white font-semibold rounded-lg transition-colors"
                        >
                            Save
                        </button>
                        <Link
                            href="/admin/routes"
                            className="px-4 py-2 bg-gray-600 hover:bg-gray-700 text-white font-semibold rounded-lg transition-colors"
                        >
                            Back
                        </Link>
                    </div>
                </div>

                {error && (
                    <div className="mb-6 p-4 bg-red-50 dark:bg-red-900/20 border border-red-200 dark:border-red-700 rounded-lg">
                        <p className="text-red-800 dark:text-red-200">{error}</p>
                    </div>
                )}

                <form onSubmit={handleSubmit} ref={formRef} className="mb-8 p-6 bg-gray-50 dark:bg-slate-900 rounded-lg border border-gray-200 dark:border-slate-700">
                    <div className="grid grid-cols-1 lg:grid-cols-3 gap-4 mb-4">
                        <div>
                            <label className="block text-sm font-medium text-gray-700 dark:text-gray-300 mb-2">
                                Route Name
                            </label>
                            <input
                                type="text"
                                value={route.name}
                                onChange={(e) => setRoute({ ...route, name: e.target.value })}
                                className="w-full px-4 py-2 border border-gray-300 dark:border-slate-600 rounded-lg bg-white dark:bg-slate-800 text-gray-900 dark:text-white"
                                required
                                maxLength={255}
                            />
                        </div>
                        <div>
                            <label className="block text-sm font-medium text-gray-700 dark:text-gray-300 mb-2">
                                Bus Line
                            </label>
                            <EntityDropdown
                                value={route.busLineId}
                                onChange={(e) => setRoute({ ...route, busLineId: e ? e.value : 0 })}
                                placeholder="Select..."
                                url="/api/bus-lines-light"
                                sorts={[
                                    { field: "LineNumber", dir: "asc" }
                                ]}
                                parseData={(data: PageResult<BusLineLightModel>) =>
                                    data.items.map((item, i) => {
                                        return {
                                            value: item.id,
                                            label: item.lineNumber
                                        };
                                    })
                                }
                                required
                            />
                        </div>
                        <div>
                            <label className="block text-sm font-medium text-gray-700 dark:text-gray-300 mb-2">
                                Direction
                            </label>
                            <EnumDropdown
                                enumName="RouteDirection"
                                value={route.direction}
                                onChange={(e) => setRoute({ ...route, direction: e ? e.value : 0 })}
                                required
                            />
                        </div>
                    </div>
                    <div className="grid grid-cols-1 lg:grid-cols-10 gap-4">
                        <div className="lg:col-span-2">
                            <label className="block text-sm font-medium text-gray-700 dark:text-gray-300 mb-2">
                                Route Stops
                            </label>
                            {route.stops.length === 0 && (
                                <p className="text-gray-600 dark:text-gray-400">None</p>
                            )}
                            <ul className="h-[600px] overflow-auto">
                                {route.stops.sort((a, b) => a.stopOrder - b.stopOrder).map((routeStop, i) => {
                                    previousRouteStopEstMins = Math.max(previousRouteStopEstMins, route.stops[i - 1]?.estimatedMinutesFromStart || 0);
                                    const rsEstMins = routeStop.estimatedMinutesFromStart;
                                    return (
                                        <li
                                            key={routeStop.id}
                                            className="flex justify-between border border-b-0 last:border-b border-gray-300 dark:border-slate-600 bg-white dark:bg-slate-800 p-2"
                                        >
                                            <div className="flex flex-col mr-2 w-full">
                                                <p className="float-left inline">
                                                    <img
                                                        src={routeStop.busStopId ? '/icons/bus-stop-32x32.png' : '/icons/route-stop-32x32.png'}
                                                        width={24}
                                                        height={24}
                                                        className="inline mr-2"
                                                    />
                                                    <span className={routeStop.busStopId ? 'font-bold' : 'text-xs text-gray-600'}>
                                                        {routeStop.busStopId ? routeStop.busStopName : `${routeStop.location?.latitude}, ${routeStop.location?.longitude}`}
                                                    </span>
                                                </p>
                                                <p className="mt-auto">
                                                    <label className="block text-xs text-gray-400 dark:text-gray-300">
                                                        Est. minutes from start
                                                    </label>
                                                    <input
                                                        type="number"
                                                        value={rsEstMins || ""}
                                                        min={0}
                                                        onChange={(e) => handleRouteStopEstMinsFromStartChange(routeStop, parseInt(e.target.value) || undefined)}
                                                        className={`w-full px-3 py-2 border ${typeof rsEstMins === 'number' && rsEstMins <= previousRouteStopEstMins ? 'border-red-500 dark:border-red-400' : 'border-gray-300 dark:border-slate-600'} rounded-lg bg-white dark:bg-slate-800 text-gray-900 dark:text-white`}
                                                    />
                                                </p>
                                            </div>
                                            <div className="text-center">
                                                <p>
                                                    <button
                                                        type="button"
                                                        onClick={() => handleRouteStopRemove(routeStop)}
                                                        className="bg-red-600 hover:bg-red-700 text-white rounded transition-colors w-6 mb-1"
                                                    >
                                                        x
                                                    </button>
                                                </p>
                                                <p>
                                                    <button
                                                        type="button"
                                                        onClick={() => handleRouteStopOrderUp(routeStop)}
                                                        className="bg-blue-600 hover:bg-blue-700  disabled:bg-gray-200 text-white rounded transition-colors w-6"
                                                        disabled={i === 0}
                                                    >
                                                        +
                                                    </button>
                                                </p>
                                                <p>
                                                    {routeStop.stopOrder}
                                                </p>
                                                <p>
                                                    <button
                                                        type="button"
                                                        onClick={() => handleRouteStopOrderDown(routeStop)}
                                                        className="bg-blue-600 hover:bg-blue-700 disabled:bg-gray-200 text-white rounded transition-colors w-6"
                                                        disabled={i === route.stops.length - 1}
                                                    >
                                                        -
                                                    </button>
                                                </p>
                                            </div>
                                        </li>
                                    );
                                })}
                            </ul>
                        </div>
                        <div className="lg:col-span-8">
                            <label className="block text-sm font-medium text-gray-700 dark:text-gray-300 mb-2">
                                Map
                            </label>
                            <div className="h-[600px] bg-gray-100 dark:bg-slate-800 rounded-lg border border-gray-200 dark:border-slate-700 overflow-hidden">
                                <RouteMap
                                    busStops={busStops}
                                    routeStops={route.stops}
                                    newRouteStop={newRouteStop}
                                    onRouteStopAdd={handleRouteStopAdd}
                                    onRouteStopRemove={handleRouteStopRemove}
                                    onRouteStopDragEnd={handleRouteStopDragEnd}
                                    onBusStopAddToRoute={handleBusStopAddToRoute}
                                />
                            </div>
                        </div>
                    </div>
                </form>
            </div>
        </div>
    );
}