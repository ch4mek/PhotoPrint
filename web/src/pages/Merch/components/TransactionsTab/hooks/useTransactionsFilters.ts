import { useMemo, useReducer } from 'react';

import { DEFAULT_MERCH_TRANSACTIONS_GRID_FILTERS } from '../constants';
import { BodyFilters } from '../types';
import { generateSetFilters } from '../utils/generateSetFilters';
import { merchTransactionsFiltersReducer } from '../utils/merchTransactionsFiltersReducer';

export const useTransactionsFilters = () => {
	const [filters, dispatch] = useReducer(
		merchTransactionsFiltersReducer,
		DEFAULT_MERCH_TRANSACTIONS_GRID_FILTERS,
		() => DEFAULT_MERCH_TRANSACTIONS_GRID_FILTERS
	);
	const setFilters = useMemo(() => generateSetFilters(dispatch), [dispatch]);

	const { idFilter } = filters;

	const responseBody: BodyFilters = useMemo(
		() => ({
			idFrom: idFilter.length ? idFilter : null,
			idTo: idFilter.length ? idFilter : null,
		}),
		[idFilter]
	);

	return {
		responseBodyFilters: responseBody,
		setFilters,
		gridFilters: filters,
	};
};
