import { Dispatch, SetStateAction } from 'react';

import { FromToProps } from '@/components/Grid';
import {
	areArraysEqualByPrimitiveItems,
	areFromToCompatibleObjectsEqual,
	arePrimitiveValuesEqual,
} from '@/helpers/areValuesEqual';
import { formatDateString } from '@/helpers/dateToISOString';
import { handleSetStateAction } from '@/helpers/handleSetStateAction';
import {
	DevelopmentPlanStatus,
	UserDevelopmentPlan,
} from '@/types/developmentPlan';

import { defaultDevelopmentPlanFilterValues } from './constants';
import {
	UserDevelopmentPlanFilterHandlers,
	UserDevelopmentPlanFilters,
	UserDevelopmentPlanReducerAction,
	UserDevelopmentPlanRow,
	UserDevelopmentPlanTableFiltersAction,
} from './types';

export const developmentPlanFiltersReducer = (
	state: UserDevelopmentPlanFilters,
	{ type, payload }: UserDevelopmentPlanReducerAction
): UserDevelopmentPlanFilters => {
	switch (type) {
		case UserDevelopmentPlanTableFiltersAction.StartDate:
			return { ...state, startDate: payload };
		case UserDevelopmentPlanTableFiltersAction.EndDate:
			return { ...state, endDate: payload };
		case UserDevelopmentPlanTableFiltersAction.DeadlineDate:
			return { ...state, deadlineDate: payload };
		case UserDevelopmentPlanTableFiltersAction.Status:
			return { ...state, status: payload };
		case UserDevelopmentPlanTableFiltersAction.Summary:
			return { ...state, summary: payload };
		case UserDevelopmentPlanTableFiltersAction.Reset:
			return { ...payload };
		default:
			return state;
	}
};

export const createDevelopmentPlanFilters = (
	state: UserDevelopmentPlanFilters,
	dispatch: Dispatch<UserDevelopmentPlanReducerAction>
): UserDevelopmentPlanFilterHandlers => ({
	startDate: (payloadForAction: SetStateAction<FromToProps<Date>>) => {
		const payload = handleSetStateAction(state.startDate, payloadForAction);

		dispatch({
			type: UserDevelopmentPlanTableFiltersAction.StartDate,
			payload,
		});
	},
	endDate: (payloadForAction: SetStateAction<FromToProps<Date>>) => {
		const payload = handleSetStateAction(state.endDate, payloadForAction);

		dispatch({
			type: UserDevelopmentPlanTableFiltersAction.EndDate,
			payload,
		});
	},
	deadlineDate: (payloadForAction: SetStateAction<FromToProps<Date>>) => {
		const payload = handleSetStateAction(state.deadlineDate, payloadForAction);

		dispatch({
			type: UserDevelopmentPlanTableFiltersAction.DeadlineDate,
			payload,
		});
	},
	summary: (payloadForAction: SetStateAction<string>) => {
		const payload = handleSetStateAction(state.summary, payloadForAction);

		dispatch({
			type: UserDevelopmentPlanTableFiltersAction.Summary,
			payload,
		});
	},
	status: (payloadForAction: SetStateAction<string[]>) => {
		const payload = handleSetStateAction(state.status, payloadForAction);

		dispatch({
			type: UserDevelopmentPlanTableFiltersAction.Status,
			payload,
		});
	},
	reset: () =>
		dispatch({
			type: UserDevelopmentPlanTableFiltersAction.Reset,
			payload: defaultDevelopmentPlanFilterValues,
		}),
});

export const isResetDisabled = (filters: UserDevelopmentPlanFilters) =>
	(Object.keys(filters) as Array<keyof typeof filters>).every((key) => {
		if (Array.isArray(filters[key])) {
			return areArraysEqualByPrimitiveItems(
				filters[key] as string[],
				defaultDevelopmentPlanFilterValues[key] as string[]
			);
		}

		if (typeof filters[key] === 'object') {
			return areFromToCompatibleObjectsEqual(
				filters[key] as FromToProps<Date>,
				defaultDevelopmentPlanFilterValues[key] as FromToProps<Date>
			);
		}

		return arePrimitiveValuesEqual(
			filters[key] as string,
			defaultDevelopmentPlanFilterValues[key] as string
		);
	});

export const mapDevelopmentPlanResponseToRows = (data: UserDevelopmentPlan[]) =>
	data.map<UserDevelopmentPlanRow>(
		({ startDate, deadlineDate, endDate, status, ...rest }) => ({
			...rest,
			startDate: formatDateString(startDate, ''),
			deadlineDate: formatDateString(deadlineDate, ''),
			endDate: formatDateString(endDate, ''),
			status: DevelopmentPlanStatus[status],
		})
	);
