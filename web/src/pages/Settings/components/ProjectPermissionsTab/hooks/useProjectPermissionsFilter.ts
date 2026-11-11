import { useEffect, useMemo, useReducer } from 'react';

import { projectPermissionsTabFiltersInitialState } from '../constants';
import { projectPermissionsFilterReducer, createSetFilters } from '../reducer';
import { ProjectPermissionsFilters } from '../types';

export const useProjectPermissionsFilter = (
	initialValues?: ProjectPermissionsFilters
) => {
	const [filtersState, dispatch] = useReducer(
		projectPermissionsFilterReducer,
		projectPermissionsTabFiltersInitialState
	);

	const setFilters = createSetFilters(dispatch);

	const { userIdFilter, userFullNameFilter, projectNameFilter } = filtersState;

	const areFiltersAtInitialState = useMemo(
		() => ({
			userIdFilter:
				userIdFilter === projectPermissionsTabFiltersInitialState.userIdFilter,
			userFullNameFilter:
				userFullNameFilter ===
				projectPermissionsTabFiltersInitialState.userFullNameFilter,
			projectNameFilter:
				projectNameFilter ===
				projectPermissionsTabFiltersInitialState.projectNameFilter,
		}),
		[userIdFilter, userFullNameFilter, projectNameFilter]
	);

	const isResetFiltersDisabled = useMemo(
		() => Object.values(areFiltersAtInitialState).every(Boolean),
		[areFiltersAtInitialState]
	);

	useEffect(() => {
		if (initialValues) {
			setFilters.applyFilters(initialValues);
		}
	}, [initialValues]);

	return {
		filters: filtersState,
		dispatch,
		setFilters,
		isResetFiltersDisabled,
	};
};
