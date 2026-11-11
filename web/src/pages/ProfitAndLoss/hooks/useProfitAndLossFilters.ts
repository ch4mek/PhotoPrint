import { initialProfitAndLossFilters } from '../constants';
import { ProfitAndLossFilters } from '../types';
import { compareProfitAndLossFilters } from '../utils/compareProfitAndLossFilters';
import { createSetFilters, ratesFiltersReducer } from '../utils/reducer';

import { useFilters } from '@/hooks/useFilters';

export const useProfitAndLossFilters = (initialValues?: ProfitAndLossFilters) =>
	useFilters(
		initialProfitAndLossFilters,
		ratesFiltersReducer,
		createSetFilters,
		initialValues,
		compareProfitAndLossFilters
	);
