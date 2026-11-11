import { useMemo, useReducer } from 'react';

import { DEFAULT_USER_CLAIMS_FILTER_VALUES } from '../constants';
import { generateSetFilters, userClaimsFiltersReducer } from '../reducer';

export const useUserClaimsGridFilters = () => {
	const [filters, dispatch] = useReducer(
		userClaimsFiltersReducer,
		DEFAULT_USER_CLAIMS_FILTER_VALUES
	);

	const setFilters = useMemo(() => generateSetFilters(dispatch), [dispatch]);

	return { filters, setFilters };
};
