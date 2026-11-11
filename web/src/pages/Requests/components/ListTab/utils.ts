import { Dispatch } from 'react';

import { FromToProps } from '@/components/Grid';
import { SortDirection } from '@/constants/sort';
import { capitalizeString } from '@/helpers/capitalizeString';
import { convertOrderToQueryParam } from '@/helpers/convertOrderToQueryParam';

import {
	columnsToReverseOrder,
	DEFAULT_REQUESTS_FILTERS,
	extraOrderByKeys,
	REQUESTS_COLUMNS,
} from './constants';
import { RequestAction, RequestFiltersNames, RequestFilterType } from './types';

export const requestFiltersReducer = (
	state: RequestFilterType,
	{ type, payload }: RequestAction
): RequestFilterType => {
	switch (type) {
		case RequestFiltersNames.USER_ID: {
			return { ...state, userId: payload as string };
		}

		case RequestFiltersNames.USER: {
			return { ...state, userFullName: payload as string };
		}

		case RequestFiltersNames.ABSENCE_NAME: {
			return { ...state, inquiryTemplateName: payload as string };
		}

		case RequestFiltersNames.CREATED: {
			return { ...state, createdAtUtc: payload as FromToProps<Date> };
		}

		case RequestFiltersNames.DATE_FROM: {
			return { ...state, dateFrom: payload as FromToProps<Date> };
		}

		case RequestFiltersNames.DATE_TO: {
			return { ...state, dateTo: payload as FromToProps<Date> };
		}

		case RequestFiltersNames.STATUS: {
			return { ...state, status: payload as string[] };
		}

		case RequestFiltersNames.USER_POSITION: {
			return { ...state, userPosition: payload as string[] };
		}

		case RequestFiltersNames.USER_DEPARTMENT: {
			return { ...state, userDepartment: payload as string[] };
		}

		case RequestFiltersNames.NOTES: {
			return { ...state, notes: payload as string };
		}
		case RequestFiltersNames.RESET: {
			return { ...(payload as RequestFilterType) };
		}
		default: {
			return state;
		}
	}
};

export const generateSetFilters = (dispatch: Dispatch<RequestAction>) => ({
	userId: (value: string) =>
		dispatch({ type: RequestFiltersNames.USER_ID, payload: value }),
	user: (value: string) =>
		dispatch({ type: RequestFiltersNames.USER, payload: value }),
	inquiryTemplateName: (value: string) =>
		dispatch({ type: RequestFiltersNames.ABSENCE_NAME, payload: value }),
	createdAtUtc: (value: FromToProps<Date>) =>
		dispatch({ type: RequestFiltersNames.CREATED, payload: value }),
	departments: (value: string[]) =>
		dispatch({ type: RequestFiltersNames.USER_DEPARTMENT, payload: value }),
	userPosition: (value: string[]) =>
		dispatch({ type: RequestFiltersNames.USER_POSITION, payload: value }),
	dateFrom: (value: FromToProps<Date>) =>
		dispatch({ type: RequestFiltersNames.DATE_FROM, payload: value }),
	dateTo: (value: FromToProps<Date>) =>
		dispatch({ type: RequestFiltersNames.DATE_TO, payload: value }),
	status: (value: string[]) =>
		dispatch({ type: RequestFiltersNames.STATUS, payload: value }),
	notes: (value: string) =>
		dispatch({ type: RequestFiltersNames.NOTES, payload: value }),
	reset: (state: RequestFilterType) =>
		dispatch({ type: RequestFiltersNames.RESET, payload: state }),
});

export const checkIsEmptyFilter = (filters: RequestFilterType) =>
	JSON.stringify(filters) === JSON.stringify(DEFAULT_REQUESTS_FILTERS);

const { user, status, position } = REQUESTS_COLUMNS;
const {
	statusOrderWeight,
	userPositionOrderWeight,
	userFirstNameRu,
	userLastNameRu,
} = extraOrderByKeys;

export const getOrderValueForAbsenceRequest = (
	order: SortDirection,
	orderBy?: string
) => {
	if (!orderBy) {
		return orderBy;
	}

	switch (orderBy) {
		case status.key:
			return `${statusOrderWeight} ${order}, id ${SortDirection.DESC}`;
		case position.key:
			return `${userPositionOrderWeight} ${convertOrderToQueryParam(
				order,
				orderBy,
				columnsToReverseOrder
			)}`;
		case user.key:
			return `${userLastNameRu} eq null ${order}, ${userFirstNameRu} eq null ${order}, ${userLastNameRu} ${order}, ${userFirstNameRu} ${order}`;
		default:
			return `${capitalizeString(orderBy)} ${order}`;
	}
};
