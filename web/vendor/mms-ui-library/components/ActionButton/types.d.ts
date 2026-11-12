import { CSSObject } from '@emotion/react';
import { ButtonHTMLAttributes, PropsWithChildren, Ref } from 'react';
export type ActionButtonColor = 'primary' | 'white';
export type ActionButtonSize = 'small' | 'medium' | 'large';
export interface ActionButtonWrapperProps {
    isActive: boolean;
    color: ActionButtonColor;
    size: ActionButtonSize;
    disabled: boolean;
    error?: boolean;
    ref?: Ref<HTMLButtonElement>;
    styles?: CSSObject;
}
export type ActionButtonProps = PropsWithChildren & Partial<ActionButtonWrapperProps> & ButtonHTMLAttributes<HTMLButtonElement>;
