import { useMemo, useReducer } from 'react';

import { defaultDevelopmentPlanFilterValues } from '../constants';
import { UserDevelopmentPlanFilterReducer } from '../types';
import {
	createDevelopmentPlanFilters,
	developmentPlanFiltersReducer,
} from '../utils';

export function useDevelopmentPlanFiltersReducer(): UserDevelopmentPlanFilterReducer {
	const [filters, dispatch] = useReducer(
		developmentPlanFiltersReducer,
		defaultDevelopmentPlanFilterValues
	);

	const setFilters = useMemo(
		() => createDevelopmentPlanFilters(filters, dispatch),
		[filters]
	);

	return [filters, setFilters];
}
