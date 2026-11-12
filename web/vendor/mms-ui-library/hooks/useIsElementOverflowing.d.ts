import { RefObject } from 'react';
export declare function useIsElementOverflowing(elementRef: RefObject<HTMLElement | null>): {
    isOverflowingXAxis: boolean;
    isOverflowingYAxis: boolean;
};
