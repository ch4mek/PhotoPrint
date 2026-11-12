import { Dispatch, SetStateAction } from 'react';
import type { InputProps, InputStylesConfig } from '@/components/Input';
import { Timestamp } from '@/types/date';
export type DateInputProps = Omit<InputProps, 'value' | keyof InnerDateInputProps> & InnerDateInputProps;
interface InnerDateInputProps {
    inputDate: string[];
    setInputDate: (value: string[]) => void;
    selectionRange: number[] | null;
    setSelectionRange: Dispatch<SetStateAction<number[] | null>>;
    disabled?: boolean;
    innerLabel?: string;
    errorText?: string;
    maxDate?: Timestamp;
    minDate?: Timestamp;
    onChange: (value: Timestamp | null) => void;
    onIconClick: () => void;
    stylesConfig?: InputStylesConfig;
    showPopup: boolean;
    dataTestId: string;
}
export {};
