import { Dispatch } from 'react';

import {
	MerchStorageFiltersAction,
	MerchStorageFiltersActionKeys,
	MerchStorageGridFilterSetters,
	StorageTabColumnsPropsFilters,
} from '../types';

import { FromToProps } from '@/components/Grid';

export const generateSetFilters = (
	dispatch: Dispatch<MerchStorageFiltersAction>
): MerchStorageGridFilterSetters => ({
	id: (id: string) =>
		dispatch({
			type: MerchStorageFiltersActionKeys.id,
			payload: id,
		}),
	name: (name: string) =>
		dispatch({
			type: MerchStorageFiltersActionKeys.name,
			payload: name,
		}),
	status: (status: string[]) =>
		dispatch({ type: MerchStorageFiltersActionKeys.status, payload: status }),
	storage: (storage: string[]) =>
		dispatch({
			type: MerchStorageFiltersActionKeys.storage,
			payload: storage,
		}),
	description: (description: string) =>
		dispatch({
			type: MerchStorageFiltersActionKeys.description,
			payload: description,
		}),
	supplier: (supplier: string) =>
		dispatch({
			type: MerchStorageFiltersActionKeys.supplier,
			payload: supplier,
		}),
	totalAmount: (payload: FromToProps<string>) =>
		dispatch({
			type: MerchStorageFiltersActionKeys.totalAmount,
			payload,
		}),
	set: (newFilters: StorageTabColumnsPropsFilters) =>
		dispatch({ type: MerchStorageFiltersActionKeys.set, payload: newFilters }),
	clear: () => dispatch({ type: MerchStorageFiltersActionKeys.clear }),
});
