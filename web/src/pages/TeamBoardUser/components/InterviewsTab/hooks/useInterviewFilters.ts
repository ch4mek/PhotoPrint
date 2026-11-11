import { DEFAULT_INTERVIEW_FILTERS } from '../constants';
import {
	generateInterviewSetFilters,
	interviewFiltersComparator,
	interviewFiltersReducer,
} from '../utils';

import { useFilters } from '@/hooks/useFilters';

export const useInterviewFilters = (
	initialFilters = DEFAULT_INTERVIEW_FILTERS
) =>
	useFilters(
		DEFAULT_INTERVIEW_FILTERS,
		interviewFiltersReducer,
		generateInterviewSetFilters,
		initialFilters,
		interviewFiltersComparator
	);
