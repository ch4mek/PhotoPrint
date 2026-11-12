import { Dayjs } from 'dayjs';
import { CalendarWeekStartsOn } from '../types';
export declare function isSameDate(date1: Dayjs, date2: Dayjs): boolean;
export declare function isSameMonth(date1?: Dayjs | null, date2?: Dayjs | null): boolean;
export declare function isWeekend(date: Dayjs, weekStartsOn?: CalendarWeekStartsOn): boolean;
export declare function stripTime(date: Dayjs): Dayjs;
export declare function isDateInRange({ date, minDate, maxDate, disablePast, disableFuture, }: {
    date: Dayjs;
    minDate?: Dayjs;
    maxDate?: Dayjs;
    disablePast?: boolean;
    disableFuture?: boolean;
}): boolean;
export declare const isDateBetweenMixAndMax: (date: Dayjs) => boolean;
