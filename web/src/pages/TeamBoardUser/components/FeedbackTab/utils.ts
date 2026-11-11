import {
	FeedbackAction,
	FeedbackFilter,
	FeedbackFilterSetter,
	FeedbackTableFiltersKeys,
} from '../../types';

import { FromToProps } from '@/components/Grid';

import { DEFAULT_FEEDBACK_FILTER_VALUES } from './constants';

export const feedbackFiltersReducer = (
	state: FeedbackFilter,
	{ type, payload }: FeedbackAction
): FeedbackFilter => {
	switch (type) {
		case FeedbackTableFiltersKeys.date: {
			return { ...state, date: payload as FromToProps<Date> };
		}

		case FeedbackTableFiltersKeys.type: {
			return { ...state, type: payload as string[] };
		}

		case FeedbackTableFiltersKeys.client: {
			return { ...state, client: payload as string };
		}

		case FeedbackTableFiltersKeys.project: {
			return { ...state, project: payload as string };
		}

		case FeedbackTableFiltersKeys.feedback: {
			return { ...state, feedback: payload as string };
		}

		case FeedbackTableFiltersKeys.actions: {
			return { ...state, actions: payload as string };
		}

		case FeedbackTableFiltersKeys.reviewer: {
			return { ...state, reviewer: payload as string };
		}

		case FeedbackTableFiltersKeys.set: {
			return payload as FeedbackFilter;
		}

		default: {
			return state;
		}
	}
};

export const generateSetFilters = (
	dispatch: React.Dispatch<FeedbackAction>
): FeedbackFilterSetter => ({
	date: (date: FromToProps<Date>) =>
		dispatch({ type: FeedbackTableFiltersKeys.date, payload: date }),
	actions: (actions: string) =>
		dispatch({
			type: FeedbackTableFiltersKeys.actions,
			payload: actions,
		}),
	type: (type: string[]) =>
		dispatch({
			type: FeedbackTableFiltersKeys.type,
			payload: type,
		}),
	client: (client: string) =>
		dispatch({
			type: FeedbackTableFiltersKeys.client,
			payload: client,
		}),
	project: (project: string) =>
		dispatch({
			type: FeedbackTableFiltersKeys.project,
			payload: project,
		}),
	feedback: (feedback: string) =>
		dispatch({
			type: FeedbackTableFiltersKeys.feedback,
			payload: feedback,
		}),
	reviewer: (reviewer: string) =>
		dispatch({
			type: FeedbackTableFiltersKeys.reviewer,
			payload: reviewer,
		}),
	set: (newFilters: FeedbackFilter) =>
		dispatch({ type: FeedbackTableFiltersKeys.set, payload: newFilters }),
	reset: () =>
		dispatch({
			type: FeedbackTableFiltersKeys.set,
			payload: DEFAULT_FEEDBACK_FILTER_VALUES,
		}),
});
