import EnumUtility from "@/helpers/EnumUtility";
import { getEnumsQueryString, getRequest } from "@/helpers/Request";
import string from "@/helpers/StringUtility";
import EnumModel from "@/types/common/EnumModel";
import { useEffect, useRef, useState } from "react";
import Select, { FilterOptionOption, MultiValue } from "react-select";

interface FlagsEnumMultiselectProps {
    enumName: string;
    exactEnumValues?: number[];
    sortByText?: boolean;
    value?: number;
    autoBind?: boolean;
    onDataBound?: (data: OptionType[]) => void;
    onChange?: (value: number | null) => void;
    onOpen?: () => void;
    placeholder?: string;
    isClearable?: boolean;
    isDisabled?: boolean;
    required?: boolean;
}

interface OptionType {
    value: number;
    label: string;
}

export default function FlagsEnumMultiselect({
    enumName,
    exactEnumValues = [],
    sortByText = false,
    value,
    autoBind = false,
    onDataBound,
    onChange,
    onOpen,
    placeholder = "Select...",
    isClearable = true,
    isDisabled = false,
    required = false
}: FlagsEnumMultiselectProps) {
    const [options, setOptions] = useState<OptionType[]>([]);
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

            const fullUrl = `${process.env.NEXT_PUBLIC_WEB_API_BASE_URL}/api/enums/${enumName}${getEnumsQueryString(exactEnumValues, sortByText)}`;

            const data = await getRequest(fullUrl, (result) => result) as EnumModel[];

            const parsedData = data.map((item, i) => {
                return {
                    value: item.value,
                    label: item.text,
                };
            });
            setOptions(parsedData);
            onDataBound?.(parsedData);
        } catch (err) {
            setError('Error loading options');
            hasFetchedRef.current = false; // Allow retry on error
        } finally {
            setIsLoading(false);
        }
    };

    const handleChange = (items: MultiValue<OptionType> | null) => {
        onChange?.(EnumUtility.arrayToFlags(items?.map(x => x.value) || []));
    };

    const filterOption = ({ label }: FilterOptionOption<OptionType>, searchString: string) => {
        return (!string.isNullOrEmpty(label) ? label : "").toLowerCase().includes(searchString.toLowerCase());
    };

    const selectedOptions = value != null ? options.filter(option => EnumUtility.flagsToArray(value).includes(option.value)) : null;

    const handleMenuOpen = async () => {
        // Fetch options when menu opens for the first time (if not already fetched)
        if (!hasFetchedRef.current) {
            await fetchOptions();
        }
        onOpen?.();
    };

    useEffect(() => {
        // Fetch immediately if autoBind is true or if there are preselected values
        if (autoBind || !!value || value === 0) {
            fetchOptions();
        }
    }, []); // Only run on mount

    return (
        <div
            className="react-select-dropdown"
            onClick={(e) => e.stopPropagation()} // Fixes the leaflet's marker popup closing when selecting an option from this dropdown
        >
            <Select
                isMulti={true}
                options={options}
                value={selectedOptions}
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
        </div>
    );
}