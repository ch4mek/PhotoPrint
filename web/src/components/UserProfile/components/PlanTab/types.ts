import { Dispatch, SetStateAction } from 'react';

import {
	DevelopmentPlanFilters,
	DevelopmentPlanRow,
	DevelopmentPlanTabTableColumns,
} from '@/types/developmentPlan';
import {
	FilterReducer,
	FilterReducerAction,
	FiltersHandlers,
} from '@/types/filter';

export enum UserDevelopmentPlanTableFiltersAction {
	StartDate = 'startDate',
	EndDate = 'endDate',
	DeadlineDate = 'deadlineDate',
	Summary = 'summary',
	Status = 'status',
	Reset = 'reset',
}

export type UserDevelopmentPlanTableColumns = Omit<
	DevelopmentPlanTabTableColumns,
	'notes'
>;

export type UserDevelopmentPlanRow = Omit<DevelopmentPlanRow, 'notes'>;

export type UserDevelopmentPlanFilters = Omit<DevelopmentPlanFilters, 'notes'>;

export type UserDevelopmentPlanFilterHandlers = FiltersHandlers<
	UserDevelopmentPlanTableFiltersAction,
	UserDevelopmentPlanFilters
>;

export type UserDevelopmentPlanReducerAction = FilterReducerAction<
	UserDevelopmentPlanTableFiltersAction,
	UserDevelopmentPlanFilters
>;

export type UserDevelopmentPlanFilterReducer = FilterReducer<
	UserDevelopmentPlanFilters,
	UserDevelopmentPlanFilterHandlers
>;

export type UseDevelopmentPlanRowsHookReturn = [
	UserDevelopmentPlanRow[],
	Dispatch<SetStateAction<UserDevelopmentPlanRow[]>>
];
