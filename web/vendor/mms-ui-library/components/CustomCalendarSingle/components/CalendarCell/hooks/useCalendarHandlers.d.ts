import { BaseCalendarDay, CalendarContextValue, RangeSelectionDay } from '@/components/CustomCalendarSingle/types';
interface UseCalendarHandlersProps {
    dateItem: RangeSelectionDay | BaseCalendarDay;
    calendarContext: CalendarContextValue;
}
export declare const useCalendarHandlers: ({ dateItem, calendarContext, }: UseCalendarHandlersProps) => {
    handleDateSelect: () => void;
    handleMouseEnter: () => void;
    handleMouseLeave: () => void;
    isRangeAndStatic: boolean;
    hoverDate: import("dayjs").Dayjs | null | undefined;
};
export {};
