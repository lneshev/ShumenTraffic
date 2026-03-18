import { DateTime } from "luxon";

export default function Article({
    title,
    date,
    children
}: {
    title: string,
    date: string,
    children: React.ReactNode
}) {
    return (
        <article className="p-6 bg-gray-50 dark:bg-slate-900 rounded-lg border border-gray-200 dark:border-slate-700">
            <div className="flex justify-between items-start mb-3">
                <h3 className="text-lg font-semibold text-gray-900 dark:text-white">
                    {title}
                </h3>
                <span className="text-sm text-gray-500 dark:text-gray-400">
                    {DateTime.fromISO(date).toLocaleString(DateTime.DATE_SHORT, { locale: 'bg-BG' })}
                </span>
            </div>
            <p className="text-gray-600 dark:text-gray-400">
                {children}
            </p>
        </article>
    );
}