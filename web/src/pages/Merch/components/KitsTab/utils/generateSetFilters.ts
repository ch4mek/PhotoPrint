import { Dispatch } from 'react';

import {
	MerchKitsFiltersActionKeys,
	MerchKitsGridFilterSetters,
	KitsTabColumnsPropsFilters,
	MerchKitsFiltersAction,
} from '../types';

import { FromToProps } from '@/components/Grid';

export const generateSetFilters = (
	dispatch: Dispatch<MerchKitsFiltersAction>
): MerchKitsGridFilterSetters => ({
	id: (id: string) =>
		dispatch({
			type: MerchKitsFiltersActionKeys.id,
			payload: id,
		}),
	name: (name: string) =>
		dispatch({
			type: MerchKitsFiltersActionKeys.name,
			payload: name,
		}),
	status: (status: string[]) =>
		dispatch({ type: MerchKitsFiltersActionKeys.status, payload: status }),
	storage: (storage: string[]) =>
		dispatch({
			type: MerchKitsFiltersActionKeys.storage,
			payload: storage,
		}),
	description: (description: string) =>
		dispatch({
			type: MerchKitsFiltersActionKeys.description,
			payload: description,
		}),
	compound: (supplier: string) =>
		dispatch({
			type: MerchKitsFiltersActionKeys.compound,
			payload: supplier,
		}),
	possibleKits: (payload: FromToProps<string>) =>
		dispatch({
			type: MerchKitsFiltersActionKeys.possibleKits,
			payload,
		}),
	set: (newFilters: KitsTabColumnsPropsFilters) =>
		dispatch({ type: MerchKitsFiltersActionKeys.set, payload: newFilters }),
	clear: () => dispatch({ type: MerchKitsFiltersActionKeys.clear }),
});
