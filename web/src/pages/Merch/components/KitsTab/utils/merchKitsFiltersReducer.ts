import { DEFAULT_MERCH_KITS_GRID_FILTERS } from '../constants';
import {
	KitsTabColumnsPropsFilters,
	MerchKitsFiltersAction,
	MerchKitsFiltersActionKeys,
} from '../types';

import { FromToProps } from '@/components/Grid';

export const merchStorageFiltersReducer = (
	state: KitsTabColumnsPropsFilters,
	{ type, payload }: MerchKitsFiltersAction
): KitsTabColumnsPropsFilters => {
	switch (type) {
		case MerchKitsFiltersActionKeys.set:
			return payload as KitsTabColumnsPropsFilters;
		case MerchKitsFiltersActionKeys.id:
			return { ...state, idFilter: payload as string };
		case MerchKitsFiltersActionKeys.name:
			return { ...state, nameFilter: payload as string };
		case MerchKitsFiltersActionKeys.status:
			return { ...state, statusFilter: payload as string[] };
		case MerchKitsFiltersActionKeys.description:
			return { ...state, descriptionFilter: payload as string };
		case MerchKitsFiltersActionKeys.storage:
			return { ...state, storageFilter: payload as string[] };
		case MerchKitsFiltersActionKeys.compound:
			return { ...state, compoundFilter: payload as string };
		case MerchKitsFiltersActionKeys.possibleKits:
			return { ...state, possibleKitsFilter: payload as FromToProps<string> };
		case MerchKitsFiltersActionKeys.clear:
			return { ...DEFAULT_MERCH_KITS_GRID_FILTERS };
		default:
			return state;
	}
};
