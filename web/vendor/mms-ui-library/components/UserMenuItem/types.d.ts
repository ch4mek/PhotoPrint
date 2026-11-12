import { ReactNode } from 'react';
export interface UserMenuItemProps {
    title: string;
    icon?: ReactNode;
    onClick?: VoidFunction;
    dataTestId?: string;
}
