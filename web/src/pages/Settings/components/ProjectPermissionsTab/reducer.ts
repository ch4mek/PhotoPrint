import { Dispatch } from 'react';

import { projectPermissionsTabFiltersInitialState } from './constants';
import {
	ProjectPermissionsFilters,
	ProjectPermissionsFilterAction,
	ProjectPermissionsFilterActionType,
} from './types';

export const projectPermissionsFilterReducer = (
	state: ProjectPermissionsFilters,
	action: ProjectPermissionsFilterAction
) => {
	switch (action.type) {
		case ProjectPermissionsFilterActionType.SET_USER_ID_FILTER:
			return { ...state, userIdFilter: action.payload };
		case ProjectPermissionsFilterActionType.SET_USER_FULLNAME_FILTER:
			return { ...state, userFullNameFilter: action.payload };
		case ProjectPermissionsFilterActionType.SET_PROJECT_NAME_FILTER:
			return { ...state, projectNameFilter: action.payload };
		case ProjectPermissionsFilterActionType.APPLY_FILTERS:
			return { ...action.payload };
		case ProjectPermissionsFilterActionType.RESET_FILTERS:
			return projectPermissionsTabFiltersInitialState;
		default:
			return state;
	}
};

export const createSetFilters = (
	dispatch: Dispatch<ProjectPermissionsFilterAction>
) => ({
	userIdFilter: (payload: string) =>
		dispatch({
			type: ProjectPermissionsFilterActionType.SET_USER_ID_FILTER,
			payload,
		}),
	userFullNameFilter: (payload: string) =>
		dispatch({
			type: ProjectPermissionsFilterActionType.SET_USER_FULLNAME_FILTER,
			payload,
		}),
	projectNameFilter: (payload: string) =>
		dispatch({
			type: ProjectPermissionsFilterActionType.SET_PROJECT_NAME_FILTER,
			payload,
		}),

	applyFilters: (payload: ProjectPermissionsFilters) =>
		dispatch({
			type: ProjectPermissionsFilterActionType.APPLY_FILTERS,
			payload,
		}),
	resetFilters: () =>
		dispatch({ type: ProjectPermissionsFilterActionType.RESET_FILTERS }),
});
