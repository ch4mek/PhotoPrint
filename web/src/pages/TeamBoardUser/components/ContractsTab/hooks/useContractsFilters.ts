import { useMemo, useReducer } from 'react';

import { CONTRACTS_FILTERS_INITIAL_STATE } from '../constants';
import { contractsFiltersReducer, createSetFilters } from '../utils';

import {
	areArraysEqualByPrimitiveItems,
	arePrimitiveValuesEqual,
} from '@/helpers/areValuesEqual';

export const useContractsFilters = () => {
	const [filters, dispatch] = useReducer(
		contractsFiltersReducer,
		CONTRACTS_FILTERS_INITIAL_STATE
	);
	const setFilters = createSetFilters(dispatch);

	const areFiltersApplied = useMemo(
		() =>
			(Object.keys(filters) as Array<keyof typeof filters>).some((key) => {
				if (Array.isArray(filters[key])) {
					return !areArraysEqualByPrimitiveItems(
						filters[key] as string[],
						CONTRACTS_FILTERS_INITIAL_STATE[key] as string[]
					);
				}

				return !arePrimitiveValuesEqual(
					filters[key] as string,
					CONTRACTS_FILTERS_INITIAL_STATE[key] as string
				);
			}),
		[filters, CONTRACTS_FILTERS_INITIAL_STATE]
	);

	return { filters, setFilters, areFiltersApplied };
};
