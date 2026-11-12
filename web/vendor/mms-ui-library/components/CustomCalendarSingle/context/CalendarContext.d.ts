import { Dayjs } from 'dayjs';
import { ReactNode } from 'react';
import { CalendarConfigInput, CalendarContextValue, DateRange } from '../types';
interface CalendarProviderProps {
    children: ReactNode;
    config?: CalendarConfigInput;
    onDateSelect?: (date: Dayjs | DateRange | null) => void;
}
export declare const useCalendarContext: () => CalendarContextValue;
export declare function CalendarProvider({ children, config, onDateSelect, }: CalendarProviderProps): import("react/jsx-runtime").JSX.Element;
export {};
