import { HTMLProps, ReactNode } from 'react';
import type { ErrorTextStyles } from '../ErrorText';
import type { BodyFontSize } from '@/types/emotion';
export interface InputTypography {
    labelTypography?: BodyFontSize;
    activeLabelTypography?: BodyFontSize;
    inputTypography?: BodyFontSize;
}
export interface InputProps extends Omit<HTMLProps<HTMLInputElement>, 'size' | 'as' | 'onChange'> {
    size: keyof InputSizes;
    onChange: (fieldName: string | undefined, value: string) => void;
    errorText?: string;
    error?: boolean;
    value: string;
    helperText?: string;
    startAdornment?: ReactNode;
    endAdornment?: ReactNode;
    isFocused?: boolean;
    stylesConfig?: InputStylesConfig;
    showToolTipIfCut?: boolean;
    isRequired?: boolean;
    dataTestId?: string;
}
export interface LabelProps {
    isNotEmpty: boolean;
    isFocused: boolean;
    error: boolean;
    disabled?: boolean;
    styles?: InputLabelStyles;
    isTextAreaLabel?: boolean;
    labelTypography: BodyFontSize;
    activeLabelTypography: BodyFontSize;
}
export interface InputLabelStyles {
    opacity?: number;
    background?: string;
    color?: string;
}
export interface StyledInputProps {
    sizeField: keyof InputSizes;
    disabled?: boolean;
    isFocused?: boolean;
    error?: boolean;
    stylesConfig?: InputStyles;
}
export interface InputFieldProps {
    hasLabel: boolean;
    isFocused?: boolean;
    stylesConfig?: InputStyles;
    inputTypography: BodyFontSize;
}
export interface InputSizes {
    large: string;
    medium: string;
    small: string;
    fullWidth: string;
}
export interface InputStyles {
    border?: string;
    placeholderColor?: string;
    color?: string;
    borderOnHover?: string;
    customColorStylesValues?: string[];
}
export interface CalendarIconStyles {
    color?: string;
}
export interface InputStylesConfig {
    input?: InputStyles;
    errorText?: ErrorTextStyles;
    label?: InputLabelStyles;
    icon?: CalendarIconStyles;
    typography?: InputTypography;
}
