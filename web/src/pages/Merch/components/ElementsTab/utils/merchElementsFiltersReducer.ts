import { DEFAULT_MERCH_ELEMENTS_GRID_FILTERS } from '../constants';
import {
	ElementsTabColumnsPropsFilters,
	MerchElementsFiltersAction,
	MerchElementsFiltersActionKeys,
} from '../types';

export const merchElementsFiltersReducer = (
	state: ElementsTabColumnsPropsFilters,
	{ type, payload }: MerchElementsFiltersAction
): ElementsTabColumnsPropsFilters => {
	switch (type) {
		case MerchElementsFiltersActionKeys.set:
			return payload as ElementsTabColumnsPropsFilters;
		case MerchElementsFiltersActionKeys.id:
			return { ...state, idFilter: payload as string };
		case MerchElementsFiltersActionKeys.name:
			return { ...state, nameFilter: payload as string };
		case MerchElementsFiltersActionKeys.status:
			return { ...state, statusFilter: payload as string[] };
		case MerchElementsFiltersActionKeys.description:
			return { ...state, descriptionFilter: payload as string };
		case MerchElementsFiltersActionKeys.storage:
			return { ...state, storageFilter: payload as string[] };
		case MerchElementsFiltersActionKeys.supplier:
			return { ...state, supplierFilter: payload as string };
		case MerchElementsFiltersActionKeys.clear:
			return { ...DEFAULT_MERCH_ELEMENTS_GRID_FILTERS };
		default:
			return state;
	}
};
