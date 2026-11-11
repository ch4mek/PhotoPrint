import { useEffect, useMemo, useReducer } from 'react';

import { DEFAULT_REQUESTS_FILTERS } from '../constants';
import { RequestFilterType } from '../types';
import { generateSetFilters, requestFiltersReducer } from '../utils';

export const useRequestsFilters = (initialState: RequestFilterType) => {
	const [filters, dispatch] = useReducer(
		requestFiltersReducer,
		DEFAULT_REQUESTS_FILTERS
	);
	const setFilters = useMemo(() => generateSetFilters(dispatch), [dispatch]);

	useEffect(() => {
		setFilters.reset(initialState);
	}, [initialState]);

	return { filters, setFilters };
};
