import { getEnumsQueryString, getRequest } from "@/helpers/Request";
import string from "@/helpers/StringUtility";
import env from "@/services/EnvService";
import EnumModel from "@/types/common/EnumModel";
import { useEffect, useRef, useState } from "react";
import Select, { FilterOptionOption } from "react-select";

interface EnumDropdownProps {
    enumName: string;
    exactEnumValues?: number[];
    sortByText?: boolean;
    value?: number;
    autoBind?: boolean;
    onDataBound?: (data: OptionType[]) => void;
    onChange?: (item: OptionType | null) => void;
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

export default function EnumDropdown({
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
}: EnumDropdownProps) {
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

            const fullUrl = `${env.getPublicWebApiBaseUrl()}/api/enums/${enumName}${getEnumsQueryString(exactEnumValues, sortByText)}`;

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

    const handleChange = (item: OptionType | null) => {
        onChange?.(item);
    };

    const filterOption = ({ label }: FilterOptionOption<OptionType>, searchString: string) => {
        return (!string.isNullOrEmpty(label) ? label : "").toLowerCase().includes(searchString.toLowerCase());
    };

    const selectedOption = value != null ? options.find(option => value === option.value) : null;

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
        </div>
    );
}