import { useMemo, useReducer } from 'react';

import { defaultUserSalaryFilters } from '../constants';
import { UserSalaryFiltersReducer } from '../types';
import { createUserSalaryFiltersHandlers, userSalaryReducer } from '../utils';

export function useSalaryFiltersReducer(): UserSalaryFiltersReducer {
	const [filters, dispatch] = useReducer(
		userSalaryReducer,
		defaultUserSalaryFilters
	);

	const setFilters = useMemo(
		() => createUserSalaryFiltersHandlers(filters, dispatch),
		[filters]
	);

	return [filters, setFilters];
}
