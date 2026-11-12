import { ColumnFiltersState } from '@tanstack/react-table';
import { SortDirection, UserData } from './types';
export declare const useUsersQueryMock: (page: number, rowsPerPage: number, order: SortDirection, orderBy: keyof UserData, columnFilters: ColumnFiltersState) => {
    data: UserData[];
    isFetching: boolean;
    total: number;
};
