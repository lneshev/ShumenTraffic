import { authorisedGetRequest, getQueryString, getRequest } from '@/helpers/Request';
import string from '@/helpers/StringUtility';
import env from "@/services/EnvService";
import Id from '@/types/common/Id';
import Sort from '@/types/common/Sort';
import { useEffect, useRef, useState } from 'react';
import Select, { FilterOptionOption } from 'react-select';

interface EntityMultiselectProps<TId extends Id> {
  parseData: (data: any) => OptionType<TId>[];
  url: string;
  isAuthorizedRequest?: boolean;
  filter?: Record<string, any>;
  sorts?: Sort[];
  value?: TId[];
  autoBind?: boolean;
  onRequestStart?: () => void;
  onDataBound?: (data: OptionType<TId>[]) => void;
  onRequestEnd?: () => void;
  onChange?: (options: OptionType<TId>[]) => void;
  onOpen?: () => void;
  placeholder?: string;
  isDisabled?: boolean;
  required?: boolean;
  children?: React.ReactNode;
}

interface OptionType<TId extends Id> {
  value: TId;
  label: string;
  data?: any;
}

export default function EntityMultiselect<TId extends Id>({
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
  isDisabled = false,
  required = false,
  children
}: EntityMultiselectProps<TId>) {
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
      const fullUrl = `${env.getPublicWebApiBaseUrl()}${url}${queryString}`;

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

  const handleChange = (newValue: readonly OptionType<TId>[] | null) => {
    const items = newValue ? [...newValue] : [];
    onChange?.(items);
  };

  const filterOption = ({ label }: FilterOptionOption<OptionType<TId>>, searchString: string) => {
    return (!string.isNullOrEmpty(label) ? label : "").toLowerCase().includes(searchString.toLowerCase());
  };

  const selectedOptions = value != null ? options.filter(option => value.includes(option.value)) : null;

  const handleMenuOpen = async () => {
    // Fetch options when menu opens for the first time (if not already fetched)
    if (!hasFetchedRef.current) {
      await fetchOptions();
    }
    onOpen?.();
  };

  useEffect(() => {
    // Fetch immediately if autoBind is true or if there are preselected values
    if (autoBind || (value && value.length > 0)) {
      fetchOptions();
    }
  }, []); // Only run on mount

  return (
    <div
      className="react-select-dropdown"
      onClick={(e) => e.stopPropagation()} // Fixes the leaflet's marker popup closing when selecting an option from this dropdown
    >
      <Select
        isMulti
        options={options}
        value={selectedOptions}
        onChange={handleChange}
        onMenuOpen={handleMenuOpen}
        isLoading={isLoading}
        placeholder={placeholder}
        noOptionsMessage={() => !!error ? (<span className='text-red-600 dark:text-red-400'>Error loading options</span>) : "No options"}
        filterOption={filterOption}
        isClearable={true}
        closeMenuOnSelect={false}
        isDisabled={isDisabled}
        required={required}
        classNamePrefix="react-select-dropdown"
      />
      {children}
    </div>
  );
}