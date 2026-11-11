import { DEFAULT_MERCH_STORAGE_GRID_FILTERS } from '../constants';
import {
	MerchStorageFiltersAction,
	MerchStorageFiltersActionKeys,
	StorageTabColumnsPropsFilters,
} from '../types';

import { FromToProps } from '@/components/Grid';

export const merchStorageFiltersReducer = (
	state: StorageTabColumnsPropsFilters,
	{ type, payload }: MerchStorageFiltersAction
): StorageTabColumnsPropsFilters => {
	switch (type) {
		case MerchStorageFiltersActionKeys.set:
			return payload as StorageTabColumnsPropsFilters;
		case MerchStorageFiltersActionKeys.id:
			return { ...state, idFilter: payload as string };
		case MerchStorageFiltersActionKeys.name:
			return { ...state, nameFilter: payload as string };
		case MerchStorageFiltersActionKeys.status:
			return { ...state, statusFilter: payload as string[] };
		case MerchStorageFiltersActionKeys.description:
			return { ...state, descriptionFilter: payload as string };
		case MerchStorageFiltersActionKeys.storage:
			return { ...state, storageFilter: payload as string[] };
		case MerchStorageFiltersActionKeys.supplier:
			return { ...state, supplierFilter: payload as string };
		case MerchStorageFiltersActionKeys.totalAmount:
			return { ...state, totalAmountFilter: payload as FromToProps<string> };
		case MerchStorageFiltersActionKeys.clear:
			return { ...DEFAULT_MERCH_STORAGE_GRID_FILTERS };
		default:
			return state;
	}
};
