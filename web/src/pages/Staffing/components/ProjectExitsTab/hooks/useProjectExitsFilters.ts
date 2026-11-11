import { initialProjectExistsFilters } from '../constants';
import { createSetFilters, projectExitsFiltersReducer } from '../reducer';
import { ProjectExitsFilters } from '../types';
import { compareProjectExitsFilters } from '../utils';

import { useFilters } from '@/hooks/useFilters';

export const useProjectExitsFilters = (initialValues?: ProjectExitsFilters) =>
	useFilters(
		initialProjectExistsFilters,
		projectExitsFiltersReducer,
		createSetFilters,
		initialValues,
		compareProjectExitsFilters
	);
