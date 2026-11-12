import { Bar } from 'recharts';
import { CategoricalChartState } from 'recharts/types/chart/types';
import { Coordinate } from 'recharts/types/util/types';
import { TooltipPayload } from '../types';
export declare function useTooltipPosition<T extends string | number>(isYAxisPositionFixed?: boolean): {
    activeBarRef: import("react").RefObject<(Bar & SVGPathElement) | null>;
    isTooltipOnLeftSide: boolean;
    tooltipRef: import("react").RefObject<HTMLDivElement | null>;
    barCharWrapperRef: import("react").RefObject<HTMLDivElement | null>;
    tooltipPosition: Partial<Coordinate> | undefined;
    handleMouseMove: (data: CategoricalChartState) => void;
    handleMouseEnter: (payload: TooltipPayload<T>) => void;
    tooltipPayload: TooltipPayload<T> | null;
};
