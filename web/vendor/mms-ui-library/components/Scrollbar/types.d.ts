import { PropsWithChildren, RefAttributes } from 'react';
export type ScrollDirections = 'both' | 'horizontal' | 'vertical';
export type ScrollbarProps = {
    id?: string;
    customScrollbarWidth?: number;
    className?: string;
} & PropsWithChildren & (ScrollbarPropsBoth | ScrollbarPropsHorizontal | ScrollbarPropsVertical) & RefAttributes<HTMLDivElement>;
interface ScrollbarPropsBoth {
    scroll: Extract<ScrollDirections, 'both'>;
    maxWidth: string;
    maxHeight: string;
}
interface ScrollbarPropsHorizontal {
    scroll: Extract<ScrollDirections, 'horizontal'>;
    maxWidth: string;
    maxHeight?: string;
}
interface ScrollbarPropsVertical {
    scroll: Extract<ScrollDirections, 'vertical'>;
    maxWidth?: string;
    maxHeight: string;
}
export {};
