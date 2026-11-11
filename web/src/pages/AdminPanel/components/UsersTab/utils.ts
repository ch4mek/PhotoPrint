import { SortDirection } from '@/constants/sort';

import { UserFiltersNames, UsersAction, UsersFilterType } from './types';

export const usersFiltersReducer = (
	state: UsersFilterType,
	{ type, payload }: UsersAction
): UsersFilterType => {
	switch (type) {
		case UserFiltersNames.ID: {
			return { ...state, id: payload as string };
		}

		case UserFiltersNames.USER: {
			return { ...state, user: payload as string };
		}

		case UserFiltersNames.EMAIL: {
			return { ...state, email: payload as string };
		}

		case UserFiltersNames.LOCATION: {
			return { ...state, location: payload as string[] };
		}

		case UserFiltersNames.DEPARTMENT: {
			return { ...state, departments: payload as string[] };
		}

		case UserFiltersNames.GROUP: {
			return { ...state, group: payload as string };
		}

		case UserFiltersNames.POSITION: {
			return { ...state, position: payload as string[] };
		}

		case UserFiltersNames.STATUS: {
			return { ...state, status: payload as string[] };
		}

		case UserFiltersNames.RESET: {
			return { ...(payload as UsersFilterType) };
		}

		case UserFiltersNames.APPLY_ALL: {
			return { ...state, ...(payload as UsersFilterType) };
		}

		default: {
			return state;
		}
	}
};

export const generateSetFilters = (
	dispatch: React.Dispatch<UsersAction>,
	initialState: UsersFilterType
) => ({
	id: (value: string) =>
		dispatch({ type: UserFiltersNames.ID, payload: value }),
	user: (value: string) =>
		dispatch({ type: UserFiltersNames.USER, payload: value }),
	email: (value: string) =>
		dispatch({ type: UserFiltersNames.EMAIL, payload: value }),
	position: (value: string[]) =>
		dispatch({ type: UserFiltersNames.POSITION, payload: value }),
	department: (value: string[]) =>
		dispatch({ type: UserFiltersNames.DEPARTMENT, payload: value }),
	group: (value: string) =>
		dispatch({ type: UserFiltersNames.GROUP, payload: value }),
	location: (value: string[]) =>
		dispatch({ type: UserFiltersNames.LOCATION, payload: value }),
	reset: () =>
		dispatch({ type: UserFiltersNames.RESET, payload: initialState }),
	applyAll: (value: UsersFilterType) =>
		dispatch({ type: UserFiltersNames.APPLY_ALL, payload: value }),
	status: (value: string[]) =>
		dispatch({ type: UserFiltersNames.STATUS, payload: value }),
});

export const getOrderByParams = (orderBy: string, order: SortDirection) =>
	orderBy === 'fullName'
		? `fullNameRu eq null asc, fullNameRu ${order}, fullName ${order}`
		: `${orderBy} ${order}`;
