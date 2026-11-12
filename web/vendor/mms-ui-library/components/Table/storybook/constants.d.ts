import { ColumnDef } from '@tanstack/react-table';
import { UsersGridRow } from './types';
export declare const columns: ColumnDef<UsersGridRow>[];
export declare enum SortDirection {
    ASC = "asc",
    DESC = "desc"
}
export declare const INITIAL_ORDER_BY_VALUE = "id";
