import { Dayjs } from 'dayjs';
import { CalendarConfigInput, CalendarData, DateRange } from '../types';
interface UseRangeSelectionProps {
    calendarConfig: CalendarConfigInput;
    currentMonth: Dayjs;
    nextMonth: Dayjs;
}
export declare const useRangeSelection: ({ calendarConfig, currentMonth, nextMonth, }: UseRangeSelectionProps) => {
    leftData: CalendarData;
    rightData: CalendarData;
    selectedRange: DateRange | null;
    isSelectingRange: boolean;
    tempStartDate: Dayjs | null;
    hoverDate: Dayjs | null;
    selectDate: (date: Dayjs) => void;
    selectRange: (startDate: Dayjs, endDate: Dayjs) => void;
    onDateHover: import("react").Dispatch<import("react").SetStateAction<Dayjs | null>>;
    clearSelection: () => void;
};
export {};
