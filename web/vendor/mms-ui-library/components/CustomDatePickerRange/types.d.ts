import { Placement } from '@floating-ui/react';
import { Dayjs } from 'dayjs';
import type { FocusEventHandler } from 'react';
import { RangeCalendarProps } from '../CustomCalendarSingle';
import { InputSizes, InputStylesConfig } from '../Input';
export type DateRange = [Dayjs | null, Dayjs | null];
export interface DatePickerRangeProps {
    range: DateRange;
    onChange: (value: DateRange) => void;
    onInput?: (value: DateRange) => void;
    size: keyof InputSizes;
    inputId?: string;
    disabled?: boolean;
    innerLabel?: string;
    label?: string;
    error?: boolean;
    errorText?: string;
    onBlur?: FocusEventHandler<HTMLInputElement>;
    onFocus?: FocusEventHandler<HTMLInputElement>;
    minDate?: Dayjs;
    maxDate?: Dayjs;
    calendarPosition?: Placement;
    stylesConfig?: InputStylesConfig;
    calendarZIndex?: number;
    isRequired?: boolean;
    calendarProps?: Partial<RangeCalendarProps>;
    dataTestId?: string;
}
