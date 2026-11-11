import { useMemo, useReducer } from 'react';

import { DEFAULT_MERCH_ELEMENTS_GRID_FILTERS } from '../constants';
import { BodyFilters } from '../types';
import { generateSetFilters } from '../utils/generateSetFilters';
import { getIsAscendingFlagForSorting } from '../utils/getIsAscendingFlagForSorting';
import { merchElementsFiltersReducer } from '../utils/merchElementsFiltersReducer';

import { MerchStorageType } from '@/types/configuration';

export const useElementsFilters = (
	merchStorages: MerchStorageType[],
	order: string,
	orderBy: string
) => {
	const [filters, dispatch] = useReducer(
		merchElementsFiltersReducer,
		DEFAULT_MERCH_ELEMENTS_GRID_FILTERS,
		() => DEFAULT_MERCH_ELEMENTS_GRID_FILTERS
	);
	const setFilters = useMemo(() => generateSetFilters(dispatch), [dispatch]);

	const {
		idFilter,
		nameFilter,
		statusFilter,
		storageFilter,
		supplierFilter,
		descriptionFilter,
	} = filters;

	const responseBody: BodyFilters = useMemo(
		() => ({
			elementId: idFilter || null,
			name: nameFilter || null,
			storagesIds: storageFilter.length
				? storageFilter
				: merchStorages.map(({ id }) => id),
			status: statusFilter || null,
			supplier: supplierFilter || null,
			description: descriptionFilter || null,
			storageSortAscending: getIsAscendingFlagForSorting(
				'storage',
				orderBy,
				order
			),
			minAmountSortAscending: getIsAscendingFlagForSorting(
				'minAmount',
				orderBy,
				order
			),
		}),
		[
			order,
			orderBy,
			idFilter,
			nameFilter,
			storageFilter,
			statusFilter,
			supplierFilter,
			descriptionFilter,
		]
	);

	return {
		responseBodyFilters: responseBody,
		setFilters,
		gridFilters: {
			idFilter,
			nameFilter,
			storageFilter,
			statusFilter,
			supplierFilter,
			descriptionFilter,
		},
	};
};
