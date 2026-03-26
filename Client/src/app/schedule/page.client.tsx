'use client';

import DirectionSelector from "@/components/DirectionSelector";
import { EntityDropdownLoader } from "@/components/EntityDropdown";
import TimetablesService from "@/services/TimetablesService";
import BusLineLightModel from "@/types/BusLineLightModel";
import PageResult from "@/types/common/PageResult";
import TimetableModel from "@/types/TimetableModel";
import { DateTime } from "luxon";
import dynamic from "next/dynamic";
import { useRouter } from "next/navigation";
import { useCallback, useEffect, useState } from "react";
import DatePicker from "react-datepicker";

// Dynamically import EntityDropdown to avoid SSR issues
const EntityDropdown = dynamic(() => import("@/components/EntityDropdown"), {
    ssr: false,
    loading: () => <EntityDropdownLoader />
});

type SchedulePageProps = {
    selectedLineId: number;
    selectedLineNumber: string;
    selectedDirection: number;
}

export default function SchedulePage({
    selectedLineId,
    selectedLineNumber,
    selectedDirection
}: SchedulePageProps) {
    const router = useRouter();

    const [selectedDate, setSelectedDate] = useState<Date | null>(new Date());
    const [timetable, setTimetable] = useState<TimetableModel | null>(null);
    const [highlightedStop, setHighlightedStop] = useState<number | null>(null);
    const [highlightedCourse, setHighlightedCourse] = useState<number | null>(null);

    const fetchTimetable = useCallback(async () => {
        setTimetable(null);
        if (selectedLineId && selectedDirection && selectedDate) {
            try {
                const dateString = selectedDate.toISOString().split('T')[0];
                const data = await TimetablesService.get(selectedLineId, selectedDirection, dateString);
                setTimetable(data);
            } catch (error) {
                console.error('Failed to fetch timetable:', error);
            }
        }
    }, [selectedLineId, selectedDirection, selectedDate]);

    useEffect(() => {
        fetchTimetable();
    }, [fetchTimetable]);

    return (
        <div className="h-full bg-background flex flex-col">
            <div className="w-full p-6">
                {/* Controls */}
                <div className="flex gap-2 mb-8 max-w-xl">
                    {/* Line Selector */}
                    <div className="flex-1">
                        <label className="label-standard">
                            Bus Line
                        </label>
                        <EntityDropdown
                            value={selectedLineId}
                            onChange={(e) => router.push(`/schedule?lineNumber=${e?.data.lineNumber}&direction=${selectedDirection}`)}
                            placeholder="Select..."
                            url="/api/bus-lines-light"
                            sorts={[{ field: "LineNumber", dir: "asc" }]}
                            parseData={(data: PageResult<BusLineLightModel>) =>
                                data.items.map((item) => {
                                    return {
                                        value: item.id,
                                        label: item.lineNumber,
                                        data: item
                                    };
                                })
                            }
                            isClearable={false}
                        />
                    </div>

                    {/* Direction Buttons */}
                    <div className="shrink-0">
                        <DirectionSelector
                            selectedDirection={selectedDirection}
                            onDirectionChange={(direction) => router.push(`/schedule?lineNumber=${selectedLineNumber}&direction=${direction}`)}
                        />
                    </div>

                    {/* Date Picker */}
                    <div className="flex-1">
                        <label className="label-standard">
                            Date
                        </label>
                        <DatePicker
                            selected={selectedDate}
                            onChange={setSelectedDate}
                            dateFormat={"dd/MM/yyyy"}
                            placeholderText="Select..."
                        />
                    </div>
                </div>

                {/* Schedule Table */}
                {!selectedLineId || !selectedDirection || !selectedDate ?
                    <div className="text-center py-12">
                        <p className="text-text-muted">Please select a bus line, direction and date to view the timetable.</p>
                    </div>
                    :
                    !timetable && (
                        <div className="text-center py-12">
                            <p className="text-text-muted">No timetable found.</p>
                        </div>
                    )}

                {timetable && (
                    <div className={`bg-background-secondary rounded-lg border border-border overflow-auto`}>
                        <table className="w-full text-sm border-spacing-0">
                            <thead>
                                <tr className="bg-background-secondary">
                                    <th className="text-left py-3 px-4 font-semibold text-foreground border-r border-b border-border sticky left-0 bg-background-secondary z-10">
                                        Bus Stop
                                    </th>
                                    {timetable?.schedule.scheduleCourses.map(course => (
                                        <th
                                            key={course.id}
                                            className={`text-center py-3 px-2 font-semibold text-foreground border-r border-b last:border-r-0 border-border whitespace-nowrap transition-colors ${highlightedCourse === course.id
                                                ? 'bg-background-light'
                                                : ''
                                                }`}
                                        >
                                            {DateTime.fromISO(course.departureTime).toFormat("HH:mm")}
                                        </th>
                                    ))}
                                </tr>
                            </thead>
                            <tbody>
                                {timetable?.timetableRows.length === 0 ? (
                                    <tr>
                                        <td colSpan={timetable.schedule.scheduleCourses.length + 1} className="py-8 px-4 text-center text-text-muted">
                                            No stops found for this route.
                                        </td>
                                    </tr>
                                ) : (
                                    timetable?.timetableRows.map((rs, idx) => (
                                        <tr
                                            key={rs.busStop.id}
                                            className={`border-b last:border-b-0 border-border transition-colors ${highlightedStop === rs.busStop.id ? 'bg-background-light' : ''}`}
                                        >
                                            <td
                                                className={`py-3 px-4 font-medium text-foreground border-r border-border sticky left-0 z-10 transition-colors ${highlightedStop === rs.busStop.id
                                                    ? 'bg-background-light'
                                                    : 'bg-background-secondary'
                                                    }`}
                                            >
                                                <div className="flex items-center gap-2">
                                                    <span className="shrink-0 w-6 h-6 bg-primary rounded-full flex items-center justify-center text-white text-xs font-bold">
                                                        {idx + 1}
                                                    </span>
                                                    <span className="truncate">{rs.busStop.name || 'Unknown'}</span>
                                                </div>
                                            </td>
                                            {timetable.schedule.scheduleCourses.map(course => (
                                                <td
                                                    key={`${rs.busStop.id}-${course.id}`}
                                                    className={`text-center py-3 px-2 text-foreground border-r last:border-r-0 border-border transition-colors cursor-pointer ${highlightedCourse === course.id
                                                        ? 'bg-background-light'
                                                        : ''
                                                        }`}
                                                    onClick={() => { setHighlightedStop(rs.busStop.id); setHighlightedCourse(course.id); }}
                                                >
                                                    {rs.timesByVariant[course.id] ? DateTime.fromISO(rs.timesByVariant[course.id]!).toFormat("HH:mm") : ''}
                                                </td>
                                            ))}
                                        </tr>
                                    ))
                                )}
                            </tbody>
                        </table>
                    </div>
                )}
            </div>
        </div>
    );
}