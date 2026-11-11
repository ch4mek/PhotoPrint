import { FromToProps } from '@/components/Grid';

import { CONTRACTS_FILTERS_INITIAL_STATE } from './constants';
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
		case ContractsFilterActionType.SET_FIRE_DATE_FILTER:
			return {
				...state,
				fireDate: {
					...state.fireDate,
					...action.payload,
				},
			};
		case ContractsFilterActionType.SET_FIRE_TYPE_FILTER:
			return {
				...state,
				fireType: action.payload,
			};
		case ContractsFilterActionType.SET_FIRE_REASON_FILTER:
			return {
				...state,
				fireReason: action.payload,
			};
		case ContractsFilterActionType.SET_FIRE_DESCRIPTION_FILTER:
			return {
				...state,
				fireDescription: action.payload,
			};
		case ContractsFilterActionType.RESET_FILTERS:
			return CONTRACTS_FILTERS_INITIAL_STATE;
		default: {
			return state;
		}
	}
};

export const createSetFilters = (
	dispatch: React.Dispatch<ContractsFilterAction>
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
	fireDate: (payload: FromToProps<Date>) =>
		dispatch({
			type: ContractsFilterActionType.SET_FIRE_DATE_FILTER,
			payload,
		}),
	fireType: (payload: string[]) =>
		dispatch({
			type: ContractsFilterActionType.SET_FIRE_TYPE_FILTER,
			payload,
		}),
	fireReason: (payload: string) =>
		dispatch({
			type: ContractsFilterActionType.SET_FIRE_REASON_FILTER,
			payload,
		}),
	fireDescription: (payload: string) =>
		dispatch({
			type: ContractsFilterActionType.SET_FIRE_DESCRIPTION_FILTER,
			payload,
		}),
});
