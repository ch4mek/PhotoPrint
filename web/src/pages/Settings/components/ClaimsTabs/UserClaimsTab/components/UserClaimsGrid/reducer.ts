import { Dispatch } from 'react';

import {
	UserClaimsTableFiltersKeys,
	UserClaimsFilterSetter,
	UserClaimTableFilters,
	UserClaimAction,
} from '@/types/settings';

import { DEFAULT_USER_CLAIMS_FILTER_VALUES } from './constants';

export const userClaimsFiltersReducer = (
	state: UserClaimTableFilters,
	{ type, payload }: UserClaimAction
): UserClaimTableFilters => {
	switch (type) {
		case UserClaimsTableFiltersKeys.userClaimName: {
			return { ...state, userClaimName: payload as string };
		}

		case UserClaimsTableFiltersKeys.userId: {
			return { ...state, userId: payload as string };
		}

		case UserClaimsTableFiltersKeys.departments: {
			return { ...state, departments: payload as string[] };
		}

		case UserClaimsTableFiltersKeys.set: {
			return payload as UserClaimTableFilters;
		}

		case UserClaimsTableFiltersKeys.reset: {
			return payload as UserClaimTableFilters;
		}

		default: {
			return state;
		}
	}
};

export const generateSetFilters = (
	dispatch: Dispatch<UserClaimAction>
): UserClaimsFilterSetter => ({
	userClaimName: (newName: string) =>
		dispatch({
			type: UserClaimsTableFiltersKeys.userClaimName,
			payload: newName,
		}),
	userId: (newId: string) =>
		dispatch({
			type: UserClaimsTableFiltersKeys.userId,
			payload: newId,
		}),
	departments: (departments: string[]) =>
		dispatch({
			type: UserClaimsTableFiltersKeys.departments,
			payload: departments,
		}),
	set: (newFilters: UserClaimTableFilters) =>
		dispatch({ type: UserClaimsTableFiltersKeys.set, payload: newFilters }),
	reset: () =>
		dispatch({
			type: UserClaimsTableFiltersKeys.reset,
			payload: DEFAULT_USER_CLAIMS_FILTER_VALUES,
		}),
});
