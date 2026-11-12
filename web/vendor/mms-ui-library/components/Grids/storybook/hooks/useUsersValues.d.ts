import { UseUsersReturnType, UserData } from '../types';
export declare const useUsersValues: (data: UserData[] | undefined, page: number, rowsPerPage: number, showSkeleton: boolean, handleEdit: (id?: string | number) => () => void) => UseUsersReturnType;
