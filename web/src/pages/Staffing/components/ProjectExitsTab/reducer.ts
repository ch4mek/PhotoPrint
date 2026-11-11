import { Dispatch } from 'react';

import { FromToProps } from '@/components/Grid';

import { initialProjectExistsFilters } from './constants';
import {
	ProjectExitsFilters,
	ProjectExitsFiltersAction,
	ProjectExitsFiltersActionType,
} from './types';

export const projectExitsFiltersReducer = (
	state: ProjectExitsFilters,
	action: ProjectExitsFiltersAction
): ProjectExitsFilters => {
	switch (action.type) {
		case ProjectExitsFiltersActionType.SetIdFilter:
			return { ...state, idFilter: action.payload };

		case ProjectExitsFiltersActionType.SetUserFilter:
			return { ...state, userFilter: action.payload };

		case ProjectExitsFiltersActionType.SetDepartmentFilter:
			return { ...state, departmentFilter: action.payload };

		case ProjectExitsFiltersActionType.SetGroupFilter:
			return { ...state, groupFilter: action.payload };

		case ProjectExitsFiltersActionType.SetClientFilter:
			return { ...state, clientFilter: action.payload };

		case ProjectExitsFiltersActionType.SetProjectFilter:
			return { ...state, projectFilter: action.payload };

		case ProjectExitsFiltersActionType.SetClosingDateFilter:
			return { ...state, closingDateFilter: action.payload };

		case ProjectExitsFiltersActionType.SetClosingReasonFilter:
			return { ...state, closingReasonFilter: action.payload };

		case ProjectExitsFiltersActionType.SetStatusFilter:
			return { ...state, statusFilter: action.payload };

		case ProjectExitsFiltersActionType.Set:
			return { ...action.payload };

		case ProjectExitsFiltersActionType.Reset:
			return initialProjectExistsFilters;

		default:
			return state;
	}
};

export const createSetFilters = (
	dispatch: Dispatch<ProjectExitsFiltersAction>
) => ({
	idFilter: (payload: string) =>
		dispatch({ type: ProjectExitsFiltersActionType.SetIdFilter, payload }),

	userFilter: (payload: string) =>
		dispatch({ type: ProjectExitsFiltersActionType.SetUserFilter, payload }),

	departmentFilter: (payload: string[]) =>
		dispatch({
			type: ProjectExitsFiltersActionType.SetDepartmentFilter,
			payload,
		}),

	groupFilter: (payload: string) =>
		dispatch({ type: ProjectExitsFiltersActionType.SetGroupFilter, payload }),

	clientFilter: (payload: string) =>
		dispatch({
			type: ProjectExitsFiltersActionType.SetClientFilter,
			payload,
		}),

	projectFilter: (payload: string) =>
		dispatch({
			type: ProjectExitsFiltersActionType.SetProjectFilter,
			payload,
		}),

	closingDateFilter: (payload: FromToProps<Date>) =>
		dispatch({
			type: ProjectExitsFiltersActionType.SetClosingDateFilter,
			payload,
		}),

	closingReasonFilter: (payload: string) =>
		dispatch({
			type: ProjectExitsFiltersActionType.SetClosingReasonFilter,
			payload,
		}),

	statusFilter: (payload: string[]) =>
		dispatch({
			type: ProjectExitsFiltersActionType.SetStatusFilter,
			payload,
		}),

	set: (payload: ProjectExitsFilters) =>
		dispatch({ type: ProjectExitsFiltersActionType.Set, payload }),

	reset: () => dispatch({ type: ProjectExitsFiltersActionType.Reset }),
});
