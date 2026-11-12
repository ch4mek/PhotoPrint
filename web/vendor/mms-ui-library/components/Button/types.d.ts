import { SerializedStyles } from '@emotion/react';
import { ButtonHTMLAttributes, PropsWithChildren, ReactNode, Ref } from 'react';
import type { BodyFontSize } from '@/types/emotion';
export type ButtonView = 'filled' | 'outline' | 'light';
export type ButtonColor = 'primary' | 'secondary' | 'negative';
type ButtonAddonPlacement = 'left' | 'right';
export interface ButtonWrapperProps {
    view: ButtonView;
    color: ButtonColor;
    disabled: boolean;
    isFullWidth: boolean;
}
export interface ButtonProps extends Partial<ButtonWrapperProps>, PropsWithChildren, Omit<ButtonHTMLAttributes<HTMLButtonElement>, 'color'> {
    view?: ButtonView;
    color?: ButtonColor;
    typography?: BodyFontSize;
    addon?: ReactNode;
    addonPlacement?: ButtonAddonPlacement;
    dataTestId?: string;
    ref?: Ref<HTMLButtonElement>;
}
export type ButtonViewColorStyles = {
    [viewKey in ButtonView]: {
        [colorKey in ButtonColor]?: SerializedStyles;
    };
};
export {};
