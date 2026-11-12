import { Column } from 'react-data-grid';
import { UsersGridRow } from './types';
export declare const headCells: readonly Column<UsersGridRow>[];
export declare enum SortDirection {
    ASC = "asc",
    DESC = "desc"
}
export declare const INITIAL_ORDER_BY_VALUE = "id";
