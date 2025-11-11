import { authorisedGetRequest, getQueryString } from "@/helpers/Request";
import { Ref, useCallback, useEffect, useImperativeHandle, useState } from "react";
import Select, { ActionMeta, MultiValue } from "react-select";
import string from "@/helpers/StringUtility";

interface EntityMultiselectProps {
    parseData: (data: any) => OptionType[];
    url: string;
    filter?: Record<string, any>;
    sorts?: { field: string, dir: number | "asc" | "desc" }[];
    value?: any[];
    autoBind?: boolean;
    onRequestStart?: () => void;
    onDataBound?: (data: OptionType[]) => void;
    onRequestEnd?: () => void;
    onChange?: (items: OptionType[]) => void;
    placeholder?: string;
    isDisabled?: boolean;
    required?: boolean;
    children?: React.ReactNode;
    ref?: Ref<unknown> | undefined;
}

interface OptionType {
    value: any;
    label: string;
    data?: any;
}

const EntityMultiselect = (props: EntityMultiselectProps) => {
    const [selectedItems, setSelectedItems] = useState<OptionType[]>([]);
    const [data, setData] = useState<OptionType[]>([]);
    const [isDataLoading, setIsDataLoading] = useState(false);
    const [isDataLoaded, setIsDataLoaded] = useState(false);

    useEffect(() => {
        if (typeof props.parseData !== "function") {
            throw new Error("'parseData' is not defined. It should be a function, which returns an array of objects, where each object has at least properties 'value' and 'label'.");
        }
        if (string.isNullOrEmpty(props.url)) {
            throw new Error("'url' is not defined. It should be a non-empty string.");
        }
    }, [props.url]);

    const readData = async () => {
        if (!isDataLoaded) {
            try {
                setIsDataLoading(true);
                props.onRequestStart?.();

                const data = await authorisedGetRequest(process.env.NEXT_PUBLIC_WEB_API_BASE_URL + `${props.url}${getQueryString(props.filter, props.sorts)}`, result => {
                    return result;
                });
                const parsedData = props.parseData(data);

                setData(parsedData);
                setIsDataLoaded(true);
                setIsDataLoading(false);
                props.onDataBound?.(parsedData);
            }
            finally {
                props.onRequestEnd?.();
            }
        }
    };

    const handleChange = useCallback((e: MultiValue<OptionType>, actionMeta: ActionMeta<OptionType>) => {
        let items = e ? e as OptionType[] : [];

        // This check is needed, because there is a bug in "Select" component (found in version 3).
        // When you have cleared the value and keep pressing "Backspace", the onChange event is raised.
        const hasChange = selectedItems.length !== 0 || items.length !== 0;

        if (hasChange) {
            setSelectedItems(items);
            props.onChange?.(items);
        }
    }, [selectedItems]);

    const filterOption = ({ label }: OptionType, searchString: string) => {
        return (!string.isNullOrEmpty(label) ? label : "").toLowerCase().includes(searchString.toLowerCase());
    };

    const initSelectedItems = useCallback(async () => {
        if ((props.value && props.value.length > 0) || (!!props.autoBind && !isDataLoaded)) {
            await readData();
            let selectedItems = [];
            if (props.value) {
                for (let i = 0; i < props.value.length; i++) {
                    const item = props.value[i];
                    const foundItem = data.find(x => x.value === item);
                    if (typeof foundItem !== "undefined" && foundItem !== null) {
                        selectedItems.push(foundItem);
                    }
                }
            }
            setSelectedItems(selectedItems);
        }
        else {
            setSelectedItems([]);
        }
    }, [props.value, props.autoBind, isDataLoaded, data, readData]);

    const reload = useCallback(async () => {
        setIsDataLoaded(false);
        await initSelectedItems();
    }, [initSelectedItems]);

    useImperativeHandle(props.ref, () => ({
        reload
    }));

    // componentDidMount and componentDidUpdate equivalent - watch for value changes
    useEffect(() => {
        initSelectedItems();
    }, [props.value, data]);

    return (
        <div className="react-select-dropdown">
            <Select
                isMulti
                options={data}
                value={selectedItems}
                onChange={handleChange}
                onMenuOpen={readData}
                isLoading={isDataLoading}
                placeholder={props.placeholder}
                filterOption={filterOption}
                isClearable
                closeMenuOnSelect={false}
                isDisabled={props.isDisabled}
                classNamePrefix="react-select-dropdown"
                required={props.required}
            />
            {props.children}
        </div>
    );
};

export default EntityMultiselect;