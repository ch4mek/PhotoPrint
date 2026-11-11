import { useMemo, useReducer } from 'react';

import { DEFAULT_SOFT_SKILLS_FILTER_VALUES } from '../constants';

import {
	generateSetFilters,
	softSkillsFiltersReducer,
} from '@/components/UserProfile/components/SoftSkillsTab/reducer';

export const useSoftSkillsFilters = () => {
	const [filters, dispatch] = useReducer(
		softSkillsFiltersReducer,
		DEFAULT_SOFT_SKILLS_FILTER_VALUES
	);

	const setFilters = useMemo(() => generateSetFilters(dispatch), [dispatch]);

	return { filters, setFilters };
};
