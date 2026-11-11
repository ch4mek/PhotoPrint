import { useMemo, useReducer } from 'react';

import { getStringFilterValues } from '../../../utils/getStringFilterValues';
import {
	DEFAULT_MERCH_STORAGE_GRID_FILTERS,
	STORAGE_TAB_GRID_COLUMNS,
} from '../constants';
import { BodyFilters } from '../types';
import { generateSetFilters } from '../utils/generateSetFilters';
import { merchStorageFiltersReducer } from '../utils/merchStorageFiltersReducer';

import { lowercaseFirstSymbol } from '@/helpers/lowercaseFirstSymbol';

const { id, name, supplier, description } = STORAGE_TAB_GRID_COLUMNS;

export const useStorageFilters = (order: string, orderBy: string) => {
	const [filters, dispatch] = useReducer(
		merchStorageFiltersReducer,
		DEFAULT_MERCH_STORAGE_GRID_FILTERS,
		() => DEFAULT_MERCH_STORAGE_GRID_FILTERS
	);
	const setFilters = useMemo(() => generateSetFilters(dispatch), [dispatch]);

	const {
		idFilter,
		nameFilter,
		statusFilter,
		storageFilter,
		supplierFilter,
		descriptionFilter,
		totalAmountFilter,
	} = filters;

	const responseBody: BodyFilters = useMemo(
		() => ({
			storageIds: storageFilter.length ? storageFilter : null,
			amountMax: Number(totalAmountFilter.to) || null,
			amountMin: Number(totalAmountFilter.from) || null,
			filters: getStringFilterValues([
				[id, idFilter],
				[name, nameFilter],
				[supplier, supplierFilter],
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
			supplierFilter,
			descriptionFilter,
			totalAmountFilter,
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
			totalAmountFilter,
		},
	};
};
