import { ColumnFiltersState, SortingState } from '@tanstack/react-table';
import { SortDirection } from '../types';
export declare const useTableState: (initialRowsPerPage?: number, initialOrder?: SortDirection, initialOrderBy?: string) => {
    rowsPerPage: number;
    setRowsPerPage: import("react").Dispatch<import("react").SetStateAction<number>>;
    order: SortDirection;
    setOrder: import("react").Dispatch<import("react").SetStateAction<SortDirection>>;
    orderBy: string;
    setOrderBy: import("react").Dispatch<import("react").SetStateAction<string>>;
    sortColumns: SortingState;
    handleSortColumnChange: import("react").Dispatch<import("react").SetStateAction<SortingState>>;
    columnFilters: ColumnFiltersState;
    handleColumnFiltersChange: import("react").Dispatch<import("react").SetStateAction<ColumnFiltersState>>;
};
