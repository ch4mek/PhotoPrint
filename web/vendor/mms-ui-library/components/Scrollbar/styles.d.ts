import type { Theme } from '@/types/theme';
import type { ScrollDirections, ScrollbarProps } from './types';
export declare const getScrollbarStyles: (theme: Theme, width: number, scroll: ScrollDirections) => {
    scrollbarColor: string;
    '&::-webkit-scrollbar': {
        width: number;
        height: number;
        background: string;
        '&-thumb': {
            backgroundColor: string;
            borderRadius: 24;
        };
        '&-track': {
            backgroundColor: string;
            borderRadius: 24;
        };
        '&-corner': {
            borderRadius: 24;
        };
    };
    overflowX: "auto";
    overflowY: "hidden";
    overflow?: undefined;
} | {
    scrollbarColor: string;
    '&::-webkit-scrollbar': {
        width: number;
        height: number;
        background: string;
        '&-thumb': {
            backgroundColor: string;
            borderRadius: 24;
        };
        '&-track': {
            backgroundColor: string;
            borderRadius: 24;
        };
        '&-corner': {
            borderRadius: 24;
        };
    };
    overflowX: "hidden";
    overflowY: "auto";
    overflow?: undefined;
} | {
    scrollbarColor: string;
    '&::-webkit-scrollbar': {
        width: number;
        height: number;
        background: string;
        '&-thumb': {
            backgroundColor: string;
            borderRadius: 24;
        };
        '&-track': {
            backgroundColor: string;
            borderRadius: 24;
        };
        '&-corner': {
            borderRadius: 24;
        };
    };
    overflow: "auto";
    overflowX?: undefined;
    overflowY?: undefined;
};
export declare const ScrollbarWrapper: import("@emotion/styled").StyledComponent<{
    theme?: import("@emotion/react").Theme;
    as?: React.ElementType;
} & ScrollbarProps, import("react").DetailedHTMLProps<import("react").HTMLAttributes<HTMLDivElement>, HTMLDivElement>, {}>;
