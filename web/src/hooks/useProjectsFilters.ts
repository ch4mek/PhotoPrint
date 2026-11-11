import { useCallback, useEffect, useMemo, useReducer } from 'react';

import { FromToProps } from '@/components/Grid/modals/types';
import { DEFAULT_PROJECTS_FILTER_VALUES } from '@/constants/filters';
import {
	areArraysEqualByPrimitiveItems,
	areFromToCompatibleObjectsEqual,
	arePrimitiveValuesEqual,
} from '@/helpers/areValuesEqual';
import {
	generateSetFilters,
	projectTableFiltersReducer,
} from '@/helpers/projectsFilter';

export const useProjectsFilters = (
	initialFilters = DEFAULT_PROJECTS_FILTER_VALUES
) => {
	const [filters, dispatch] = useReducer(
		projectTableFiltersReducer,
		initialFilters
	);

	const setFilters = useMemo(() => generateSetFilters(dispatch), [dispatch]);

	const areFiltersApplied = useMemo(
		() =>
			(Object.keys(filters) as Array<keyof typeof filters>).some((key) => {
				if (Array.isArray(filters[key])) {
					return !areArraysEqualByPrimitiveItems(
						filters[key] as string[],
						initialFilters[key] as string[]
					);
				}

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

	return { filters, setFilters, areFiltersApplied, resetFilters };
};
