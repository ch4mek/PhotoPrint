import { Dispatch, SetStateAction } from 'react';
import { Range } from '../../types';
import { InputSizes, InputStylesConfig } from '@/components/Input';
import { InputDate, Timestamp } from '@/types/date';
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
    maxDate?: Timestamp;
    minDate?: Timestamp;
    onChange: (value: Range) => void;
    onIconClick: () => void;
    size: keyof InputSizes;
    stylesConfig?: InputStylesConfig;
    showPopup: boolean;
    dataTestId?: string;
}
