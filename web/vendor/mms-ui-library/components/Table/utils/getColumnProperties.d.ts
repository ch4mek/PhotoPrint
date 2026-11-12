import { Column, RowData } from '@tanstack/react-table';
export declare const getColumnProperties: <TData extends RowData>(column: Column<TData, unknown>) => {
    isPinned: import("@tanstack/react-table").ColumnPinningPosition;
    left: number;
    right: number;
    $width: number;
};
