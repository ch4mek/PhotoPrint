import { CSSProperties, Dispatch, FocusEventHandler, InputHTMLAttributes, KeyboardEventHandler, ReactNode, SetStateAction } from 'react';
import type { AutoselectTypes, StylesConfig } from '../types';
import type { ErrorTextStyles } from '@/components/ErrorText';
import type { InputTypography } from '@/components/Input';
import { CheckIfOptionEqualToValue, GetKey, GetOptionLabel } from '@/types/common';
export interface AutoselectStylesConfig extends StylesConfig {
    errorText?: ErrorTextStyles;
    typography?: InputTypography;
    popover?: CSSProperties;
}
export interface AutoselectProps<T> {
    value: T | null;
    options: T[];
    valueField?: string;
    subValueField?: string;
    groupByField?: string;
    errorText?: string;
    label: string;
    error?: boolean;
    disabled?: boolean;
    checkIfChipDisabled?: (option: T) => boolean;
    width?: number;
    size?: AutoselectTypes;
    disableClearable?: boolean;
    enableCustomValue?: boolean;
    labelAndErrorBgColor?: string;
    noOptionsText?: string;
    valueToClean?: T;
    name?: string;
    isDropDownOpenOuter?: boolean;
    setIsDropDownOpenOuter?: Dispatch<SetStateAction<boolean>>;
    onChange: (option: T | null) => void;
    onFocus?: FocusEventHandler<HTMLInputElement>;
    onBlur?: FocusEventHandler<HTMLInputElement>;
    onSearchInputChange?: (value: string) => void;
    getOptionLabel?: GetOptionLabel<T>;
    getKey?: GetKey<T>;
    checkIfOptionEqualToValue?: CheckIfOptionEqualToValue<T>;
    stylesConfig?: AutoselectStylesConfig;
    onKeyDown?: KeyboardEventHandler<HTMLInputElement>;
    showToolTipIfCut?: boolean;
    preventDropdownFlip?: boolean;
    renderInputToolTipContent?: (optionNmae: string) => ReactNode;
    shouldBeClosedOnOutsideScroll?: boolean;
    isRequired?: boolean;
    height?: string;
    inputProps?: Omit<InputHTMLAttributes<HTMLInputElement>, 'height'>;
    dataTestId?: string;
}
