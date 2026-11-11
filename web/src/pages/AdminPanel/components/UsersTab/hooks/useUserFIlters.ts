import { useEffect, useMemo, useReducer } from 'react';

import { UsersFilterType } from '../types';
import { generateSetFilters, usersFiltersReducer } from '../utils';

export const useUserFilters = (initialState: UsersFilterType) => {
	const [filters, dispatch] = useReducer(usersFiltersReducer, initialState);
	const setFilters = useMemo(
		() => generateSetFilters(dispatch, initialState),
		[dispatch]
	);

	useEffect(() => {
		setFilters.reset();
	}, [initialState]);

	return { filters, setFilters };
};
