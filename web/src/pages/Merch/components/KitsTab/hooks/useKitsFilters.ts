import { useMemo, useReducer } from 'react';

import { getStringFilterValues } from '../../../utils/getStringFilterValues';
import {
	DEFAULT_MERCH_KITS_GRID_FILTERS,
	KITS_TAB_GRID_COLUMNS,
} from '../constants';
import { BodyFilters } from '../types';
import { generateSetFilters } from '../utils/generateSetFilters';
import { merchStorageFiltersReducer } from '../utils/merchKitsFiltersReducer';

import { lowercaseFirstSymbol } from '@/helpers/lowercaseFirstSymbol';

const { id, name, compound, description } = KITS_TAB_GRID_COLUMNS;

export const useKitsFilters = (order: string, orderBy: string) => {
	const [filters, dispatch] = useReducer(
		merchStorageFiltersReducer,
		DEFAULT_MERCH_KITS_GRID_FILTERS,
		() => DEFAULT_MERCH_KITS_GRID_FILTERS
	);
	const setFilters = useMemo(() => generateSetFilters(dispatch), [dispatch]);

	const {
		idFilter,
		nameFilter,
		statusFilter,
		storageFilter,
		compoundFilter,
		descriptionFilter,
		possibleKitsFilter,
	} = filters;

	const responseBody: BodyFilters = useMemo(
		() => ({
			storageIds: storageFilter.length ? storageFilter : null,
			amountMax: possibleKitsFilter.to ? Number(possibleKitsFilter.to) : null,
			amountMin: possibleKitsFilter.from
				? Number(possibleKitsFilter.from)
				: null,
			filters: getStringFilterValues([
				[id, idFilter],
				[name, nameFilter],
				[compound, compoundFilter],
				[description, descriptionFilter],
			]),
			sortColumn: lowercaseFirstSymbol(orderBy),
			sortOrder: order,
			statusIds: statusFilter.length ? statusFilter : null,
		}),
		[
			order,
			orderBy,
			idFilter,
			nameFilter,
			storageFilter,
			statusFilter,
			compoundFilter,
			descriptionFilter,
			possibleKitsFilter,
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
			compoundFilter,
			descriptionFilter,
			possibleKitsFilter,
		},
	};
};
