import { Dayjs } from 'dayjs';
import { Dispatch, FocusEventHandler, SetStateAction } from 'react';
import { DateRange } from '../../types';
import { InputSizes, InputStylesConfig } from '@/components/Input';
import { InputDate } from '@/types/date';
export interface DateRangeInputProps {
    inputDate: InputDate;
    setInputDate: (value: InputDate) => void;
    selectionRange: number[] | null;
    setSelectionRange: Dispatch<SetStateAction<number[] | null>>;
    disabled?: boolean;
    label?: string;
    innerLabel?: string;
    error?: boolean;
    errorText?: string;
    onBlur?: FocusEventHandler<HTMLInputElement>;
    onFocus?: FocusEventHandler<HTMLInputElement>;
    maxDate?: Dayjs;
    minDate?: Dayjs;
    valueRange: DateRange;
    onChange: (value: DateRange) => void;
    onInput?: (value: DateRange) => void;
    onIconClick: VoidFunction;
    size: keyof InputSizes;
    stylesConfig?: InputStylesConfig;
    showPopup: boolean;
    dataTestId?: string;
    onInputFocusChange?: (isFocused: boolean) => void;
}
