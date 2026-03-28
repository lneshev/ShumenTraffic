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
        <article className="p-6 bg-background-secondary rounded-lg border border-border">
            <div className="flex justify-between items-start mb-3">
                <h3 className="text-lg font-semibold">
                    {title}
                </h3>
                <span className="text-sm">
                    {DateTime.fromISO(date).toLocaleString(DateTime.DATE_SHORT, { locale: 'bg-BG' })}
                </span>
            </div>
            <p>
                {children}
            </p>
        </article>
    );
}