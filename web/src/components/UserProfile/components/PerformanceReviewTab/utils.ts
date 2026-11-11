import isFunction from 'lodash/isFunction';
import { Dispatch, SetStateAction } from 'react';

import { FromToProps } from '@/components/Grid';
import { formatDateString } from '@/helpers/dateToISOString';
import { UserPerformanceReviewData } from '@/types/teamboard';

import { defaultUserPerformanceReviewFilters } from './constants';
import {
	UserPerformanceReviewFilterAction,
	UserPerformanceReviewFilters,
	UserPerformanceReviewFiltersHandlers,
	UserPerformanceReviewReducerAction,
} from './types';

export const isResetDisabled = (filters: UserPerformanceReviewFilters) => {
	const keys = Object.keys(filters) as Array<keyof typeof filters>;

	return keys.every(
		(key) => filters[key] === defaultUserPerformanceReviewFilters[key]
	);
};

export const mapPerformanceReviewDataToRows = (
	data: UserPerformanceReviewData[]
) =>
	data.map(
		({
			id,
			performanceReviewDate,
			performanceReviewDateType,
			agreements,
			odpConversationFiles,
			odpConversationDocumentLinks,
		}) => ({
			id,
			performanceReviewDate: formatDateString(performanceReviewDate),
			performanceReviewDateType,
			agreements,
			files: odpConversationFiles,
			links: odpConversationDocumentLinks?.map(({ link }) => link) || [],
		})
	);

export function userPerformanceReviewReducer(
	state: UserPerformanceReviewFilters,
	{ type, payload }: UserPerformanceReviewReducerAction
): UserPerformanceReviewFilters {
	switch (type) {
		case UserPerformanceReviewFilterAction.Reset:
			return { ...payload };
		case UserPerformanceReviewFilterAction.Date:
			return {
				...state,
				performanceReviewDate: {
					...state.performanceReviewDate,
					...payload,
				},
			};
		case UserPerformanceReviewFilterAction.Agreements:
			return { ...state, agreements: payload };
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

export const createUserPerformanceReviewFiltersHandlers = (
	state: UserPerformanceReviewFilters,
	dispatch: Dispatch<UserPerformanceReviewReducerAction>
): UserPerformanceReviewFiltersHandlers => ({
	reset: () =>
		dispatch({
			type: UserPerformanceReviewFilterAction.Reset,
			payload: defaultUserPerformanceReviewFilters,
		}),
	performanceReviewDate: (
		payloadForAction: SetStateAction<FromToProps<Date>>
	) => {
		const payload = handleSetStateAction(
			state.performanceReviewDate,
			payloadForAction
		);

		dispatch({ type: UserPerformanceReviewFilterAction.Date, payload });
	},
	agreements: (payloadForAction: SetStateAction<string>) => {
		const payload = handleSetStateAction(state.agreements, payloadForAction);

		dispatch({ type: UserPerformanceReviewFilterAction.Agreements, payload });
	},
});
