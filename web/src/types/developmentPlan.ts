import { FromToProps } from '@/components/Grid/modals/types';
import { DynamicHeightCellDefaultProps } from '@/components/Grid/renderers/types';

import { FilterReducerAction, FiltersHandlers } from './filter';

export interface UserDevelopmentPlan {
	id: number;
	startDate: string;
	deadlineDate: string | null;
	endDate: string | null;
	status: keyof typeof DevelopmentPlanStatus;
	summary: string;
	notes: string;
	userId: number;
}

export enum DevelopmentPlanStatus {
	None = 'None',
	UnderReview = 'Under review',
	Approved = 'Approved',
	Rejected = 'Rejected',
}

export interface DevelopmentPlanRowContextMenuBindings {
	handleEditOptionClick?(id?: number | string): VoidFunction;
	handleApproveOptionClick?(
		id?: number | string,
		startDate?: string
	): VoidFunction;
	handleRejectOptionClick?(id?: number | string): VoidFunction;
	handleDeleteOptionClick?(
		id?: number | string,
		startDate?: string
	): VoidFunction;
}

type DevelopmentPlanRowContextMenuOperations = {
	[K in keyof DevelopmentPlanRowContextMenuBindings]: VoidFunction;
};

export interface DevelopmentPlanRow
	extends DynamicHeightCellDefaultProps,
		DevelopmentPlanRowContextMenuOperations {
	id: number;
	startDate: string;
	endDate: string | null;
	deadlineDate: string | null;
	summary: string;
	status: DevelopmentPlanStatus;
	notes: string;
}

export type DevelopmentPlanTabTableColumns = Omit<
	DevelopmentPlanRow,
	keyof DynamicHeightCellDefaultProps
>;
export type DevelopmentPlanSectionTableColumns = Pick<
	DevelopmentPlanTabTableColumns,
	'status' | 'deadlineDate' | 'summary'
>;

export enum DevelopmentPlanTableFiltersAction {
	StartDate = 'startDate',
	EndDate = 'endDate',
	DeadlineDate = 'deadlineDate',
	Summary = 'summary',
	Status = 'status',
	Notes = 'notes',
	Reset = 'reset',
}

export interface DevelopmentPlanFilters {
	startDate: FromToProps<Date>;
	endDate: FromToProps<Date>;
	deadlineDate: FromToProps<Date>;
	summary: string;
	status: string[];
	notes: string;
}

export type DevelopmentPlanReducerAction = FilterReducerAction<
	DevelopmentPlanTableFiltersAction,
	DevelopmentPlanFilters
>;

export type DevelopmentPlanFilterHandlers = FiltersHandlers<
	DevelopmentPlanTableFiltersAction,
	DevelopmentPlanFilters
>;
