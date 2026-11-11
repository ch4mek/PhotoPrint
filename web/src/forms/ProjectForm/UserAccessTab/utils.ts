import {
	UserAccessAction,
	UserAccessActionType,
	UserAccessFilters,
	UserAccessSetFilters,
} from '../types';

import { FromToProps } from '@/components/Grid';
import { BillabilityStatus } from '@/constants/billabilityStatus';
import {
	areArraysEqualByPrimitiveItems,
	areFromToCompatibleObjectsEqual,
} from '@/helpers/areValuesEqual';
import { formatDateString } from '@/helpers/dateToISOString';

import { DEFAULT_USERS_ACCESS_FILTERS } from './constants';

interface ProjectUser {
	departmentId: string;
	id: number;
	isActive: boolean;
	isTimeOnProjectBillable: boolean;
	projectId: number;
	projectName: string;
	userFullName: string;
	userId: number;
	userOnProjectFrom: string;
	userOnProjectTo: string | null;
}

export const mapUsersByProjectIdResponseToGridRows = (
	projectUsers: ProjectUser[],
	handleEditClick: (projectId?: number | string) => () => void
) =>
	projectUsers.map(
		({
			id,
			isActive,
			isTimeOnProjectBillable,
			userFullName,
			userOnProjectFrom,
			userOnProjectTo,
		}) => ({
			id,
			userFullName,
			isActive,
			isTimeOnProjectBillable: isTimeOnProjectBillable
				? BillabilityStatus.Billable
				: BillabilityStatus.NonBillable,
			userOnProjectFrom: formatDateString(userOnProjectFrom),
			userOnProjectTo: userOnProjectTo
				? formatDateString(userOnProjectTo)
				: null,
			handleEditClick,
			disabled: !isActive,
		})
	);

export const usersAccessFiltersReducer = (
	state: UserAccessFilters,
	action: UserAccessActionType
): UserAccessFilters => {
	switch (action.type) {
		case UserAccessAction.USER_NAME: {
			return { ...state, userName: action.payload };
		}

		case UserAccessAction.BILLABILITY: {
			return { ...state, billability: action.payload };
		}

		case UserAccessAction.STATUS: {
			return { ...state, status: action.payload };
		}

		case UserAccessAction.START_DATE: {
			return {
				...state,
				startDate: {
					...state.startDate,
					...action.payload,
				},
			};
		}

		case UserAccessAction.END_DATE: {
			return {
				...state,
				endDate: {
					...state.endDate,
					...action.payload,
				},
			};
		}
		case UserAccessAction.RESET: {
			return DEFAULT_USERS_ACCESS_FILTERS;
		}

		case UserAccessAction.SET: {
			return { ...action.payload };
		}

		default: {
			return state;
		}
	}
};

export const generateSetFilters = (
	dispatch: React.Dispatch<UserAccessActionType>
): UserAccessSetFilters => ({
	userName: (newName: string) =>
		dispatch({ type: UserAccessAction.USER_NAME, payload: newName }),
	billability: (billabilities: string[]) =>
		dispatch({
			type: UserAccessAction.BILLABILITY,
			payload: billabilities,
		}),
	status: (statusPayload: string[]) =>
		dispatch({ type: UserAccessAction.STATUS, payload: statusPayload }),
	startDate: (date: FromToProps<Date>) =>
		dispatch({
			type: UserAccessAction.START_DATE,
			payload: date,
		}),
	endDate: (date: FromToProps<Date>) => {
		dispatch({
			type: UserAccessAction.END_DATE,
			payload: date,
		});
	},
	set: (payload: UserAccessFilters) => {
		dispatch({
			type: UserAccessAction.SET,
			payload,
		});
	},
	reset: () => {
		dispatch({
			type: UserAccessAction.RESET,
		});
	},
});

export const userAccessFiltersComparator = (
	{ userName, startDate, status, billability, endDate }: UserAccessFilters,
	initialState: UserAccessFilters
) => ({
	userName: userName === initialState.userName,
	startDate: areFromToCompatibleObjectsEqual(initialState.startDate, startDate),
	endDate: areFromToCompatibleObjectsEqual(initialState.endDate, endDate),
	status: areArraysEqualByPrimitiveItems(status, initialState.status),
	billability: areArraysEqualByPrimitiveItems(
		billability,
		initialState.billability
	),
});
