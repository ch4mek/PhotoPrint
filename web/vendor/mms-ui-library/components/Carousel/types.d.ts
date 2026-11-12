import { CSSProperties } from 'react';
export interface CarouselProps {
    sliderWidth?: number | string;
    slidesToShow?: number;
    isControlVisible?: boolean;
    enableScroll?: boolean;
    scrollThreshold?: number;
    dataTestId?: string;
    slideStyles?: CSSProperties;
}
export interface ScrollState {
    delta: number;
    scrolling: boolean;
    timerId: ReturnType<typeof setTimeout> | null;
}
