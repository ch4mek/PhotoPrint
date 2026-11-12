import { Dayjs } from 'dayjs';
import { ComponentType, HTMLAttributes, MouseEvent } from 'react';
export interface BaseCalendarDay {
    date: Dayjs;
    isCurrentMonth: boolean;
    isToday: boolean;
    isSelected: boolean;
    isWeekend: boolean;
    isHoliday: boolean;
}
export interface RangeSelectionDay extends BaseCalendarDay {
    isRangeStart: boolean;
    isRangeEnd: boolean;
    isInRange: boolean;
    isRangeLast: boolean;
    isTempStart: boolean;
    isTempRangeLast: boolean;
    isTempRangeEnd: boolean;
    isInTempRange: boolean;
    isTempStartWithoutInner: boolean;
    isStartWithoutInner: boolean;
    isRangeStartRowEnd: boolean;
    isRangeEndRowStart: boolean;
    isInRangeRowEnd: boolean;
    isInRangeRowStart: boolean;
    isTempStartRowEnd: boolean;
    isTempEndRowStart: boolean;
    isInTempRangeRowEnd: boolean;
    isInTempRangeRowStart: boolean;
}
export interface DateRange {
    start: Dayjs | null;
    end: Dayjs | null;
}
export interface Holiday {
    date: Dayjs;
    title: string;
}
export interface HighlightedDays {
    yourList?: number[];
    blockedList?: number[];
    [key: string]: any;
}
export interface HoveredDateRange {
    start: Dayjs | null;
    end: Dayjs | null;
}
export interface DaySlotProps {
    date: Dayjs;
    isCurrentMonth: boolean;
    isToday: boolean;
    isSelected: boolean;
    isWeekend: boolean;
    isHoliday: boolean;
    selectedDay?: Dayjs;
    highlightedDays?: HighlightedDays;
    firstDay?: Dayjs;
    lastDay?: Dayjs;
    lastAvailableDay?: Dayjs;
    hovered?: Dayjs | null;
    hoveredDateRange?: HoveredDateRange;
    isSameDate?: (date1: Dayjs, date2: Dayjs) => boolean;
    onMouseEnter?: (event: MouseEvent<HTMLButtonElement>) => void;
    onMouseLeave?: (event: MouseEvent<HTMLButtonElement>) => void;
    onClick?: (event: MouseEvent<HTMLButtonElement>) => void;
    isRangeStart?: boolean;
    isRangeEnd?: boolean;
    isInRange?: boolean;
    isRangeLast?: boolean;
    isTempStart?: boolean;
    isTempRangeLast?: boolean;
    isTempRangeEnd?: boolean;
    isInTempRange?: boolean;
    isTempStartWithoutInner?: boolean;
    isStartWithoutInner?: boolean;
    isRangeStartRowEnd?: boolean;
    isRangeEndRowStart?: boolean;
    isInRangeRowEnd?: boolean;
    isInRangeRowStart?: boolean;
    isTempStartRowEnd?: boolean;
    isTempEndRowStart?: boolean;
    isInTempRangeRowEnd?: boolean;
    isInTempRangeRowStart?: boolean;
    disabled?: boolean;
    isOtherMonth?: boolean;
    holiday?: Holiday;
    className?: string;
    [key: string]: any;
}
export interface CalendarSlots {
    day?: ComponentType<DaySlotProps & HTMLAttributes<HTMLButtonElement>>;
}
export interface CalendarSlotProps {
    day?: {
        selectedDay?: Dayjs;
        highlightedDays?: HighlightedDays;
        firstDay?: Dayjs;
        lastDay?: Dayjs;
        lastAvailableDay?: Dayjs;
        hovered?: Dayjs | null;
        hoveredDateRange?: HoveredDateRange;
        isSameDate?: (date1: Dayjs, date2: Dayjs) => boolean;
        onMouseEnter?: (date: Dayjs) => void;
        onMouseLeave?: VoidFunction;
        [key: string]: any;
    };
}
interface CalendarBaseProps {
    className?: string;
    calendarWidth?: number;
    minDate?: Dayjs;
    maxDate?: Dayjs;
    weekStartsOn?: CalendarWeekStartsOn;
    showWeekends?: boolean;
    showHolidays?: boolean;
    holidays?: Holiday[];
    disablePast?: boolean;
    disableFuture?: boolean;
    disableHighlightToday?: boolean;
    isInteractive?: boolean;
    disabled?: boolean;
    showDaysOutsideCurrentMonth?: boolean;
    onMonthChange?: (month: Dayjs) => void;
    onYearChange?: (year: Dayjs) => void;
    onChange?: (value: Dayjs | DateRange | null) => void;
    slots?: CalendarSlots;
    slotProps?: CalendarSlotProps;
}
export interface SingleCalendarProps extends CalendarBaseProps {
    mode?: CalendarMode;
    selectedDate?: Dayjs | null;
}
export interface RangeCalendarProps extends Omit<CalendarBaseProps, 'isInteractive' | 'showDaysOutsideCurrentMonth' | 'onMonthChange' | 'onYearChange' | 'disablePast' | 'disableFuture' | 'showDaysOutsideCurrentMonth' | 'calendarWidth'> {
    mode?: CalendarMode;
    selectedRange?: DateRange | null;
    defaultRange?: DateRange | null;
    isStatic?: boolean;
}
export type CalendarConfigInput = Partial<SingleCalendarProps & RangeCalendarProps>;
export interface CalendarData {
    currentDate: Dayjs;
    selectedDate: Dayjs | null;
    selectedRange?: DateRange | null;
    mode?: CalendarMode;
    weekStartsOn: CalendarWeekStartsOn;
    days: BaseCalendarDay[];
    holidays: Holiday[];
    dayHeaders?: string[];
}
interface BaseCalendarContext {
    config: CalendarConfigInput;
    calendarData: CalendarData;
    navigateToDate: (date: Dayjs) => void;
    selectDate: (date: Dayjs | null) => void;
    onMonthSelect: (month: number) => void;
    onYearSelect: (year: number) => void;
    onDateSelect?: (date: Dayjs | DateRange | null) => void;
    generateDaysFor: (date: Dayjs) => BaseCalendarDay[] | RangeSelectionDay[];
}
export interface RangeCalendarContext extends BaseCalendarContext {
    currentMonth: Dayjs;
    nextMonth: Dayjs;
    selectedRange: DateRange | null;
    leftData: CalendarData;
    rightData: CalendarData;
    hoverDate: Dayjs | null;
    handleMonthNavigation: (direction: CalendarNavigation) => void;
    selectRange: (startDate: Dayjs, endDate: Dayjs) => void;
    clearSelection: VoidFunction;
    onDateHover: (date: Dayjs | null) => void;
}
export type CalendarContextValue = BaseCalendarContext | RangeCalendarContext;
interface BaseCalendarHook {
    data: CalendarData;
    currentDate: Dayjs;
    selectedDate: Dayjs | null;
    config: CalendarConfigInput;
    selectDate: (date: Dayjs | null) => void;
    selectMonth: (month: number) => void;
    selectYear: (year: number) => void;
    navigateToDate: (date: Dayjs) => void;
    navigateToMonthYear: (year: number, month: number) => void;
    generateDaysFor: (date: Dayjs) => BaseCalendarDay[];
}
export interface RangeCalendarHook extends BaseCalendarHook {
    leftData: CalendarData;
    rightData: CalendarData;
    selectedRange: DateRange | null;
    currentMonth: Dayjs;
    nextMonth: Dayjs;
    isSelectingRange: boolean;
    tempStartDate: Dayjs | null;
    hoverDate: Dayjs | null;
    handleMonthNavigation: (direction: CalendarNavigation) => void;
    selectRange: (start: Dayjs, end: Dayjs) => void;
    clearSelection: VoidFunction;
    onDateHover: (date: Dayjs | null) => void;
}
export declare const enum RangeCalendarPart {
    Left = "Left",
    Right = "Right"
}
export declare const enum CalendarNavigation {
    Prev = "Prev",
    Next = "Next"
}
export declare const enum CalendarWeekStartsOn {
    Sunday = 0,
    Monday = 1
}
export declare const enum CalendarMode {
    Single = "single",
    Range = "range"
}
export {};
