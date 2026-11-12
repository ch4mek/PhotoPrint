import { RefAttributes } from 'react';
import { Margin } from 'recharts/types/util/types';
export interface CustomizedAxisTickProps<T, U extends string | number> {
    x?: number;
    y?: number;
    index?: number;
    payload?: {
        value: U;
    };
    values?: T[];
}
type Tick<T, U extends string | number> = boolean | React.SVGProps<SVGTextElement> | React.ReactElement<SVGElement> | ((props: CustomizedAxisTickProps<T, U>) => React.ReactElement<SVGElement>) | undefined;
interface TooltipPayloadItem<T, U extends string | number> {
    dataKey?: keyof T;
    payload: T;
    value?: U;
}
export interface CustomizedTooltipProps<T, U extends string | number> extends RefAttributes<HTMLDivElement> {
    isTooltipOnLeftSide: boolean;
    active?: boolean;
    payload?: Array<TooltipPayloadItem<T, U>>;
}
export interface ChartProps<T, U extends string | number> {
    data: T[];
    xAxisDataKey?: keyof T;
    yAxisDataKey?: keyof T;
    xAxisHeight?: number;
    xAxisWidth?: number;
    yAxisWidth?: number;
    xAxisTick?: Tick<T, U>;
    yAxisTick?: Tick<T, U>;
    CustomizedTooltipComponent?: (props: CustomizedTooltipProps<T, U>) => React.ReactNode;
    shouldUseDataAsXAxisValues?: boolean;
    shouldUseDataAsYAxisValues?: boolean;
    heightOfChart?: number;
    barColor?: string;
    barChartMargins?: Margin;
}
export type BarBorders = [number, number, number, number];
export {};
