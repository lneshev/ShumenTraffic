import string from "@/helpers/StringUtility";
import { ApiError } from "@/lib/api";
import ApiResponse from "@/types/common/ApiResponse";
import { enhanceGeoJSON } from "@/types/common/GeoJSON";
import Sort from "@/types/common/Sort";

export async function getRequest(url: string, responseHandler: (value: any) => any, keepalive = false) {
    return await makeRequest("GET", url, null, responseHandler, false, keepalive);
}

export async function postRequest(url: string, body: object | null, responseHandler: (value: any) => any, keepalive = false) {
    return await makeRequest("POST", url, body, responseHandler, false, keepalive);
}

export async function putRequest(url: string, body: object, responseHandler: (value: any) => any, keepalive = false) {
    return await makeRequest("PUT", url, body, responseHandler, false, keepalive);
}

export async function patchRequest(url: string, body: object, responseHandler: (value: any) => any, keepalive = false) {
    return await makeRequest("PATCH", url, body, responseHandler, false, keepalive);
}

export async function deleteRequest(url: string, responseHandler: (value: any) => any, keepalive = false) {
    return await makeRequest("DELETE", url, null, responseHandler, false, keepalive);
}

export async function authorisedGetRequest(url: string, responseHandler: (value: any) => any, keepalive = false) {
    return await makeRequest("GET", url, null, responseHandler, true, keepalive);
}

export async function authorisedPostRequest(url: string, body: object | null, responseHandler: (value: any) => any, keepalive = false) {
    return await makeRequest("POST", url, body, responseHandler, true, keepalive);
}

export async function authorisedPutRequest(url: string, body: object, responseHandler: (value: any) => any, keepalive = false) {
    return await makeRequest("PUT", url, body, responseHandler, true, keepalive);
}

export async function authorisedPatchRequest(url: string, body: object, responseHandler: (value: any) => any, keepalive = false) {
    return await makeRequest("PATCH", url, body, responseHandler, true, keepalive);
}

export async function authorisedDeleteRequest(url: string, responseHandler: (value: any) => any, keepalive = false) {
    return await makeRequest("DELETE", url, null, responseHandler, true, keepalive);
}

async function makeRequest(method: string, url: string, body: object | null, responseHandler: (value: any) => any, isAuthorisedRequest: boolean, keepalive = false) {
    const headers = new Headers({
        "Content-Type": "application/json",
        Accept: "application/json",
    });

    let requestOptions: RequestInit;

    if (method === "POST" || method === "PUT" || method === "PATCH") {
        requestOptions = {
            method: method,
            headers: headers,
            credentials: "include",
            keepalive: keepalive,
            body: body !== null && body !== undefined ? JSON.stringify(body) : undefined
        };
    } else {
        requestOptions = {
            method: method,
            headers: headers,
            credentials: "include",
            keepalive: keepalive
        };
    }

    return await fetch(url, requestOptions)
        .then(async (response) => {
            const text = await response.text();
            const result = !string.isNullOrEmpty(text) ? (JSON.parse(text) as ApiResponse<any>) : null;

            return new Promise(function (resolve, reject) {
                if (response.ok && result?.success) {
                    resolve(enhanceGeoJSON(result?.data));
                } else {
                    reject(new ApiError(result?.message || `Request failed with status ${response.status}`, response.status, result?.errors));
                }
            });
        })
        .then(responseHandler);
}

export function getQueryString(filter: Record<string, any> = {}, sorts: Sort[] = [], pageNumber?: number, pageSize?: number) {
    // Prepare filter
    let filterString = "";

    for (var property in filter) {
        const value = filter[property];
        if (Array.isArray(value)) {
            for (let i = 0; i < value.length; i++) {
                filterString += `${property}[${i}]=${encodeURIComponent(value[i])}&`;
            }
        } else if (!string.isNullOrEmpty(value)) {
            filterString += `${property}=${encodeURIComponent(value)}&`;
        }
    }

    if (!string.isNullOrEmpty(filterString)) {
        filterString = filterString.slice(0, -1);
    }

    // Prepare sorts
    let sortString = "";

    for (let i = 0; i < sorts.length; i++) {
        const sort = sorts[i];
        sortString += `sorts[${i}].field=${encodeURIComponent(sort.field)}&`;
        sortString += `sorts[${i}].dir=${encodeURIComponent(sort.dir)}&`;
    }

    if (!string.isNullOrEmpty(sortString)) {
        sortString = sortString.slice(0, -1);
    }

    // Prepare page number
    let pageNumberString = "";

    if (pageNumber !== undefined) {
        pageNumberString = `pageNumber=${encodeURIComponent(pageNumber)}`;
    }

    // Prepare page size
    let pageSizeString = "";

    if (pageSize !== undefined) {
        pageSizeString = `pageSize=${encodeURIComponent(pageSize)}`;
    }

    // Prepare result string
    let result = [filterString, sortString, pageNumberString, pageSizeString].filter(Boolean).join("&");

    if (!string.isNullOrEmpty(result)) {
        result = "?" + result;
    }

    return result;
}