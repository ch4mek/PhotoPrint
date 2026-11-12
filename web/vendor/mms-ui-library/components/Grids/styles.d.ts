import { CSSProperties } from 'react';
import type { Theme } from '@/types/theme';
import { ColoredWrapperProps } from './types';
export declare const getDataGridStyles: (theme: Theme, hideVerticalScrollbar: boolean, hasPagination?: boolean) => import("@emotion/react").SerializedStyles;
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
