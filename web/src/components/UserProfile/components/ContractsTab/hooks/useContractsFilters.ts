import { useMemo, useReducer } from 'react';

import { contractsFiltersInitialState } from '../constants';
import { contractsFiltersReducer, createSetFilters } from '../utils';

import {
	areFromToCompatibleObjectsEqual,
	arePrimitiveValuesEqual,
} from '@/helpers/areValuesEqual';

export const useContractsFilters = () => {
	const [filters, dispatch] = useReducer(
		contractsFiltersReducer,
		contractsFiltersInitialState
	);
	const setFilters = createSetFilters(dispatch);

	const areFiltersApplied = useMemo(
		() =>
			(Object.keys(filters) as Array<keyof typeof filters>).some((key) => {
				if (
					typeof filters[key] === 'object' &&
					typeof contractsFiltersInitialState[key] === 'object'
				) {
					return !areFromToCompatibleObjectsEqual(
						filters[key],
						contractsFiltersInitialState[key]
					);
				}

				return !arePrimitiveValuesEqual(
					filters[key] as string,
					contractsFiltersInitialState[key] as string
				);
			}),
		[filters]
	);

	return { filters, setFilters, areFiltersApplied };
};
