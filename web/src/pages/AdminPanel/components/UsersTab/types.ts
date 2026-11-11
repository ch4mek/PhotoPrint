import { SetStateAction } from 'react';

import { SelectionModalOption } from '@/components/Grid/renderers/header/types';
import { SortDirection } from '@/constants/sort';
import { Dayjs } from '@/lib/dayjs';
import { ByPageResponse } from '@/queries/types';
import { LocationType, RoleType } from '@/types/configuration';
import { DepartmentResponse } from '@/types/department';
import { GridColumnWidthsConfiguration } from '@/types/grid';
import { TooltipContent } from '@/types/hooks';
import {
	LookupUserResponse,
	UsersGridRow,
	UseUsersResponse,
} from '@/types/userInfo';

import { generateSetFilters } from './utils';

export enum UserFiltersNames {
	ID = 'id',
	USER = 'user',
	EMAIL = 'email',
	POSITION = 'position',
	DEPARTMENT = 'department',
	GROUP = 'group',
	LOCATION = 'location',
	RESET = 'reset',
	STATUS = 'status',
	APPLY_ALL = 'applyAll',
}

export interface UsersOptions {
	location: Array<LocationType>;
	department: Array<SelectionModalOption>;
	position: Array<RoleType>;
	group: string[];
	users: string[];
}

export interface UsersFilterType {
	id: string;
	user: string;
	email: string;
	position: string[];
	departments: string[];
	group: string;
	location: string[];
	status: string[];
}

export interface UsersAction {
	type: string;
	payload?: string[] | string | UsersFilterType;
}

export interface PositionOption {
	disabled: boolean;
	id: string;
	name: string;
	description: string;
	location: unknown;
}

export interface UseGridOptionsProps {
	departmentsInfo?: DepartmentResponse[];
	users?: LookupUserResponse[];
	hasUsersModuleAccess: boolean;
}

export interface UsersColumnsParams {
	filters: UsersFilterType;
	setFilters: ReturnType<typeof generateSetFilters>;
	options: UsersOptions;
	initialFilters: UsersFilterType;
	positionOptions: PositionOption[];
	departmentOptions: SelectionModalOption[];
	usersColumnsWidths: GridColumnWidthsConfiguration;
	setTooltipContent: (value: SetStateAction<TooltipContent>) => void;
	onCellClick: (id?: string | number) => VoidFunction | undefined;
	enableCellClick: (row: UsersGridRow) => boolean;
	onToggleUserActive: (row: UsersGridRow) => void;
}

export interface UsersRowsParams {
	items: UseUsersResponse[] | undefined;
	dataUpdatedAt: Dayjs | Date | number;
	page: number;
	rowsPerPage: number;
	showSkeleton: boolean;
	handleEdit: (id?: string | number) => () => void;
	handlePageOpen: (id?: string | number) => VoidFunction | undefined;
	hasUsersModuleAccess: boolean;
	hasManageAllInternsClaim: boolean;
}

export type UseUserFormDefaultValuesProps = {
	isEditOpen: boolean;
	data?: ByPageResponse<UseUsersResponse>;
	editId?: string | number;
	departmentsInfo?: DepartmentResponse[];
};

export type UseUsersMutationsProps = {
	refetch: VoidFunction;
	refetchUsersLookup: VoidFunction;
	page: number;
	rowsPerPage: number | string;
	order: SortDirection;
	orderBy: string;
};
