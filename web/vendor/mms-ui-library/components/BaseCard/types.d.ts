import { HTMLAttributes, ReactNode } from 'react';
export interface WrapperStyledProps {
    maxWidth?: number;
    maxHeight?: number;
    minWidth?: number;
    isFullWidth?: boolean;
}
export interface HeaderStyledProps {
    color?: string;
}
export interface BodyStyledProps {
    color?: string;
    hoverColor?: string;
    padding?: number;
}
export interface RowStyledProps {
    marginTop?: number;
    marginBottom?: number;
}
export interface BaseCardWrapperProps extends HTMLAttributes<HTMLDivElement>, WrapperStyledProps {
    children: ReactNode;
    className?: string;
    dataTestId?: string;
}
export interface BaseCardHeaderProps extends HTMLAttributes<HTMLElement>, HeaderStyledProps {
    children: ReactNode;
    className?: string;
    element?: ReactNode;
    dataTestId?: string;
}
export interface BaseCardBodyProps extends HTMLAttributes<HTMLDivElement>, BodyStyledProps {
    children: ReactNode;
    className?: string;
    dataTestId?: string;
}
export interface BaseCardRowProps extends HTMLAttributes<HTMLElement>, RowStyledProps {
    children: ReactNode;
    className?: string;
    dataTestId?: string;
}
