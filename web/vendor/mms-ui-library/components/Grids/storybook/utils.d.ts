import { SortDirection as ExternalSortDirection } from 'react-data-grid';
import { SortDirection } from './constants';
export declare function convertToDataGridSortDirection(direction: SortDirection): ExternalSortDirection;
export declare function convertToNativeSortDirection(direction: ExternalSortDirection): SortDirection;
