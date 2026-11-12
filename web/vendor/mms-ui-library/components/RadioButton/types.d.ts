import { RefAttributes } from 'react';
import type { TypographyProps } from '../Typography';
type Value = string | number;
export interface OnChangeArgs {
    value?: Value;
    checked?: boolean;
    name?: string;
}
export interface RadioButtonProps extends RefAttributes<HTMLInputElement> {
    checked?: boolean;
    name?: string;
    value?: Value;
    onChange?: (args: OnChangeArgs) => void;
    disabled?: boolean;
    label?: string;
    labelProps?: Partial<TypographyProps>;
    dataTestId?: string;
    forAttribute?: string;
    color?: string;
}
export interface RadioButtonCircleProps {
    checked: boolean;
}
export interface RadioButtonWrapperProps {
    disabled: boolean;
}
export {};
