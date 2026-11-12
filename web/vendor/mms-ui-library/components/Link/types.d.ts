import { MouseEvent, ReactNode } from 'react';
export interface LinkProps {
    href?: string;
    onClick?: (event: MouseEvent<HTMLAnchorElement>) => void;
    children?: ReactNode;
    target?: string;
    dataTestId?: string;
}
export interface LinkComponentProps {
    dataTestId?: string;
}
