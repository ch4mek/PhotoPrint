import { Dispatch, SetStateAction } from 'react';

import { FromToProps } from '@/components/Grid';
import { handleSetStateAction } from '@/helpers/handleSetStateAction';
import {
	DevelopmentPlanFilterHandlers,
	DevelopmentPlanFilters,
	DevelopmentPlanReducerAction,
	DevelopmentPlanTableFiltersAction,
} from '@/types/developmentPlan';

import { defaultDevelopmentPlanFilterValues } from './constants';

export const developmentPlanFiltersReducer = (
	state: DevelopmentPlanFilters,
	{ type, payload }: DevelopmentPlanReducerAction
): DevelopmentPlanFilters => {
	switch (type) {
		case DevelopmentPlanTableFiltersAction.StartDate:
			return { ...state, startDate: payload };
		case DevelopmentPlanTableFiltersAction.EndDate:
			return { ...state, endDate: payload };
		case DevelopmentPlanTableFiltersAction.DeadlineDate:
			return { ...state, deadlineDate: payload };
		case DevelopmentPlanTableFiltersAction.Status:
			return { ...state, status: payload };
		case DevelopmentPlanTableFiltersAction.Summary:
			return { ...state, summary: payload };
		case DevelopmentPlanTableFiltersAction.Notes:
			return { ...state, notes: payload };
		case DevelopmentPlanTableFiltersAction.Reset:
			return { ...payload };
		default:
			return state;
	}
};

export const generateSetFilters = (
	state: DevelopmentPlanFilters,
	dispatch: Dispatch<DevelopmentPlanReducerAction>
): DevelopmentPlanFilterHandlers => ({
	startDate: (payloadForAction: SetStateAction<FromToProps<Date>>) => {
		const payload = handleSetStateAction(state.startDate, payloadForAction);

		dispatch({
			type: DevelopmentPlanTableFiltersAction.StartDate,
			payload,
		});
	},
	endDate: (payloadForAction: SetStateAction<FromToProps<Date>>) => {
		const payload = handleSetStateAction(state.endDate, payloadForAction);

		dispatch({
			type: DevelopmentPlanTableFiltersAction.EndDate,
			payload,
		});
	},
	deadlineDate: (payloadForAction: SetStateAction<FromToProps<Date>>) => {
		const payload = handleSetStateAction(state.deadlineDate, payloadForAction);

		dispatch({
			type: DevelopmentPlanTableFiltersAction.DeadlineDate,
			payload,
		});
	},
	summary: (payloadForAction: SetStateAction<string>) => {
		const payload = handleSetStateAction(state.summary, payloadForAction);

		dispatch({
			type: DevelopmentPlanTableFiltersAction.Summary,
			payload,
		});
	},
	status: (payloadForAction: SetStateAction<string[]>) => {
		const payload = handleSetStateAction(state.status, payloadForAction);

		dispatch({
			type: DevelopmentPlanTableFiltersAction.Status,
			payload,
		});
	},
	notes: (payloadForAction: SetStateAction<string>) => {
		const payload = handleSetStateAction(state.notes, payloadForAction);

		dispatch({
			type: DevelopmentPlanTableFiltersAction.Notes,
			payload,
		});
	},
	reset: () =>
		dispatch({
			type: DevelopmentPlanTableFiltersAction.Reset,
			payload: defaultDevelopmentPlanFilterValues,
		}),
});
