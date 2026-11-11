import { useCallback, useEffect, useMemo, useReducer } from 'react';

import { DEFAULT_PERFORMANCE_FILTER_VALUES } from '../constants';

import { FromToProps } from '@/components/Grid';
import { SortDirection } from '@/constants/sort';
import {
	areFromToCompatibleObjectsEqual,
	arePrimitiveValuesEqual,
} from '@/helpers/areValuesEqual';
import { useTableState } from '@/pages/hooks/useTableState';
import {
	generateSetFilters,
	performanceFiltersReducer,
} from '@/pages/TeamBoardUser/components/PerformanceReviewTab/utils';

export const usePerformanceReviewFilters = (
	initialFilters = DEFAULT_PERFORMANCE_FILTER_VALUES
) => {
	const [filters, dispatch] = useReducer(
		performanceFiltersReducer,
		DEFAULT_PERFORMANCE_FILTER_VALUES
	);

	const {
		order,
		orderBy,
		handleSortColumnChange,
		sortColumns,
		page,
		setPage,
		rowsPerPage,
		handleOnPerPageChange,
	} = useTableState({
		initialOrder: SortDirection.DESC,
		initialOrderBy: 'performanceReviewDate',
	});

	const setFilters = useMemo(() => generateSetFilters(dispatch), [dispatch]);

	const areFiltersApplied = useMemo(
		() =>
			(Object.keys(filters) as Array<keyof typeof filters>).some((key) => {
				if (typeof filters[key] === 'object') {
					return !areFromToCompatibleObjectsEqual(
						filters[key] as FromToProps<Date>,
						initialFilters[key] as FromToProps<Date>
					);
				}

				return !arePrimitiveValuesEqual(
					filters[key] as string,
					initialFilters[key] as string
				);
			}),
		[filters, initialFilters]
	);

	useEffect(() => {
		setFilters.set(initialFilters);
	}, [initialFilters]);

	const resetFilters = useCallback(() => {
		setFilters.set(initialFilters);
	}, [initialFilters]);

	return {
		page,
		setPage,
		filters,
		setFilters,
		areFiltersApplied,
		rowsPerPage,
		resetFilters,
		order,
		orderBy,
		handleSortColumnChange,
		sortColumns,
		handleOnPerPageChange,
	};
};
