import { ReactElement, JSX, ElementType, MouseEvent } from 'react';
export interface MenuItemProps {
    selected?: boolean;
    isShowTooltip?: boolean;
    label: string;
    icon: ReactElement | JSX.Element;
    id: number;
    onClick: (id: number | null, e: MouseEvent) => void;
    isCompactStyle?: boolean;
    dataTestId: string;
    as?: ElementType;
    href?: string;
    to?: string;
}
export type WrapperContentProps = Pick<MenuItemProps, 'to' | 'href' | 'selected' | 'isCompactStyle'>;
export type IconWrapperProps = Pick<MenuItemProps, 'selected' | 'isCompactStyle'>;
