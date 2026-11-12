import { ReactNode } from 'react';
export type SliderButtonDirection = 'left' | 'right';
export type SliderProps = {
    children?: ReactNode;
    childrenPerSlide?: number;
    height?: number;
    width?: number;
    dataTestId?: string;
    hasBorderTop?: boolean;
    hasBorderLeft?: boolean;
    hasBorderRight?: boolean;
    hasBorderBottom?: boolean;
    SliderEmptyComponent?: ReactNode;
};
export type SlidesWrapperProps = {
    left: string;
};
export type SliderContainerProps = {
    height?: number;
    width?: number;
};
export type SliderWrapperProps = {
    hasBorderTop: boolean;
    hasBorderLeft?: boolean;
    hasBorderRight?: boolean;
    hasBorderBottom?: boolean;
};
export interface SliderButtonProps {
    direction: SliderButtonDirection;
}
