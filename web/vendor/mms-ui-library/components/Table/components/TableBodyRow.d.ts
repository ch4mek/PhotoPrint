import { RowData } from '@tanstack/react-table';
import { TableBodyRowProps } from '../types';
export declare const RowStyleContext: import("react").Context<{
    offsetTop: number;
    height: number;
} | null>;
declare function TableBodyRowInner<TData extends RowData>({ row, start, index, height, }: TableBodyRowProps<TData>): import("react/jsx-runtime").JSX.Element;
export declare const TableBodyRow: import("react").MemoExoticComponent<typeof TableBodyRowInner>;
export {};
