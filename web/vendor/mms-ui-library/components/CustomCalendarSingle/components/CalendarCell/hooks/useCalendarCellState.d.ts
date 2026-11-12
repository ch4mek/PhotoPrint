import { BaseCalendarDay, CalendarData, RangeCalendarProps, RangeSelectionDay, SingleCalendarProps } from '@/components/CustomCalendarSingle/types';
interface UseCalendarCellStateProps {
    dateItem: RangeSelectionDay | BaseCalendarDay;
    calendarData: CalendarData;
    config: Partial<SingleCalendarProps & RangeCalendarProps>;
}
export declare const useCalendarCellState: ({ dateItem, calendarData, config, }: UseCalendarCellStateProps) => {
    holiday: import("@/components/CustomCalendarSingle/types").Holiday | undefined;
    isRangeCalendar: boolean;
    shouldRender: boolean;
    isSelected: boolean | null;
    isInRange: boolean;
    isOtherMonth: boolean;
    isStart: boolean;
    isEnd: boolean;
    isInner: boolean;
    isInnerLast: boolean;
    isWeekendVisible: boolean | undefined;
    isHolidayVisible: boolean | undefined;
    isTempStart: boolean;
    isTempEnd: boolean;
    isTempInner: boolean;
    isTempRangeLast: boolean;
    isTempStartWithoutInner: boolean;
    isStartWithoutInner: boolean;
    isRangeStartRowEnd: boolean;
    isRangeEndRowStart: boolean;
    isInRangeRowEnd: boolean;
    isInRangeRowStart: boolean;
    isInTempRangeRowEnd: boolean;
    isInTempRangeRowStart: boolean;
    isTempStartRowEnd: boolean;
    isTempEndRowStart: boolean;
};
export {};
