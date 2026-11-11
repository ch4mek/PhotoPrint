import { Dispatch } from 'react';

import { FromToProps } from '@/components/Grid';

import { contractsFiltersInitialState } from './constants';
import {
	ContractsFilterAction,
	ContractsFilterActionType,
	ContractsFilters,
	SetFiltersType,
} from './types';

export const contractsFiltersReducer = (
	state: ContractsFilters,
	action: ContractsFilterAction
) => {
	switch (action.type) {
		case ContractsFilterActionType.SET_CONTRACT_FROM_FILTER:
			return {
				...state,
				contractFrom: {
					...state.contractFrom,
					...action.payload,
				},
			};

		case ContractsFilterActionType.SET_CONTRACT_TO_FILTER:
			return {
				...state,
				contractTo: {
					...state.contractTo,
					...action.payload,
				},
			};
		case ContractsFilterActionType.RESET_FILTERS:
			return contractsFiltersInitialState;
		default: {
			return state;
		}
	}
};

export const createSetFilters = (
	dispatch: Dispatch<ContractsFilterAction>
): SetFiltersType => ({
	reset: () => dispatch({ type: ContractsFilterActionType.RESET_FILTERS }),
	contractFrom: (payload: FromToProps<Date>) =>
		dispatch({
			type: ContractsFilterActionType.SET_CONTRACT_FROM_FILTER,
			payload,
		}),
	contractTo: (payload: FromToProps<Date>) =>
		dispatch({
			type: ContractsFilterActionType.SET_CONTRACT_TO_FILTER,
			payload,
		}),
});
