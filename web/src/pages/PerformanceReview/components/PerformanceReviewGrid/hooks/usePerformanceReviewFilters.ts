import { useMemo, useReducer } from 'react';

import { defaultPerformanceReviewFilterValues } from '@/pages/PerformanceReview/components/PerformanceReviewGrid/constants';
import {
	generateSetFilters,
	performanceReviewFiltersReducer,
} from '@/pages/PerformanceReview/components/PerformanceReviewGrid/reducer';

export const usePerformanceReviewFilters = () => {
	const [filters, dispatch] = useReducer(
		performanceReviewFiltersReducer,
		defaultPerformanceReviewFilterValues
	);

	const setFilters = useMemo(() => generateSetFilters(dispatch), [dispatch]);

	return { filters, setFilters };
};
