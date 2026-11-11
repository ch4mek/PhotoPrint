import { PROJECT_FILTERS_INITIAL_STATE } from '../constants';
import { createSetFilters, projectsFiltersReducer } from '../reducer';
import { ProjectsFilters } from '../types';
import { compareProjectsFilters } from '../utils';

import { useFilters } from '@/hooks/useFilters';

export const useProjectsFilters = (initialValues?: ProjectsFilters) =>
	useFilters(
		PROJECT_FILTERS_INITIAL_STATE,
		projectsFiltersReducer,
		createSetFilters,
		initialValues,
		compareProjectsFilters
	);
