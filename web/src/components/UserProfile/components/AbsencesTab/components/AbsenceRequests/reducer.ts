import { Dispatch, SetStateAction } from 'react';

import { FromToProps } from '@/components/Grid';
import { defaultAbsenceRequestFilterValues } from '@/constants/absence';
import { handleSetStateAction } from '@/helpers/handleSetStateAction';
import {
	AbsenceRequestFilters,
	AbsenceRequestsFilterHandlers,
	AbsenceRequestsReducerAction,
	AbsenceRequestsTableFiltersAction,
} from '@/types/absences';

export const absenceRequestsFiltersReducer = (
	state: AbsenceRequestFilters,
	{ type, payload }: AbsenceRequestsReducerAction
): AbsenceRequestFilters => {
	switch (type) {
		case AbsenceRequestsTableFiltersAction.DateFrom:
			return { ...state, dateFrom: payload };
		case AbsenceRequestsTableFiltersAction.DateTo:
			return { ...state, dateTo: payload };
		case AbsenceRequestsTableFiltersAction.InquiryTemplateName:
			return { ...state, inquiryTemplateName: payload };
		case AbsenceRequestsTableFiltersAction.Status:
			return { ...state, status: payload };
		case AbsenceRequestsTableFiltersAction.Reset:
			return { ...payload };
		default:
			return state;
	}
};

export const createAbsenceRequestFilters = (
	state: AbsenceRequestFilters,
	dispatch: Dispatch<AbsenceRequestsReducerAction>
): AbsenceRequestsFilterHandlers => ({
	dateFrom: (payloadForAction: SetStateAction<FromToProps<Date>>) => {
		const payload = handleSetStateAction(state.dateFrom, payloadForAction);

		dispatch({
			type: AbsenceRequestsTableFiltersAction.DateFrom,
			payload,
		});
	},
	dateTo: (payloadForAction: SetStateAction<FromToProps<Date>>) => {
		const payload = handleSetStateAction(state.dateTo, payloadForAction);

		dispatch({
			type: AbsenceRequestsTableFiltersAction.DateTo,
			payload,
		});
	},
	inquiryTemplateName: (payloadForAction: SetStateAction<string[]>) => {
		const payload = handleSetStateAction(
			state.inquiryTemplateName,
			payloadForAction
		);

		dispatch({
			type: AbsenceRequestsTableFiltersAction.InquiryTemplateName,
			payload,
		});
	},
	status: (payloadForAction: SetStateAction<string[]>) => {
		const payload = handleSetStateAction(state.status, payloadForAction);

		dispatch({
			type: AbsenceRequestsTableFiltersAction.Status,
			payload,
		});
	},
	reset: () =>
		dispatch({
			type: AbsenceRequestsTableFiltersAction.Reset,
			payload: defaultAbsenceRequestFilterValues,
		}),
});
