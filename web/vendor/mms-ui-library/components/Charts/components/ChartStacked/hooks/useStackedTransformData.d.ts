import type { ChartStackedProjectAdapted } from '../index.stories';
export declare function useStackedTransformData(data: ChartStackedProjectAdapted[]): {
    keyWithNameAndColorMap: import("..").KeyColorItem[];
    transformedData: {
        name: string | {
            date: number;
            weekDay: string;
        };
        lastProjectIndex: string | null;
    }[];
    calculatedYAxisTicks: number[];
};
export type TransformedData = ReturnType<typeof useStackedTransformData>['transformedData'][number];
