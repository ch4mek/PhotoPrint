import { useEffect, useMemo, useReducer } from 'react';

import { TeamboardFilterType } from '../types';
import { generateSetFilters, teamboardFiltersReducer } from '../utils';

export const useTeamboardFilters = (initialState: TeamboardFilterType) => {
	const [filters, dispatch] = useReducer(teamboardFiltersReducer, initialState);
	const setFilters = useMemo(() => generateSetFilters(dispatch), [dispatch]);

	useEffect(() => {
		setFilters.reset(initialState);
	}, [initialState]);

	return { filters, setFilters };
};
