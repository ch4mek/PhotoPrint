import { Dispatch, useEffect, useMemo, useReducer } from 'react';

import { areObjectsShallowlyEqual } from '@/helpers/areValuesEqual';
import { Booleanify, SetFilters } from '@/types/helpers';

export const useFilters = <T extends Record<string, any>, A>(
	initialState: T,
	reducer: (state: T, action: A) => T,
	createSetFilters: (dispatch: Dispatch<A>) => SetFilters<T>,
	initialValues?: T,
	compareFilters?: (filtersState: T, initialState: T) => Booleanify<T>
) => {
	const [filtersState, dispatch] = useReducer(reducer, initialState);

	const setFilters = useMemo(
		() => createSetFilters(dispatch),
		[dispatch, createSetFilters]
	);

	const areFiltersAtInitialState = useMemo(
		() =>
			compareFilters
				? compareFilters(filtersState, initialState)
				: areObjectsShallowlyEqual(filtersState, initialState),
		[filtersState, initialState, compareFilters]
	);

	const isResetFiltersDisabled = useMemo(
		() => Object.values(areFiltersAtInitialState).every(Boolean),
		[areFiltersAtInitialState]
	);

	useEffect(() => {
		if (initialValues) {
			setFilters.set(initialValues);
		}
	}, [initialValues, setFilters]);

	return {
		filters: filtersState,
		dispatch,
		setFilters,
		isResetFiltersDisabled,
		areFiltersAtInitialState,
	};
};
