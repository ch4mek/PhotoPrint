import { ButtonHTMLAttributes, RefAttributes } from 'react';
export type ResetFilterButtonSize = 'small' | 'big';
export interface ResetFilterButtonWrapperProps {
    size: ResetFilterButtonSize;
    disabled: boolean;
    dataTestId?: string;
}
export type ResetFilterButtonProps = Partial<ResetFilterButtonWrapperProps> & ButtonHTMLAttributes<HTMLButtonElement> & RefAttributes<HTMLButtonElement>;
