import { ButtonHTMLAttributes, RefAttributes } from 'react';
export type FilledEditingButtonVariant = 'apply' | 'cancel';
export interface FilledEditingButtonWrapperProps extends RefAttributes<HTMLButtonElement> {
    variant: FilledEditingButtonVariant;
    dataTestId?: string;
}
export type FilledEditingButtonProps = FilledEditingButtonWrapperProps & ButtonHTMLAttributes<HTMLButtonElement>;
