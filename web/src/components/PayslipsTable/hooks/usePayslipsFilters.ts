import { useCallback, useEffect, useMemo, useReducer } from 'react';

import { DEFAULT_PAYSLIPS_FILTER_VALUES } from '../constants';
import { generatePayslipsSetFilters } from '../util/generatePayslipsSetFilters';
import { payslipsFiltersReducer } from '../util/payslipsFIltersReducer';

import { areFromToCompatibleObjectsEqual } from '@/helpers/areValuesEqual';

export const usePayslipsFilters = (
	initialFilters = DEFAULT_PAYSLIPS_FILTER_VALUES
) => {
	const [filters, dispatch] = useReducer(
		payslipsFiltersReducer,
		DEFAULT_PAYSLIPS_FILTER_VALUES
	);

	const setFilters = useMemo(
		() => generatePayslipsSetFilters(dispatch),
		[dispatch]
	);

	const areFiltersApplied = useMemo(
		() =>
			(Object.keys(filters) as Array<keyof typeof filters>).some(
				(key) =>
					!areFromToCompatibleObjectsEqual(filters[key], initialFilters[key])
			),
		[filters, initialFilters]
	);

	const resetFilters = useCallback(() => {
		setFilters.set(initialFilters);
	}, [initialFilters]);

	useEffect(() => {
		resetFilters();
	}, [initialFilters]);

	return { filters, setFilters, areFiltersApplied, resetFilters };
};
