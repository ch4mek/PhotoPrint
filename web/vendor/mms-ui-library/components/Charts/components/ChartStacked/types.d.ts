import { RefAttributes } from 'react';
import { Margin } from 'recharts/types/util/types';
import { TooltipPayload } from '../../types';
type Tick<T, U extends string | number> = boolean | React.SVGProps<SVGTextElement> | React.ReactElement<SVGElement> | ((props: CustomizedAxisTickPropsStacked<T, U>) => React.ReactElement<SVGElement>) | undefined;
export interface ChartStackedProps<T, U, V extends string | number> {
    data: T[];
    keyWithNameAndColorMap: KeyColorItem[];
    transformedData: U[];
    calculatedYAxisTicks: number[];
    lastStackedItemKey: keyof U;
    xAxisDataKey?: keyof T;
    yAxisDataKey?: keyof T;
    xAxisHeight?: number;
    xAxisWidth?: number;
    yAxisWidth?: number;
    xAxisTick?: Tick<T, V>;
    yAxisTick?: Tick<T, V>;
    CustomizedTooltipComponent?: (props: CustomizedTooltipPropsStacked<U, V>) => React.ReactNode;
    shouldUseDataAsXAxisValues?: boolean;
    shouldUseDataAsYAxisValues?: boolean;
    heightOfChart?: number;
    barChartMargins?: Margin;
}
export interface CustomizedAxisTickPropsStacked<T, U extends string | number> {
    x?: number;
    y?: number;
    index?: number;
    payload?: {
        value: U;
    };
    values?: T[];
}
export interface TooltipPayloadItem<T, U extends string | number> {
    dataKey: string;
    payload: T;
    value: U;
}
export interface CustomizedTooltipPropsStacked<T, U extends string | number> extends RefAttributes<HTMLDivElement> {
    isTooltipOnLeftSide: boolean;
    tooltipPayload: TooltipPayload<U> | null;
    active?: boolean;
    payload?: Array<TooltipPayloadItem<T, U>>;
}
export interface KeyColorItem {
    id: string;
    name: string;
    color: string;
}
export {};
