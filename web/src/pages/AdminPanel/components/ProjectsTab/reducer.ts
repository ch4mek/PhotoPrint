import { Dispatch } from 'react';

import { FromToProps } from '@/components/Grid';

import { PROJECT_FILTERS_INITIAL_STATE } from './constants';
import {
	ProjectsFiltersAction,
	ProjectsFiltersActionType,
	ProjectsFilters,
} from './types';

export const projectsFiltersReducer = (
	state: ProjectsFilters,
	action: ProjectsFiltersAction
): ProjectsFilters => {
	switch (action.type) {
		case ProjectsFiltersActionType.SET_PROJECT_FILTER:
			return { ...state, projectFilter: action.payload };
		case ProjectsFiltersActionType.SET_CLIENT_FILTER:
			return { ...state, clientFilter: action.payload };
		case ProjectsFiltersActionType.SET_FACT_TRACKED_TIME_FILTER:
			return { ...state, factTrackedTimeFilter: action.payload };
		case ProjectsFiltersActionType.SET_BILLABLE_TRACKED_TIME_FILTER:
			return { ...state, billableTrackedTimeFilter: action.payload };
		case ProjectsFiltersActionType.SET_STATUS_FILTER:
			return { ...state, statusFilter: action.payload };
		case ProjectsFiltersActionType.SET_PUBLIC_FILTER:
			return { ...state, publicFilter: action.payload };
		case ProjectsFiltersActionType.SET_BILLABLE_FILTER:
			return { ...state, billableFilter: action.payload };
		case ProjectsFiltersActionType.SET_ID_FILTER:
			return { ...state, idFilter: action.payload };
		case ProjectsFiltersActionType.SET:
			return { ...action.payload };
		case ProjectsFiltersActionType.RESET:
			return PROJECT_FILTERS_INITIAL_STATE;
		default:
			return state;
	}
};

export const createSetFilters = (
	dispatch: Dispatch<ProjectsFiltersAction>
) => ({
	projectFilter: (payload: string) =>
		dispatch({ type: ProjectsFiltersActionType.SET_PROJECT_FILTER, payload }),
	clientFilter: (payload: string) =>
		dispatch({ type: ProjectsFiltersActionType.SET_CLIENT_FILTER, payload }),
	factTrackedTimeFilter: (payload: FromToProps<string>) =>
		dispatch({
			type: ProjectsFiltersActionType.SET_FACT_TRACKED_TIME_FILTER,
			payload,
		}),
	billableTrackedTimeFilter: (payload: FromToProps<string>) =>
		dispatch({
			type: ProjectsFiltersActionType.SET_BILLABLE_TRACKED_TIME_FILTER,
			payload,
		}),
	statusFilter: (payload: string[]) =>
		dispatch({ type: ProjectsFiltersActionType.SET_STATUS_FILTER, payload }),
	publicFilter: (payload: string[]) =>
		dispatch({ type: ProjectsFiltersActionType.SET_PUBLIC_FILTER, payload }),
	billableFilter: (payload: string[]) =>
		dispatch({ type: ProjectsFiltersActionType.SET_BILLABLE_FILTER, payload }),
	idFilter: (payload: string) =>
		dispatch({ type: ProjectsFiltersActionType.SET_ID_FILTER, payload }),
	set: (payload: ProjectsFilters) =>
		dispatch({ type: ProjectsFiltersActionType.SET, payload }),
	reset: () => dispatch({ type: ProjectsFiltersActionType.RESET }),
});
