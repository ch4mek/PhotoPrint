import { useCallback, useEffect, useMemo, useReducer } from 'react';

import { DEFAULT_SALARY_FILTER_VALUES } from '../constants';
import { generateSetFilters, salaryFiltersReducer } from '../utils';

import { FromToProps } from '@/components/Grid';
import { areFromToCompatibleObjectsEqual } from '@/helpers/areValuesEqual';

export const useSalaryFilters = (
	initialFilters = DEFAULT_SALARY_FILTER_VALUES
) => {
	const [filters, dispatch] = useReducer(
		salaryFiltersReducer,
		DEFAULT_SALARY_FILTER_VALUES
	);

	const setFilters = useMemo(() => generateSetFilters(dispatch), [dispatch]);

	const areFiltersApplied = useMemo(
		() =>
			(Object.keys(filters) as Array<keyof typeof filters>).some(
				(key) =>
					!areFromToCompatibleObjectsEqual(
						filters[key] as FromToProps<string | Date>,
						initialFilters[key] as FromToProps<string | Date>
					)
			),
		[filters, initialFilters]
	);

	useEffect(() => {
		setFilters.set(initialFilters);
	}, [initialFilters]);

	const resetFilters = useCallback(() => {
		setFilters.set(initialFilters);
	}, [initialFilters]);

	return { filters, setFilters, areFiltersApplied, resetFilters };
};
