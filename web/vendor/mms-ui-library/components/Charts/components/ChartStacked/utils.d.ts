import type { ChartStackedProjectAdapted } from './index.stories';
import { KeyColorItem } from './types';
export declare const getHoursFromNumber: (number: number) => number;
export declare const getMinutesFromNumber: (number: number) => number;
export declare function calculateMaxYAxisValueInHours(data: ChartStackedProjectAdapted[]): number;
export declare function createKeyWithNameAndColorMap(data: ChartStackedProjectAdapted[], maxYAxisValueInHours: number | undefined, mockProjectColor?: string): KeyColorItem[];
