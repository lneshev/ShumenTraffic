import { env } from 'next-runtime-env';

export default {
    getPublicWebApiBaseUrl() {
        if (typeof window !== 'undefined') {
            return env('NEXT_PUBLIC_WEB_API_BASE_URL');
        }

        return process.env.NEXT_PUBLIC_WEB_API_BASE_URL;
    },
    getInternalWebApiBaseUrl() {
        if (typeof window !== 'undefined') {
            return env('INTERNAL_WEB_API_BASE_URL');
        }

        return process.env.INTERNAL_WEB_API_BASE_URL;
    }
}