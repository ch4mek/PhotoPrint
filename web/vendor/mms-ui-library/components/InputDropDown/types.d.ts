import { CSSProperties } from 'react';
import type { DropTabTypography, DropTabValuesOptions } from '../DropTab';
import { InputProps, InputSizes } from '../Input/types';
export type OptionType = Record<string, string | number>;
export interface InputDropDownProps<T extends OptionType> extends DropTabValuesOptions, Omit<Partial<MainProps>, 'isActive'> {
    value?: string;
    options: T[];
    onChange?: (value: string) => void;
    valueField: string;
    nameField?: string;
    label?: string;
    error?: boolean;
    errorText?: string;
    isRequired?: boolean;
    size?: keyof InputSizes;
    disabled?: boolean;
    hasSelected?: boolean;
    dropTabPaddings?: number;
    minItemHeight?: number;
    paddingInsideItemStyle?: string;
    dropTabWidth?: string;
    closeOnSelect?: boolean;
    className?: string;
    stylesConfig?: CSSProperties;
    showToolTipIfCut?: boolean;
    preventDropdownFlip?: boolean;
    typography?: DropTabTypography;
    useAnchorWidth?: boolean;
    inputProps?: Omit<Partial<InputProps>, 'value' | 'onChange'>;
    dataTestId?: string;
}
export interface MainProps {
    stylesConfig?: CSSProperties;
}
