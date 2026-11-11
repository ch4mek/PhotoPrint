import { FromToProps } from '@/components/Grid';
import { SelectionModalOption } from '@/components/Grid/renderers/header/types';
import { Dayjs } from '@/lib/dayjs';
import { HandleRowClick } from '@/types/grid';
import { SetFilters } from '@/types/helpers';
import { ProjectExitEntity, ProjectExitsGridRow } from '@/types/projectExits';

export interface ProjectExitsFilters {
	idFilter: string;
	userFilter: string;
	departmentFilter: string[];
	groupFilter: string;
	clientFilter: string;
	projectFilter: string;
	closingDateFilter: FromToProps<Date>;
	closingReasonFilter: string;
	statusFilter: string[];
}

interface ProjectExitsSetFilters extends SetFilters<ProjectExitsFilters> {}

export enum ProjectExitsFiltersActionType {
	SetIdFilter = 'SetIdFilter',
	SetUserFilter = 'SetUserFilter',
	SetDepartmentFilter = 'SetDepartmentFilter',
	SetGroupFilter = 'SetGroupFilter',
	SetClientFilter = 'SetClientFilter',
	SetProjectFilter = 'SetProjectFilter',
	SetClosingDateFilter = 'SetClosingDateFilter',
	SetClosingReasonFilter = 'SetClosingReasonFilter',
	SetStatusFilter = 'SetStatusFilter',
	Reset = 'Reset',
	Set = 'Set',
}

export type ProjectExitsFiltersAction =
	| { type: ProjectExitsFiltersActionType.SetIdFilter; payload: string }
	| { type: ProjectExitsFiltersActionType.SetUserFilter; payload: string }
	| {
			type: ProjectExitsFiltersActionType.SetDepartmentFilter;
			payload: string[];
	  }
	| { type: ProjectExitsFiltersActionType.SetGroupFilter; payload: string }
	| { type: ProjectExitsFiltersActionType.SetClientFilter; payload: string }
	| { type: ProjectExitsFiltersActionType.SetProjectFilter; payload: string }
	| {
			type: ProjectExitsFiltersActionType.SetClosingDateFilter;
			payload: FromToProps<Date>;
	  }
	| {
			type: ProjectExitsFiltersActionType.SetClosingReasonFilter;
			payload: string;
	  }
	| { type: ProjectExitsFiltersActionType.SetStatusFilter; payload: string[] }
	| { type: ProjectExitsFiltersActionType.Set; payload: ProjectExitsFilters }
	| { type: ProjectExitsFiltersActionType.Reset };

export interface UseProjectExitsRowsProps {
	items?: Array<ProjectExitEntity>;
	dataUpdatedAt: Dayjs | Date | number;
	page: number;
	rowsPerPage: number;
	isDataLoading: boolean;
}

export interface ProjectExitsGridOptions {
	users: string[];
	departments: SelectionModalOption[];
	groups: string[];
	clients: string[];
	projects: string[];
	reasons: string[];
}

export interface UseProjectExitsProps {
	filters: ProjectExitsFilters;
	setFilters: ProjectExitsSetFilters;
	isResetFiltersDisabled: boolean;
	options: ProjectExitsGridOptions;
	canManageProjectExits: boolean;
	onCellClick: HandleRowClick<ProjectExitsGridRow>;
	handleDelete: HandleRowClick<ProjectExitsGridRow>;
	handleRestore: HandleRowClick<ProjectExitsGridRow>;
	handleEdit: HandleRowClick<ProjectExitsGridRow>;
}

export interface ProjectExitReason {
	id: number;
	name: string;
}
