import { Placement } from '@floating-ui/react';
import { CalendarRangeProps } from '../CalendarRange';
import { InputSizes, InputStylesConfig } from '../Input';
import { Timestamp } from '@/types/date';
export type Range = [number | null, number | null];
export interface DatePickerRangeProps {
    range: [Timestamp | null, Timestamp | null];
    onChange: (value: [Timestamp | null, Timestamp | null]) => void;
    size: keyof InputSizes;
    inputId?: string;
    disabled?: boolean;
    innerLabel?: string;
    label?: string;
    error?: boolean;
    errorText?: string;
    minDate?: Timestamp;
    maxDate?: Timestamp;
    calendarPosition?: Placement;
    stylesConfig?: InputStylesConfig;
    calendarZIndex?: number;
    isRequired?: boolean;
    calendarProps?: Partial<CalendarRangeProps>;
    dataTestId?: string;
}
