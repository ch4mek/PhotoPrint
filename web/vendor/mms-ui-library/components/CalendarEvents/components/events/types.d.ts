export declare enum CalendarMode {
    Week = "week",
    Month = "month"
}
export type CalendarEvent = {
    id: string;
    title: string;
    start: string;
    end: string;
    duration?: number;
    recurring?: boolean;
};
export type BaseEventProps = {
    event: CalendarEvent;
    mode: CalendarMode;
};
