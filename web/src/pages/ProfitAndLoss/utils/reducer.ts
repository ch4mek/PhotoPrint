import { Dispatch } from 'react';

import { initialProfitAndLossFilters } from '../constants';
import {
	ProfitAndLossFilters,
	ProfitAndLossFiltersAction,
	ProfitAndLossFiltersActionType,
} from '../types';

export const ratesFiltersReducer = (
	state: ProfitAndLossFilters,
	action: ProfitAndLossFiltersAction
): ProfitAndLossFilters => {
	switch (action.type) {
		case ProfitAndLossFiltersActionType.SetIdFilter:
			return { ...state, id: action.payload };

		case ProfitAndLossFiltersActionType.SetUserFilter:
			return { ...state, user: action.payload };

		case ProfitAndLossFiltersActionType.SetDepartmentFilter:
			return { ...state, department: action.payload };

		case ProfitAndLossFiltersActionType.SetProjectFilter:
			return { ...state, project: action.payload };

		case ProfitAndLossFiltersActionType.SetClientFilter:
			return { ...state, client: action.payload };

		case ProfitAndLossFiltersActionType.SetLegalEntityFilter:
			return { ...state, legalEntity: action.payload };

		case ProfitAndLossFiltersActionType.Set:
			return { ...action.payload };

		case ProfitAndLossFiltersActionType.Reset:
			return initialProfitAndLossFilters;

		default:
			return state;
	}
};

export const createSetFilters = (
	dispatch: Dispatch<ProfitAndLossFiltersAction>
) => ({
	id: (payload: string) =>
		dispatch({ type: ProfitAndLossFiltersActionType.SetIdFilter, payload }),

	user: (payload: string) =>
		dispatch({ type: ProfitAndLossFiltersActionType.SetUserFilter, payload }),

	department: (payload: string[]) =>
		dispatch({
			type: ProfitAndLossFiltersActionType.SetDepartmentFilter,
			payload,
		}),

	project: (payload: string) =>
		dispatch({
			type: ProfitAndLossFiltersActionType.SetProjectFilter,
			payload,
		}),

	client: (payload: string) =>
		dispatch({ type: ProfitAndLossFiltersActionType.SetClientFilter, payload }),

	legalEntity: (payload: string) =>
		dispatch({
			type: ProfitAndLossFiltersActionType.SetLegalEntityFilter,
			payload,
		}),

	set: (payload: ProfitAndLossFilters) =>
		dispatch({ type: ProfitAndLossFiltersActionType.Set, payload }),

	reset: () => dispatch({ type: ProfitAndLossFiltersActionType.Reset }),
});
