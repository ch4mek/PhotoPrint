import { Dispatch } from 'react';

import {
	ElementsTabColumnsPropsFilters,
	MerchElementsFiltersAction,
	MerchElementsFiltersActionKeys,
	MerchElementsGridFilterSetters,
} from '../types';

export const generateSetFilters = (
	dispatch: Dispatch<MerchElementsFiltersAction>
): MerchElementsGridFilterSetters => ({
	id: (id: string) =>
		dispatch({
			type: MerchElementsFiltersActionKeys.id,
			payload: id,
		}),
	name: (name: string) =>
		dispatch({
			type: MerchElementsFiltersActionKeys.name,
			payload: name,
		}),
	status: (status: string[]) =>
		dispatch({ type: MerchElementsFiltersActionKeys.status, payload: status }),
	storage: (storage: string[]) =>
		dispatch({
			type: MerchElementsFiltersActionKeys.storage,
			payload: storage,
		}),
	description: (description: string) =>
		dispatch({
			type: MerchElementsFiltersActionKeys.description,
			payload: description,
		}),
	supplier: (supplier: string) =>
		dispatch({
			type: MerchElementsFiltersActionKeys.supplier,
			payload: supplier,
		}),
	set: (newFilters: ElementsTabColumnsPropsFilters) =>
		dispatch({ type: MerchElementsFiltersActionKeys.set, payload: newFilters }),
	clear: () => dispatch({ type: MerchElementsFiltersActionKeys.clear }),
});
