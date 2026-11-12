import { ColumnDef } from '@tanstack/react-table';
import { GridRowDefaultProps } from './GridRenderers/types';
export interface UsersGridRow extends GridRowDefaultProps {
    fullName: string;
    id?: string | number;
    email: string;
    position: string;
    department: string;
    location: string;
    color: string;
}
export interface UseUsersReturnType {
    rows: UsersGridRow[];
    headCells: ColumnDef<UsersGridRow>[];
}
export interface UserData {
    id: number;
    lastName: string;
    firstName: string;
    fullName: string;
    email: string;
    position: string;
    rightPosition: string;
    avatarColor?: string | null;
    avatarFileId?: null;
    timeZone?: string;
    color: string;
    department: string | null;
    rightDepartment: string;
    location: string;
}
export declare enum SortDirection {
    ASC = "asc",
    DESC = "desc"
}
