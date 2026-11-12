import { SortDirection, UserData } from './types';
export declare const useUsersQueryMock: (page: number, rowsPerPage: number, order: SortDirection, orderBy: keyof UserData) => {
    data: UserData[];
    isFetching: boolean;
    total: number;
};
