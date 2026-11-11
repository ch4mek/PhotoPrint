import { useMemo, useReducer } from 'react';

import { DEFAULT_SPECIALIZATION_FILTER_VALUES } from '../constants';
import {
	generateSetFilters,
	specializationTableFiltersReducer,
} from '../utils';

export function useSpecializationFilters() {
	const [filters, dispatch] = useReducer(
		specializationTableFiltersReducer,
		DEFAULT_SPECIALIZATION_FILTER_VALUES
	);

	const setFilters = useMemo(() => generateSetFilters(dispatch), [dispatch]);

	return { filters, setFilters };
}
