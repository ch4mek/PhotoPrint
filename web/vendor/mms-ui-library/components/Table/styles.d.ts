import { CSSProperties } from 'react';
import type { Theme } from '@/types/theme';
import { ColoredWrapperProps, TableDataProps } from './types';
export declare const getTableStyles: (theme: Theme, hideVerticalScrollbar: boolean, hasPagination?: boolean) => import("@emotion/react").SerializedStyles;
export declare const TableData: import("@emotion/styled").StyledComponent<{
    theme?: import("@emotion/react").Theme;
    as?: React.ElementType;
} & TableDataProps, import("react").DetailedHTMLProps<import("react").TdHTMLAttributes<HTMLTableDataCellElement>, HTMLTableDataCellElement>, {}>;
export declare const TableHeader: import("@emotion/styled").StyledComponent<{
    theme?: import("@emotion/react").Theme;
    as?: React.ElementType;
} & TableDataProps & import("react").ClassAttributes<HTMLTableHeaderCellElement> & import("react").ThHTMLAttributes<HTMLTableHeaderCellElement> & {
    theme?: import("@emotion/react").Theme;
}, {}, {}>;
export declare const ColoredWrapper: import("@emotion/styled").StyledComponent<{
    theme?: import("@emotion/react").Theme;
    as?: React.ElementType;
} & ColoredWrapperProps, import("react").DetailedHTMLProps<import("react").HTMLAttributes<HTMLDivElement>, HTMLDivElement>, {}>;
export declare const PaginationContainer: import("@emotion/styled").StyledComponent<{
    theme?: import("@emotion/react").Theme;
    as?: React.ElementType;
} & {
    stylesConfig?: CSSProperties;
}, import("react").DetailedHTMLProps<import("react").HTMLAttributes<HTMLDivElement>, HTMLDivElement>, {}>;
