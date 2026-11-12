import { Dayjs } from 'dayjs';
import { Dispatch, SetStateAction } from 'react';
import type { InputProps, InputStylesConfig } from '@/components/Input';
export type DateInputProps = Omit<InputProps, 'value' | keyof InnerDateInputProps> & InnerDateInputProps;
interface InnerDateInputProps {
    inputDate: string[];
    setInputDate: (value: string[]) => void;
    selectionRange: number[] | null;
    setSelectionRange: Dispatch<SetStateAction<number[] | null>>;
    disabled?: boolean;
    innerLabel?: string;
    errorText?: string;
    maxDate?: Dayjs;
    minDate?: Dayjs;
    onChange: (value: Dayjs | null) => void;
    onIconClick: VoidFunction;
    stylesConfig?: InputStylesConfig;
    showPopup: boolean;
    dataTestId: string;
}
export {};
