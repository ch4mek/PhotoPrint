import { RangeCalendarProps, SingleCalendarProps } from '../types';
export declare const getSingleDefaultConfig: () => Required<Omit<SingleCalendarProps, "onChange" | "onMonthChange" | "onYearChange" | "selectedDate" | "slots" | "slotProps">> & Pick<SingleCalendarProps, "selectedDate">;
export declare const getRangeDefaultConfig: () => Required<Omit<RangeCalendarProps, "onChange" | "selectedRange" | "defaultRange" | "slots" | "slotProps">> & Pick<RangeCalendarProps, "selectedRange" | "defaultRange">;
