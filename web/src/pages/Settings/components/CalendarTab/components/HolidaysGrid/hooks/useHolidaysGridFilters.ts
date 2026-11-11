import { useMemo, useReducer } from 'react';

import { defaultHolidaysFiltersValues } from '../constants';
import { generateSetFilters } from '../utils/generateSetFilters';
import { holidaysFiltersReducer } from '../utils/holidaysFiltersReducer';

export const useHolidaysGridFilters = () => {
	const [filters, dispatch] = useReducer(
		holidaysFiltersReducer,
		defaultHolidaysFiltersValues,
		() => defaultHolidaysFiltersValues
	);

	const setFilters = useMemo(
		() => generateSetFilters(filters, dispatch),
		[dispatch]
	);

	return { filters, setFilters };
};
