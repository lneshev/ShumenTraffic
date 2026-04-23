import { env } from 'next-runtime-env';

export default {
    getPublicWebApiBaseUrl() {
        return env('NEXT_PUBLIC_WEB_API_BASE_URL');
    }
}