import { Dispatch } from 'react';

import {
	ConversationAction,
	ConversationFilter,
	ConversationFilterSetter,
	ConversationTableFiltersKeys,
} from '../../types';

import { FromToProps } from '@/components/Grid';

export const conversationFiltersReducer = (
	state: ConversationFilter,
	{ type, payload }: ConversationAction
): ConversationFilter => {
	switch (type) {
		case ConversationTableFiltersKeys.date: {
			return { ...state, date: payload as FromToProps<Date> };
		}

		case ConversationTableFiltersKeys.conversationResult: {
			return { ...state, conversationResult: payload as string[] };
		}

		case ConversationTableFiltersKeys.conversationType: {
			return { ...state, conversationType: payload as string[] };
		}

		case ConversationTableFiltersKeys.agreements: {
			return { ...state, agreements: payload as string };
		}

		case ConversationTableFiltersKeys.employeeTheses: {
			return { ...state, employeeTheses: payload as string };
		}

		case ConversationTableFiltersKeys.theses: {
			return { ...state, theses: payload as string };
		}

		case ConversationTableFiltersKeys.supervisor: {
			return { ...state, supervisor: payload as string };
		}

		case ConversationTableFiltersKeys.set: {
			return payload as ConversationFilter;
		}

		default: {
			return state;
		}
	}
};

export const generateSetFilters = (
	dispatch: Dispatch<ConversationAction>
): ConversationFilterSetter => ({
	date: (date: FromToProps<Date>) =>
		dispatch({ type: ConversationTableFiltersKeys.date, payload: date }),
	conversationResult: (result: string[]) =>
		dispatch({
			type: ConversationTableFiltersKeys.conversationResult,
			payload: result,
		}),
	conversationType: (conversationType: string[]) =>
		dispatch({
			type: ConversationTableFiltersKeys.conversationType,
			payload: conversationType,
		}),
	agreements: (agreements: string) =>
		dispatch({
			type: ConversationTableFiltersKeys.agreements,
			payload: agreements,
		}),
	employeeTheses: (employeeTheses: string) =>
		dispatch({
			type: ConversationTableFiltersKeys.employeeTheses,
			payload: employeeTheses,
		}),
	theses: (theses: string) =>
		dispatch({
			type: ConversationTableFiltersKeys.theses,
			payload: theses,
		}),
	supervisor: (supervisor: string) =>
		dispatch({
			type: ConversationTableFiltersKeys.supervisor,
			payload: supervisor,
		}),
	set: (newFilters: ConversationFilter) =>
		dispatch({ type: ConversationTableFiltersKeys.set, payload: newFilters }),
});
