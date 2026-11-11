import { useMemo, useReducer } from 'react';

import { defaultUserPerformanceReviewFilters } from '../constants';
import { UserPerformanceReviewFiltersReducer } from '../types';
import {
	createUserPerformanceReviewFiltersHandlers,
	userPerformanceReviewReducer,
} from '../utils';

export function usePerformanceReviewFiltersReducer(): UserPerformanceReviewFiltersReducer {
	const [filters, dispatch] = useReducer(
		userPerformanceReviewReducer,
		defaultUserPerformanceReviewFilters
	);

	const setFilters = useMemo(
		() => createUserPerformanceReviewFiltersHandlers(filters, dispatch),
		[filters]
	);

	return [filters, setFilters];
}
