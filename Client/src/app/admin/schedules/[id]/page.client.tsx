'use client';

import EntityDropdown from "@/components/EntityDropdown";
import EnumDropdown from "@/components/EnumDropdown";
import { ApiError } from "@/lib/api";
import ScheduleService from "@/services/ScheduleService";
import BusLineLightModel from "@/types/BusLineLightModel";
import PageResult from "@/types/common/PageResult";
import ScheduleCourseModel from "@/types/ScheduleCourseModel";
import ScheduleModel from "@/types/ScheduleModel";
import { DateTime } from "luxon";
import Link from "next/link";
import { useRouter } from "next/navigation";
import { useEffect, useRef, useState } from "react";

export default function ScheduleDetails({ id }: { id: number }) {
    const initialScheduleCourse: ScheduleCourseModel = {
        id: 0
    };
    const formRef = useRef<HTMLFormElement>(null);
    const router = useRouter();
    const [schedule, setSchedule] = useState<ScheduleModel | null>(null);
    const [error, setError] = useState('');
    const startDateText = schedule ? DateTime.fromISO(schedule.startDate).toLocaleString(DateTime.DATE_SHORT, { locale: 'bg-BG' }) : '';
    const endDateText = schedule ? (schedule.endDate ? DateTime.fromISO(schedule.endDate).toLocaleString(DateTime.DATE_SHORT, { locale: 'bg-BG' }) : '♾️') : '';

    useEffect(() => {
        setModel();
    }, []);

    const setModel = async (model?: ScheduleModel) => {
        if (model) {
            setSchedule(model);
        } else {
            if (id) {
                try {
                    const model = await ScheduleService.get(id);
                    setSchedule(model);
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
            const model = await ScheduleService.update(schedule!);
            setModel(model);
        } catch (err) {
            setError(err instanceof ApiError ? err.message : 'Error updating route');
        }
    };

    if (!schedule) {
        return null;
    }

    return (
        <div className="min-h-screen bg-white dark:bg-slate-950">
            <div className="max-w-7xl mx-auto px-4 sm:px-6 lg:px-8 py-12">
                <div className="flex justify-between items-center mb-8">
                    <h1 className="text-4xl font-bold text-gray-900 dark:text-white">
                        Schedule for: 🚌 {schedule.busLineNumber} 📅 {startDateText} - {endDateText}
                    </h1>
                    <div className="flex gap-2">
                        <button
                            onClick={() => formRef.current?.requestSubmit()}
                            className="px-4 py-2 bg-green-600 hover:bg-green-700 text-white font-semibold rounded-lg transition-colors"
                        >
                            Save
                        </button>
                        <Link
                            href="/admin/schedules"
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
                    <div className="grid grid-cols-1 lg:grid-cols-3 gap-4">
                        <div className="mb-4">
                            <label className="block text-sm font-medium text-gray-700 dark:text-gray-300 mb-2">
                                Bus Line
                            </label>
                            <EntityDropdown
                                value={schedule.busLineId}
                                onChange={(e) => setSchedule({ ...schedule, busLineId: e ? e.value : 0, busLineNumber: e ? e.label : '' })}
                                placeholder="Select..."
                                url="/api/bus-lines-light"
                                sorts={[{ field: "lineNumber", dir: "asc" }]}
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
                                Start Date
                            </label>
                            <input
                                type="date"
                                value={schedule.startDate}
                                onChange={(e) => setSchedule({ ...schedule, startDate: e.target.value })}
                                className="w-full px-4 py-2 border border-gray-300 dark:border-slate-600 rounded-lg bg-white dark:bg-slate-800 text-gray-900 dark:text-white"
                                required
                            />
                        </div>
                        <div className="mb-4">
                            <label className="block text-sm font-medium text-gray-700 dark:text-gray-300 mb-2">
                                End Date
                            </label>
                            <input
                                type="date"
                                value={schedule.endDate || ''}
                                onChange={(e) => setSchedule({ ...schedule, endDate: e.target.value })}
                                className="w-full px-4 py-2 border border-gray-300 dark:border-slate-600 rounded-lg bg-white dark:bg-slate-800 text-gray-900 dark:text-white"
                            />
                        </div>
                    </div>
                    <div className="grid grid-cols-1 lg:grid-cols-3 gap-4">
                        <div className="mb-4">
                            <label className="block text-sm font-medium text-gray-700 dark:text-gray-300 mb-2">
                                Day Type
                            </label>
                            <EnumDropdown
                                enumName="DayType"
                                value={schedule.dayType}
                                onChange={(e) => setSchedule({ ...schedule, dayType: e ? e.value : 0 })}
                                isClearable={false}
                                required
                            />
                        </div>
                        <div className="mb-4">
                            <label className="block text-sm font-medium text-gray-700 dark:text-gray-300 mb-2">
                                Priority
                            </label>
                            <EnumDropdown
                                enumName="SchedulePriority"
                                value={schedule.priority}
                                onChange={(e) => setSchedule({ ...schedule, priority: e ? e.value : 0 })}
                                isClearable={false}
                                required
                            />
                        </div>
                    </div>
                </form>
            </div>
        </div>
    );
}