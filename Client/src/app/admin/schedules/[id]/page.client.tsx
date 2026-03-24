'use client';

import EntityDropdown from "@/components/EntityDropdown";
import EnumDropdown from "@/components/EnumDropdown";
import FlagsEnumMultiselect from "@/components/FlagsEnumMultiselect";
import { nullifyNegativeIds } from "@/helpers/Request";
import ScheduleService from "@/services/ScheduleService";
import BusLineLightModel from "@/types/BusLineLightModel";
import { ApiError } from "@/types/common/ApiError";
import PageResult from "@/types/common/PageResult";
import ServerEnums from "@/types/common/ServerEnums";
import RouteOverviewModel from "@/types/RouteOverviewModel";
import ScheduleCourseModel from "@/types/ScheduleCourseModel";
import ScheduleModel from "@/types/ScheduleModel";
import { DateTime } from "luxon";
import Link from "next/link";
import { useRouter } from "next/navigation";
import { useEffect, useRef, useState } from "react";
import DatePicker from "react-datepicker";

export default function ScheduleDetails({ id }: { id: number }) {
    const initialScheduleCourse: ScheduleCourseModel = {
        id: 0,
        departureTime: '',
        routeId: 0
    };
    const formRef = useRef<HTMLFormElement>(null);
    const router = useRouter();
    const [schedule, setSchedule] = useState<ScheduleModel | null>(null);
    const [newScheduleCourse, setNewScheduleCourse] = useState<ScheduleCourseModel>({ ...initialScheduleCourse });
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

    const handleNewScheduleCourseSubmit = (e: React.FormEvent) => {
        e.preventDefault();

        const newScheduleCourseToAdd: ScheduleCourseModel = {
            ...newScheduleCourse,
            id: getNewMimimumId()
        };

        setSchedule({
            ...schedule!,
            scheduleCourses: [...schedule!.scheduleCourses, newScheduleCourseToAdd]
        });

        setNewScheduleCourse({ ...initialScheduleCourse });
    }

    const getNewMimimumId = () => {
        const result = Math.min(0, Math.min(...schedule!.scheduleCourses.map(x => x.id))) - 1;
        return result;
    }

    const handleSubmit = async (e: React.FormEvent) => {
        e.preventDefault();
        try {
            setError('');
            const scheduleToSend: ScheduleModel = { ...schedule!, scheduleCourses: nullifyNegativeIds(schedule!.scheduleCourses) };
            const model = await ScheduleService.update(scheduleToSend);
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

                <div className="p-6 bg-gray-50 dark:bg-slate-900 rounded-lg border border-gray-200 dark:border-slate-700">
                    <form onSubmit={handleSubmit} ref={formRef}>
                        <div className="grid grid-cols-1 lg:grid-cols-3 gap-4 mb-4">
                            <div>
                                <label className="block text-sm font-medium text-gray-700 dark:text-gray-300 mb-2">
                                    Bus Line
                                </label>
                                <EntityDropdown
                                    value={schedule.busLineId}
                                    onChange={(e) => setSchedule({ ...schedule, busLineId: e ? e.value : 0, busLineNumber: e ? e.label : '' })}
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
                                    isDisabled={schedule.scheduleCourses.length > 0}
                                    required
                                />
                            </div>
                            <div>
                                <label className="block text-sm font-medium text-gray-700 dark:text-gray-300 mb-2">
                                    Direction
                                </label>
                                <EnumDropdown
                                    enumName="RouteDirection"
                                    value={schedule.direction}
                                    onChange={(e) => setSchedule({ ...schedule, direction: e ? e.value : 0 })}
                                    isDisabled={schedule.scheduleCourses.length > 0}
                                    required
                                />
                            </div>
                            <div>
                                <label className="block text-sm font-medium text-gray-700 dark:text-gray-300 mb-2">
                                    Days of Week
                                </label>
                                <FlagsEnumMultiselect
                                    enumName="DaysOfWeek"
                                    exactEnumValues={[
                                        ServerEnums.DaysOfWeek.Monday,
                                        ServerEnums.DaysOfWeek.Tuesday,
                                        ServerEnums.DaysOfWeek.Wednesday,
                                        ServerEnums.DaysOfWeek.Thursday,
                                        ServerEnums.DaysOfWeek.Friday,
                                        ServerEnums.DaysOfWeek.Saturday,
                                        ServerEnums.DaysOfWeek.Sunday
                                    ]}
                                    value={schedule.daysOfWeek}
                                    onChange={(e) => setSchedule({ ...schedule, daysOfWeek: e ? e : 0 })}
                                    required
                                />
                            </div>
                            <div>
                                <label className="block text-sm font-medium text-gray-700 dark:text-gray-300 mb-2">
                                    Start Date
                                </label>
                                <DatePicker
                                    selected={schedule.startDate ? DateTime.fromISO(schedule.startDate).toJSDate() : null}
                                    onChange={(e: Date | null) => setSchedule({ ...schedule, startDate: DateTime.fromJSDate(e!).toISODate()! })}
                                    dateFormat={"dd/MM/yyyy"}
                                    placeholderText="Select..."
                                    required
                                />
                            </div>
                            <div>
                                <label className="block text-sm font-medium text-gray-700 dark:text-gray-300 mb-2">
                                    End Date
                                </label>
                                <DatePicker
                                    selected={schedule.endDate ? DateTime.fromISO(schedule.endDate).toJSDate() : null}
                                    onChange={(e: Date | null) => setSchedule({ ...schedule, endDate: e ? DateTime.fromJSDate(e).toISODate()! : undefined })}
                                    dateFormat={"dd/MM/yyyy"}
                                    placeholderText="Select..."
                                    isClearable
                                />
                            </div>
                            <div>
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
                    <div>
                        <div>
                            <label className="block text-sm font-medium text-gray-700 dark:text-gray-300 mb-2">
                                Schedule Courses ({schedule.scheduleCourses.length})
                            </label>
                            <ul>
                                {schedule.scheduleCourses.sort((a, b) => a.departureTime.localeCompare(b.departureTime)).map((scheduleCourse, i) => {
                                    return (
                                        <li
                                            key={scheduleCourse.id}
                                            className="grid grid-cols-3 gap-4 justify-between border border-b-0 last:border-b border-gray-300 dark:border-slate-600 bg-white dark:bg-slate-800 p-2"
                                        >
                                            <div>
                                                <DatePicker
                                                    selected={scheduleCourse.departureTime ? DateTime.fromISO(scheduleCourse.departureTime).toJSDate() : null}
                                                    onChange={(e: Date | null) => setSchedule({
                                                        ...schedule,
                                                        scheduleCourses: schedule.scheduleCourses.map(x => x.id === scheduleCourse.id ? { ...x, departureTime: DateTime.fromJSDate(e!).toISOTime({ precision: 'minutes', includeOffset: false })! } : x)
                                                    })}
                                                    showTimeSelect
                                                    showTimeSelectOnly
                                                    dateFormat={"HH:mm"}
                                                    timeFormat={"HH:mm"}
                                                    placeholderText="Select..."
                                                    required
                                                />
                                            </div>
                                            <EntityDropdown
                                                value={scheduleCourse.routeId}
                                                onChange={(e) => setSchedule({
                                                    ...schedule,
                                                    scheduleCourses: schedule.scheduleCourses.map(x => x.id === scheduleCourse.id ? { ...x, routeId: e ? e.value : 0 } : x)
                                                })}
                                                placeholder="Select..."
                                                url="/api/routes-overview"
                                                filter={{
                                                    busLineId: schedule.busLineId,
                                                    direction: schedule.direction
                                                }}
                                                sorts={[
                                                    { field: 'Name', dir: 'asc' },
                                                    { field: 'BusLineNumber', dir: 'asc' },
                                                    { field: 'DirectionText', dir: 'asc' }
                                                ]}
                                                parseData={(data: PageResult<RouteOverviewModel>) =>
                                                    data.items.map((item, i) => {
                                                        return {
                                                            value: item.id,
                                                            label: item.name
                                                        };
                                                    })
                                                }
                                                required
                                            />
                                            <button
                                                type="button"
                                                onClick={(e) => setSchedule({
                                                    ...schedule,
                                                    scheduleCourses: schedule.scheduleCourses.filter(x => x.id !== scheduleCourse.id)
                                                })}
                                                className="bg-red-600 hover:bg-red-700 text-white rounded transition-colors w-9 h-full"
                                            >
                                                x
                                            </button>
                                        </li>
                                    );
                                })}
                                <form onSubmit={handleNewScheduleCourseSubmit}>
                                    <li
                                        className="grid grid-cols-3 gap-4 justify-between border border-b-0 last:border-b border-gray-300 dark:border-slate-600 bg-white dark:bg-slate-800 p-2"
                                    >
                                        <div>
                                            <DatePicker
                                                selected={newScheduleCourse.departureTime ? DateTime.fromISO(newScheduleCourse.departureTime).toJSDate() : null}
                                                onChange={(e: Date | null) => setNewScheduleCourse({ ...newScheduleCourse, departureTime: DateTime.fromJSDate(e!).toISOTime({ precision: 'minutes', includeOffset: false })! })}
                                                showTimeSelect
                                                showTimeSelectOnly
                                                dateFormat={"HH:mm"}
                                                timeFormat={"HH:mm"}
                                                placeholderText="Select..."
                                                required
                                            />
                                        </div>
                                        <EntityDropdown
                                            value={newScheduleCourse.routeId}
                                            onChange={(e) => setNewScheduleCourse({ ...newScheduleCourse, routeId: e ? e.value : 0 })}
                                            placeholder="Select..."
                                            url="/api/routes-overview"
                                            filter={{
                                                busLineId: schedule.busLineId,
                                                direction: schedule.direction
                                            }}
                                            sorts={[
                                                { field: 'Name', dir: 'asc' },
                                                { field: 'BusLineNumber', dir: 'asc' },
                                                { field: 'DirectionText', dir: 'asc' }
                                            ]}
                                            parseData={(data: PageResult<RouteOverviewModel>) =>
                                                data.items.map((item, i) => {
                                                    return {
                                                        value: item.id,
                                                        label: item.name
                                                    };
                                                })
                                            }
                                            required
                                        />
                                        <button
                                            type="submit"
                                            className="bg-green-600 hover:bg-green-700 text-white rounded transition-colors w-9 h-full"
                                        >
                                            +
                                        </button>
                                    </li>
                                </form>
                            </ul>
                        </div>
                    </div>
                </div>
            </div>
        </div>
    );
}