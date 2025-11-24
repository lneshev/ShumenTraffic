'use client';

import EntityDropdown from "@/components/EntityDropdown";
import EnumDropdown from "@/components/EnumDropdown";
import { ApiError } from "@/lib/api";
import RouteService from "@/services/RouteService";
import BusLineLightModel from "@/types/BusLineLightModel";
import PageResult from "@/types/common/PageResult";
import RouteModel from "@/types/RouteModel";
import Link from "next/link";
import { useRouter } from "next/navigation";
import { useEffect, useState } from "react";

export default function RouteDetails({ id }: { id: number }) {
    const router = useRouter();
    const [route, setRoute] = useState<RouteModel | null>(null);
    const [error, setError] = useState('');

    useEffect(() => {
        setModel();
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

    const handleSubmit = async (e: React.FormEvent) => {
        e.preventDefault();
        try {
            setError('');
            const model = await RouteService.update(route!);
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
                            onClick={handleSubmit}
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

                <form onSubmit={handleSubmit} className="mb-8 p-6 bg-gray-50 dark:bg-slate-900 rounded-lg border border-gray-200 dark:border-slate-700">
                    <div className="mb-4">
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
                    <div className="mb-4">
                        <label className="block text-sm font-medium text-gray-700 dark:text-gray-300 mb-2">
                            Bus Line
                        </label>
                        <EntityDropdown
                            value={route.busLineId}
                            onChange={(e) => setRoute({ ...route, busLineId: e ? e.value : 0 })}
                            placeholder="Select..."
                            url="/api/bus-lines-light"
                            sorts={[
                                { field: "name", dir: "asc" }
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
                    <div className="mb-4">
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
                </form>
            </div>
        </div>
    );
}