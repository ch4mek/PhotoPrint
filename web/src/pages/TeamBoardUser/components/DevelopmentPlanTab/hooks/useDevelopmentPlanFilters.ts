import { useMemo, useReducer } from 'react';

import { defaultDevelopmentPlanFilterValues } from '../constants';
import { generateSetFilters, developmentPlanFiltersReducer } from '../utils';

import { FromToProps } from '@/components/Grid';
import {
	areArraysEqualByPrimitiveItems,
	areFromToCompatibleObjectsEqual,
	arePrimitiveValuesEqual,
} from '@/helpers/areValuesEqual';

export function useDevelopmentPlanFilters() {
	const [filters, dispatch] = useReducer(
		developmentPlanFiltersReducer,
		defaultDevelopmentPlanFilterValues
	);

	const setFilters = useMemo(
		() => generateSetFilters(filters, dispatch),
		[filters]
	);

	const areFiltersApplied = useMemo(
		() =>
			(Object.keys(filters) as Array<keyof typeof filters>).some((key) => {
				if (Array.isArray(filters[key])) {
					return !areArraysEqualByPrimitiveItems(
						filters[key] as string[],
						defaultDevelopmentPlanFilterValues[key] as string[]
					);
				}

				if (typeof filters[key] === 'object') {
					return !areFromToCompatibleObjectsEqual(
						filters[key] as FromToProps<Date>,
						defaultDevelopmentPlanFilterValues[key] as FromToProps<Date>
					);
				}

				return !arePrimitiveValuesEqual(
					filters[key] as string,
					defaultDevelopmentPlanFilterValues[key] as string
				);
			}),
		[filters]
	);

	return {
		filters,
		setFilters,
		areFiltersApplied,
	};
}
