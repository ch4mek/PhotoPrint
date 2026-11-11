import { DEFAULT_USERS_ACCESS_FILTERS } from '../constants';
import {
	generateSetFilters,
	userAccessFiltersComparator,
	usersAccessFiltersReducer,
} from '../utils';

import { useFilters } from '@/hooks/useFilters';

export const useUsersAccessFilters = (
	initialFilters = DEFAULT_USERS_ACCESS_FILTERS
) =>
	useFilters(
		DEFAULT_USERS_ACCESS_FILTERS,
		usersAccessFiltersReducer,
		generateSetFilters,
		initialFilters,
		userAccessFiltersComparator
	);
