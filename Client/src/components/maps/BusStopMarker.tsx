import BusStopMapMode from "@/enums/BusStopMapMode";
import BusStopModel from "@/types/BusStopModel";
import PageResult from "@/types/common/PageResult";
import ZoneModel from "@/types/ZoneModel";
import { DivIcon, Icon, LeafletEventHandlerFnMap } from "leaflet";
import { RefObject } from "react";
import { Marker, Popup, Tooltip } from "react-leaflet";
import EntityDropdown from "../EntityDropdown";

interface BusStopMarkerProps {
    mode: BusStopMapMode;
    busStop: BusStopModel;
    position: [number, number];
    icon: Icon | DivIcon;
    draggable: boolean;
    eventHandlers?: LeafletEventHandlerFnMap;
    tooltip?: React.ReactElement<typeof Tooltip>;
    ref?: RefObject<L.Marker | null>;
    onBusStopNameChange?: (stop: BusStopModel, newName: string) => void;
    onBusStopZoneIdChange?: (stop: BusStopModel, newZoneId: number) => void;
    onButtonSaveClick?: (stop: BusStopModel) => void;
    onButtonDeleteClick?: (stop: BusStopModel) => void;
    onButtonCancelClick?: (stop: BusStopModel) => void;
}

export function BusStopMarker({
    mode,
    busStop,
    position,
    icon,
    draggable,
    eventHandlers,
    tooltip,
    ref,
    onBusStopNameChange,
    onBusStopZoneIdChange,
    onButtonSaveClick,
    onButtonDeleteClick,
    onButtonCancelClick
}: BusStopMarkerProps) {
    return (
        <Marker
            data={busStop}
            position={position}
            icon={icon}
            draggable={draggable}
            eventHandlers={eventHandlers}
            ref={ref}
        >
            {tooltip}
            <Popup>
                <form onSubmit={(e) => { e.preventDefault(); onButtonSaveClick?.(busStop); }} className="text-sm">
                    <div className="font-semibold text-gray-900">
                        {mode === BusStopMapMode.View && (
                            <>
                                {busStop.name}<br />
                                {busStop.zoneName}
                            </>
                        )}
                        {mode === BusStopMapMode.Edit && (
                            <>
                                <input
                                    type="text"
                                    name="busStopName"
                                    value={busStop.name}
                                    onChange={(e) => onBusStopNameChange?.(busStop, e.target.value)}
                                    className="px-4 py-2 border border-gray-300 dark:border-slate-600 rounded-lg bg-white dark:bg-slate-800 text-gray-900 dark:text-white"
                                    required
                                    maxLength={255}
                                />
                                <EntityDropdown
                                    value={busStop.zoneId}
                                    onChange={(e) => onBusStopZoneIdChange?.(busStop, e ? e.value : 0)}
                                    placeholder="Select..."
                                    url="/api/zones"
                                    sorts={[
                                        { field: "name", dir: "asc" }
                                    ]}
                                    parseData={(data: PageResult<ZoneModel>) =>
                                        data.items.map((item, i) => {
                                            return {
                                                value: item.id,
                                                label: item.name
                                            };
                                        })
                                    }
                                    required
                                />
                            </>
                        )}
                    </div>
                    <p className="text-gray-600 text-xs">
                        {busStop.location.latitude}, {busStop.location.longitude}
                    </p>
                    {mode == BusStopMapMode.Edit && (
                        <div>
                            <button
                                type="submit"
                                className="mb-6 px-4 py-2 bg-blue-600 hover:bg-blue-700 text-white font-semibold rounded-lg transition-colors"
                            >
                                Save
                            </button>
                            {busStop.id > 0 && (
                                <button
                                    type="button"
                                    onClick={() => onButtonDeleteClick?.(busStop)}
                                    className="mb-6 px-4 py-2 bg-red-600 hover:bg-red-700 text-white font-semibold rounded-lg transition-colors"
                                >
                                    Delete
                                </button>
                            )}
                            <button
                                type="button"
                                onClick={() => onButtonCancelClick?.(busStop)}
                                className="mb-6 px-4 py-2 bg-blue-600 hover:bg-blue-700 text-white font-semibold rounded-lg transition-colors"
                            >
                                Cancel
                            </button>
                        </div>
                    )}
                </form>
            </Popup>
        </Marker>
    );
}