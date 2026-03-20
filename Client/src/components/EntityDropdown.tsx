import { authorisedGetRequest, getQueryString, getRequest } from "@/helpers/Request";
import string from "@/helpers/StringUtility";
import Id from "@/types/common/Id";
import Sort from "@/types/common/Sort";
import React, { useEffect, useRef, useState } from "react";
import Select, { FilterOptionOption } from "react-select";
import CreatableSelect from "react-select/creatable";

interface EntityDropdownProps<TId extends Id> {
    parseData: (data: any) => OptionType<TId>[];
    url: string;
    isAuthorizedRequest?: boolean;
    filter?: Record<string, any>;
    sorts?: Sort[];
    value?: TId;
    autoBind?: boolean;
    onRequestStart?: () => void;
    onDataBound?: (data: OptionType<TId>[]) => void;
    onRequestEnd?: () => void;
    onChange?: (item: OptionType<TId> | null) => void;
    onOpen?: () => void;
    placeholder?: string;
    isClearable?: boolean;
    isDisabled?: boolean;
    required?: boolean;
    children?: React.ReactNode;
    // Creatable props
    creatable?: boolean;
    formatCreateLabel?: (inputValue: string) => string;
    onCreate?: (inputValue: string) => void;
}

interface OptionType<TId extends Id> {
    value: TId;
    label: string;
    data?: any;
}

export default function EntityDropdown<TId extends Id>({
    parseData,
    url,
    isAuthorizedRequest = false,
    filter,
    sorts = [],
    value,
    autoBind = false,
    onRequestStart,
    onDataBound,
    onRequestEnd,
    onChange,
    onOpen,
    placeholder = "Select...",
    isClearable = true,
    isDisabled = false,
    required = false,
    children,
    // Creatable props
    creatable = false,
    formatCreateLabel,
    onCreate,
}: EntityDropdownProps<TId>) {
    const [options, setOptions] = useState<OptionType<TId>[]>([]);
    const [isLoading, setIsLoading] = useState(false);
    const [error, setError] = useState<string | null>(null);
    const hasFetchedRef = useRef(false);

    const fetchOptions = async () => {
        if (hasFetchedRef.current) {
            return;
        }

        try {
            setIsLoading(true);
            setError(null);
            hasFetchedRef.current = true;
            onRequestStart?.();

            const queryString = getQueryString(filter, sorts);
            const fullUrl = `${process.env.NEXT_PUBLIC_WEB_API_BASE_URL}${url}${queryString}`;

            let data;
            if (isAuthorizedRequest) {
                data = await authorisedGetRequest(fullUrl, (result) => result);
            }
            else {
                data = await getRequest(fullUrl, (result) => result);
            }
            const parsedData = parseData(data);
            setOptions(parsedData);
            onDataBound?.(parsedData);
        } catch (err) {
            setError('Error loading options');
            hasFetchedRef.current = false; // Allow retry on error
        } finally {
            setIsLoading(false);
            onRequestEnd?.();
        }
    };

    const handleChange = (item: OptionType<TId> | null) => {
        onChange?.(item);
    };

    const filterOption = ({ label }: FilterOptionOption<OptionType<TId>>, searchString: string) => {
        return (!string.isNullOrEmpty(label) ? label : "").toLowerCase().includes(searchString.toLowerCase());
    };

    const selectedOption = value ? options.find(option => value === option.value) : null;

    const handleMenuOpen = async () => {
        // Fetch options when menu opens for the first time (if not already fetched)
        if (!hasFetchedRef.current) {
            await fetchOptions();
        }
        onOpen?.();
    };

    useEffect(() => {
        // Fetch immediately if autoBind is true or if there are preselected values
        if (autoBind || !!value) {
            fetchOptions();
        }
    }, [autoBind, value]);

    return (
        <div
            className="react-select-dropdown"
            onClick={(e) => e.stopPropagation()} // Fixes the leaflet's marker popup closing when selecting an option from this dropdown
        >
            {creatable ? (
                <CreatableSelect
                    options={options}
                    value={selectedOption}
                    onChange={handleChange}
                    onMenuOpen={handleMenuOpen}
                    isLoading={isLoading}
                    placeholder={placeholder}
                    noOptionsMessage={() => !!error ? (<span className='text-red-600 dark:text-red-400'>Error loading options</span>) : "No options"}
                    filterOption={filterOption}
                    isClearable={isClearable}
                    isDisabled={isDisabled}
                    required={required}
                    classNamePrefix="react-select-dropdown"
                    // Createable props
                    createOptionPosition="first"
                    formatCreateLabel={formatCreateLabel}
                    onCreateOption={onCreate}
                />
            ) : (
                <Select
                    options={options}
                    value={selectedOption}
                    onChange={handleChange}
                    onMenuOpen={handleMenuOpen}
                    isLoading={isLoading}
                    placeholder={placeholder}
                    noOptionsMessage={() => !!error ? (<span className='text-red-600 dark:text-red-400'>Error loading options</span>) : "No options"}
                    filterOption={filterOption}
                    isClearable={isClearable}
                    isDisabled={isDisabled}
                    required={required}
                    classNamePrefix="react-select-dropdown"
                />
            )}
            {children}
        </div>
    );
};

export const EntityDropdownLoader = () => {
    return (
        <div className="react-select-dropdown animate-pulse h-10 w-full bg-white dark:bg-slate-800 rounded-lg border border-gray-200 dark:border-slate-700"></div>
    );
};