import { Placement } from '@floating-ui/react';
import { Dayjs } from 'dayjs';
import { InputSizes, InputStylesConfig } from '../Input';
export interface MonthYearPickerProps {
    value: Dayjs | null;
    onChange: (value: Dayjs) => void;
    inputId?: string;
    disabled?: boolean;
    label?: string;
    innerLabel?: string;
    errorText?: string;
    error?: boolean;
    stylesConfig?: InputStylesConfig;
    isRequired?: boolean;
    minDate?: Dayjs;
    maxDate?: Dayjs;
    size: keyof InputSizes;
    dataTestId?: string;
    calendarPosition?: Placement;
}
