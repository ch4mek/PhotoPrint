import { SerializedStyles } from '@emotion/react';
import { ColumnDef, ColumnFiltersState, ColumnPinningState, OnChangeFn, Row, RowData, TableMeta, SortingState, ColumnPinningPosition } from '@tanstack/react-table';
import { CSSProperties, Ref, JSX, ReactNode } from 'react';
import type { PaginationProps } from '../Pagination';
import type { Theme } from '@/types/theme';
interface TablePaginationProps extends PaginationProps {
    overridePaginationContainerStyles?: (theme: Theme) => SerializedStyles;
}
interface MinHeightRestrictions {
    minRowsCount?: number;
    maxRowsCount?: number;
    restrictionRowHeight?: number;
}
export interface TableProps<TData extends RowData, TValue = unknown> extends MinHeightRestrictions {
    overrideTableStyles?: (theme: Theme) => SerializedStyles;
    paginationProps?: TablePaginationProps;
    containerElement?: () => (arg: {
        children: React.ReactNode;
    }) => JSX.Element;
    hideVerticalScrollbar?: boolean;
    scrollToTopOnRowsChange?: boolean;
    stylesConfig?: TableStylesConfig;
    dataTestId?: string;
    ref?: Ref<HTMLDivElement>;
    rows: TData[];
    columns: ColumnDef<TData, TValue>[];
    rowHeight?: number | ((row: TData) => number | null) | null;
    headerRowHeight?: number | null;
    sortColumns?: SortingState;
    onSortColumnsChange?: OnChangeFn<SortingState>;
    columnFilters?: ColumnFiltersState;
    onColumnFiltersChange?: OnChangeFn<ColumnFiltersState>;
    columnPinning?: ColumnPinningState;
    meta?: TableMeta<TData>;
    overscan?: number;
    noRowsFallback?: ReactNode;
    onRowsUpdate?: VoidFunction;
}
export interface TableBodyRowProps<TData extends RowData> {
    row: Row<TData>;
    index: number;
    start: number;
    height: number;
}
interface TableStylesConfig {
    pagination?: {
        container?: CSSProperties;
    };
}
export interface ColoredWrapperProps extends MinHeightRestrictions {
    headerHeight?: number | null;
    isScrollVisible?: boolean;
    paginationHeight?: number;
}
export interface TableDataProps {
    isPinned: ColumnPinningPosition;
    left: number;
    right: number;
    $width: number;
}
export {};
