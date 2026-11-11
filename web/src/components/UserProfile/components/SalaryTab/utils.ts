import isFunction from 'lodash/isFunction';
import { Dispatch, SetStateAction } from 'react';

import { FromToProps } from '@/components/Grid';

import { defaultUserSalaryFilters } from './constants';
import {
	UserSalaryFilterActions,
	UserSalaryFilters,
	UserSalaryFiltersHandlers,
	UserSalaryReducerAction,
} from './types';

export function userSalaryReducer(
	state: UserSalaryFilters,
	{ type, payload }: UserSalaryReducerAction
): UserSalaryFilters {
	switch (type) {
		case UserSalaryFilterActions.RESET:
			return { ...payload };
		case UserSalaryFilterActions.SALARY_FROM:
			return {
				...state,
				salaryFrom: {
					...state.salaryFrom,
					...payload,
				},
			};
		case UserSalaryFilterActions.SALARY_USD:
			return {
				...state,
				salaryUsd: {
					...state.salaryUsd,
					...payload,
				},
			};
		default:
			return state;
	}
}

const handleSetStateAction = <T>(
	prevValue: T,
	payload: SetStateAction<T>
): T => {
	if (isFunction(payload)) {
		return payload(prevValue);
	}

	return payload;
};

export const createUserSalaryFiltersHandlers = (
	state: UserSalaryFilters,
	dispatch: Dispatch<UserSalaryReducerAction>
): UserSalaryFiltersHandlers => ({
	reset: () =>
		dispatch({
			type: UserSalaryFilterActions.RESET,
			payload: defaultUserSalaryFilters,
		}),
	salaryFrom: (payloadForAction: SetStateAction<FromToProps<Date>>) => {
		const payload = handleSetStateAction(state.salaryFrom, payloadForAction);

		dispatch({ type: UserSalaryFilterActions.SALARY_FROM, payload });
	},
	salaryUsd: (payloadForAction: SetStateAction<FromToProps<string>>) => {
		const payload = handleSetStateAction(state.salaryUsd, payloadForAction);

		dispatch({ type: UserSalaryFilterActions.SALARY_USD, payload });
	},
});

export const isResetDisabled = (filters: UserSalaryFilters) => {
	const keys = Object.keys(filters) as Array<keyof typeof filters>;

	return keys.every((key) => filters[key] === defaultUserSalaryFilters[key]);
};
