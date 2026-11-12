import { ButtonHTMLAttributes, Ref } from 'react';
export type FilterButtonSize = 'small' | 'big';
export interface FilterButtonWrapperProps {
    isActive: boolean;
    isOpen: boolean;
    size?: FilterButtonSize;
    dataTestId?: string;
    ref?: Ref<HTMLButtonElement>;
}
export type FilterButtonProps = FilterButtonWrapperProps & ButtonHTMLAttributes<HTMLButtonElement>;
