import { Dayjs } from 'dayjs';
import { ElementType } from 'react';
import { CalendarUIProps } from '../Calendar/types';
export interface Holiday {
    date: Dayjs;
    title: string;
}
export interface CalendarRangeProps extends Omit<CalendarUIProps, 'value' | 'onChange' | 'isInteractive'> {
    range: [number | null, number | null];
    onChange?: (range: [number | null, number | null]) => void;
    isStatic?: boolean;
    holidays?: Holiday[];
    calendarRangeWrapper?: ElementType;
    handleClose?: VoidFunction;
}
export interface CalendarsWrappersProps {
    isStatic: boolean;
    maxHeight: number | undefined;
    shouldWrapContent: boolean;
}
export interface CalendarWrapperProps {
    isStatic: boolean;
    isWrapped: boolean;
}
