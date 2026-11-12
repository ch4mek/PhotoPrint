import { SerializedStyles } from '@emotion/react';
import { CSSProperties, Key, Ref, JSX } from 'react';
import { DataGridHandle, DataGridProps as RDGDataGridProps, TreeDataGridProps as RDGTreeDataGridProps } from 'react-data-grid';
import type { PaginationProps } from '../Pagination';
import type { Theme } from '@/types/theme';
export interface GridPaginationProps extends PaginationProps {
    overridePaginationContainerStyles?: (theme: Theme) => SerializedStyles;
}
interface MinHeightRestrictions {
    minRowsCount?: number;
    maxRowsCount?: number;
    restrictionRowHeight?: number;
}
export interface DataGridProps<R, SR = unknown, K extends Key = Key> extends RDGDataGridProps<R, SR, K>, MinHeightRestrictions {
    overrideDataGridStyles?: (theme: Theme) => SerializedStyles;
    paginationProps?: GridPaginationProps;
    containerElement?: () => JSX.Element;
    hideVerticalScrollbar?: boolean;
    scrollToTopOnRowsChange?: boolean;
    stylesConfig?: DataGridStylesConfig;
    dataTestId?: string;
    ref?: Ref<DataGridHandle>;
}
export interface TreeDataGridProps<R, SR = unknown, K extends Key = Key> extends RDGTreeDataGridProps<R, SR, K> {
    overrideDataGridStyles?: (theme: Theme) => SerializedStyles;
    paginationProps?: GridPaginationProps;
    containerElement?: () => JSX.Element;
    hideVerticalScrollbar?: boolean;
    ref?: Ref<DataGridHandle>;
}
export interface DataGridStylesConfig {
    pagination?: {
        container?: CSSProperties;
    };
}
export interface ColoredWrapperProps extends MinHeightRestrictions {
    headerHeight?: number | null;
    isScrollVisible?: boolean;
    paginationHeight?: number;
}
export {};
