import { Placement } from '@floating-ui/react';
import { Dayjs } from 'dayjs';
import { SingleCalendarProps } from '../CustomCalendarSingle';
import type { InputLabelStyles, InputSizes, InputStylesConfig } from '../Input';
export interface DatePickerProps {
    value: Dayjs | null;
    onChange: (value: Dayjs | null) => void;
    inputId?: string;
    disabled?: boolean;
    label?: string;
    innerLabel?: string;
    errorText?: string;
    minDate?: Dayjs;
    maxDate?: Dayjs;
    size?: keyof InputSizes;
    calendarPosition?: Placement;
    calendarOffset?: [number, number];
    stylesConfig?: InputStylesConfig;
    calendarZIndex?: number;
    isRequired?: boolean;
    calendarProps?: SingleCalendarProps;
    parseInputDateToDate?: (value: string[]) => Dayjs | null;
    dataTestId?: string;
}
export interface LabelProps {
    stylesConfig?: InputLabelStyles;
}
