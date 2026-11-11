import { initialLabboardFilters } from '../constants';
import { createSetFilters, labboardFiltersReducer } from '../reducer';
import { LabboardFilters } from '../types';
import { compareLabboardFilters } from '../utils/compareLabboardFilters';

import { useFilters } from '@/hooks/useFilters';

export const useLabboardFilters = (initialValues?: LabboardFilters) =>
	useFilters(
		initialLabboardFilters,
		labboardFiltersReducer,
		createSetFilters,
		initialValues,
		compareLabboardFilters
	);
