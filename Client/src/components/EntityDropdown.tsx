// UNDER DEVELOPMENT!!!

import React, { useState, useEffect, useCallback, useImperativeHandle, forwardRef } from "react";
import Select from "react-select";
import CreatableSelect from "react-select/creatable";
import { authorisedGetRequest, getQueryString } from "@/helpers/Request";
import string from "@/helpers/StringUtility";

interface OptionType {
    value: any;
    label: string;
    data?: any;
}

interface EntityDropdownProps {
    parseData: (data: any) => OptionType[];
    url: string;
    filter?: any;
    sorts?: any;
    value?: any;
    autoBind?: boolean;
    doNotCacheValue?: boolean;
    onChange?: (item: OptionType | null) => void;
    onDataBound?: (data: OptionType[]) => void;
    onCascade?: (item: OptionType | null) => void;
    onOpen?: () => void;
    onCreate?: (inputValue: string) => void;
    creatable?: boolean;
    placeholder?: string;
    formatCreateLabel?: (inputValue: string) => string;
    isDisabled?: boolean;
    required?: boolean;
    children?: React.ReactNode;
}

export interface EntityDropdownRef {
    reload: () => Promise<void>;
}

const EntityDropdown = forwardRef<EntityDropdownRef, EntityDropdownProps>((props, ref) => {
    const {
        parseData,
        url,
        filter,
        sorts,
        value,
        autoBind,
        doNotCacheValue,
        onChange,
        onDataBound,
        onCascade,
        onOpen,
        onCreate,
        creatable,
        placeholder,
        formatCreateLabel,
        isDisabled,
        required,
        children,
    } = props;

    const [selectedItem, setSelectedItem] = useState<OptionType | null>(null);
    const [data, setData] = useState<OptionType[]>([]);
    const [isDataLoading, setIsDataLoading] = useState(false);
    const [isDataLoaded, setIsDataLoaded] = useState(false);

    // Validate props on mount
    useEffect(() => {
        if (typeof parseData !== "function") {
            throw new Error(
                "'parseData' is not defined. It should be a function, which returns an array of objects, where each object has at least properties 'value' and 'label'."
            );
        }
        if (string.isNullOrEmpty(url)) {
            throw new Error("'url' is not defined. It should be a non-empty string.");
        }
    }, [parseData, url]);

    const readData = useCallback(async () => {
        if (!isDataLoaded) {
            setIsDataLoading(true);

            const responseData = await authorisedGetRequest(
                process.env.NEXT_PUBLIC_WEB_API_BASE_URL + `${url}${getQueryString(filter, sorts)}`,
                (result) => {
                    return result;
                }
            );
            const parsedData = parseData(responseData);

            setData(parsedData);
            setIsDataLoaded(true);
            setIsDataLoading(false);

            if (typeof onDataBound === "function") {
                onDataBound(parsedData);
            }
        }
    }, [isDataLoaded, url, filter, sorts, parseData, onDataBound]);

    const handleChange = useCallback((item: OptionType | null) => {
        // This check is needed, because there is a bug in "Select" component.
        // When you have cleared the value and keep pressing "Backspace", the onChange event is raised.
        setSelectedItem((prevSelectedItem) => {
            const hasChange = prevSelectedItem !== item;

            if (hasChange) {
                if (!doNotCacheValue) {
                    // Update state
                }

                if (typeof onChange === "function") {
                    onChange(item);
                }

                return doNotCacheValue ? prevSelectedItem : item;
            }

            return prevSelectedItem;
        });
    }, [doNotCacheValue, onChange]);

    const filterOption = useCallback(
        ({ label }: OptionType, searchString: string) => {
            return (!string.isNullOrEmpty(label) ? label : "").toLowerCase().includes(searchString.toLowerCase());
        },
        []
    );

    const initSelectedItem = useCallback(async () => {
        if (value || (!!autoBind && !isDataLoaded)) {
            await readData();

            if (!doNotCacheValue) {
                const foundItem = data.find((x) => x.value === value);
                if (foundItem) {
                    setSelectedItem(foundItem);
                }
            }
        } else {
            if (!doNotCacheValue) {
                setSelectedItem(null);
            }
        }
        if (typeof onCascade === "function") {
            onCascade(selectedItem);
        }
    }, [value, autoBind, isDataLoaded, doNotCacheValue, data, readData, onCascade, selectedItem]);

    const reload = useCallback(async () => {
        setIsDataLoaded(false);
        await initSelectedItem();
    }, [initSelectedItem]);

    // Expose reload method via ref
    useImperativeHandle(ref, () => ({
        reload,
    }));

    // componentDidMount equivalent
    useEffect(() => {
        initSelectedItem();
        // eslint-disable-next-line react-hooks/exhaustive-deps
    }, []);

    // componentDidUpdate equivalent - watch for value changes
    useEffect(() => {
        initSelectedItem();
        // eslint-disable-next-line react-hooks/exhaustive-deps
    }, [value]);

    const handleMenuOpen = useCallback(() => {
        readData();
        if (typeof onOpen === "function") {
            onOpen();
        }
    }, [readData, onOpen]);

    return (
        <div className="react-select-dropdown">
            {creatable ? (
                <CreatableSelect
                    options={data}
                    value={selectedItem}
                    onChange={handleChange}
                    onMenuOpen={readData}
                    isLoading={isDataLoading}
                    placeholder={placeholder}
                    filterOption={filterOption}
                    isClearable
                    createOptionPosition="first"
                    formatCreateLabel={formatCreateLabel}
                    onCreateOption={onCreate}
                    isDisabled={isDisabled}
                    classNamePrefix="react-select-dropdown"
                />
            ) : (
                <Select
                    options={data}
                    value={selectedItem}
                    onChange={handleChange}
                    onMenuOpen={handleMenuOpen}
                    isLoading={isDataLoading}
                    placeholder={placeholder}
                    filterOption={filterOption}
                    isClearable
                    isDisabled={isDisabled}
                    classNamePrefix="react-select-dropdown"
                />
            )}

            <input
                type="text"
                value={selectedItem ? selectedItem.value : ""}
                onChange={() => { }}
                required={required}
                className="hidden"
            />
            {children}
        </div>
    );
});

EntityDropdown.displayName = "EntityDropdown";

export default EntityDropdown;
