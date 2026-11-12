import dayjs, { Dayjs } from 'dayjs';
import { CalendarNavigation } from '../types';
export declare const useRangeMonthNavigation: (date?: Dayjs | null) => {
    currentMonth: dayjs.Dayjs;
    nextMonth: dayjs.Dayjs;
    handleMonthNavigation: (direction: CalendarNavigation) => void;
};
