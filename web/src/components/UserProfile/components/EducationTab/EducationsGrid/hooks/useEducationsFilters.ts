import { useMemo, useReducer } from 'react';

import { DEFAULT_EDUCATIONS_FILTER_VALUES } from '../constants';
import { educationsFiltersReducer } from '../utils/educationsFiltersReducer';
import { generateSetFilters } from '../utils/generateSetFilters';

export const useEducationsFilters = () => {
	const [filters, dispatch] = useReducer(
		educationsFiltersReducer,
		DEFAULT_EDUCATIONS_FILTER_VALUES,
		() => DEFAULT_EDUCATIONS_FILTER_VALUES
	);

	const setFilters = useMemo(() => generateSetFilters(dispatch), [dispatch]);

	return { filters, setFilters };
};
