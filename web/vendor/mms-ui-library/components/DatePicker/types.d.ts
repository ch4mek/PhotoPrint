import { Placement } from '@floating-ui/react';
import type { CalendarUIProps } from '../Calendar';
import type { InputLabelStyles, InputSizes, InputStylesConfig } from '../Input';
import { Timestamp } from '@/types/date';
export interface DatePickerProps {
    value: Timestamp | null;
    onChange: (value: Timestamp | null) => void;
    inputId?: string;
    disabled?: boolean;
    label?: string;
    innerLabel?: string;
    errorText?: string;
    minDate?: Timestamp;
    maxDate?: Timestamp;
    size?: keyof InputSizes;
    calendarPosition?: Placement;
    calendarOffset?: [number, number];
    stylesConfig?: InputStylesConfig;
    calendarZIndex?: number;
    isRequired?: boolean;
    calendarProps?: CalendarUIProps;
    parseInputDateToDate?: (value: string[]) => Timestamp | null;
    dataTestId?: string;
}
export interface LabelProps {
    stylesConfig?: InputLabelStyles;
}
