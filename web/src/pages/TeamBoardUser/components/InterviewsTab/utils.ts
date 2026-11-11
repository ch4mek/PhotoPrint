import { Dispatch } from 'react';
import { SortColumn, SortDirection } from 'react-data-grid';

import { FromToProps } from '@/components/Grid';
import {
	areArraysEqualByPrimitiveItems,
	areFromToCompatibleObjectsEqual,
} from '@/helpers/areValuesEqual';
import { InterviewStatus, InterviewType } from '@/types/interview';

import { DEFAULT_INTERVIEW_FILTERS, interviewSortMapper } from './constants';
import {
	InterviewAction,
	InterviewSetFilters,
	InterviewTabFilters,
	InterviewTabFiltersNames,
} from './types';

export const interviewFiltersReducer = (
	state: InterviewTabFilters,
	action: InterviewAction
): InterviewTabFilters => {
	switch (action.type) {
		case InterviewTabFiltersNames.ID:
			return { ...state, id: action.payload };

		case InterviewTabFiltersNames.SPECIALIZATION:
			return { ...state, specialization: action.payload };

		case InterviewTabFiltersNames.CLIENT:
			return { ...state, client: action.payload };

		case InterviewTabFiltersNames.TYPE:
			return { ...state, type: action.payload };

		case InterviewTabFiltersNames.DATE:
			return { ...state, date: action.payload };

		case InterviewTabFiltersNames.TIME:
			return { ...state, time: action.payload };

		case InterviewTabFiltersNames.STATUS:
			return { ...state, status: action.payload };

		case InterviewTabFiltersNames.SALE_MANAGER:
			return { ...state, saleManager: action.payload };

		case InterviewTabFiltersNames.JIRA_TASK:
			return { ...state, jiraTask: action.payload };

		case InterviewTabFiltersNames.REASON_FOR_REFUSAL:
			return { ...state, reasonForRefusal: action.payload };

		case InterviewTabFiltersNames.NOTE:
			return { ...state, note: action.payload };

		case InterviewTabFiltersNames.SET: {
			return { ...action.payload };
		}

		case InterviewTabFiltersNames.RESET:
			return DEFAULT_INTERVIEW_FILTERS;

		default:
			return state;
	}
};

export const generateInterviewSetFilters = (
	dispatch: Dispatch<InterviewAction>
): InterviewSetFilters => ({
	id: (value: string) =>
		dispatch({ type: InterviewTabFiltersNames.ID, payload: value }),

	specialization: (value: string) =>
		dispatch({ type: InterviewTabFiltersNames.SPECIALIZATION, payload: value }),

	client: (value: string) =>
		dispatch({ type: InterviewTabFiltersNames.CLIENT, payload: value }),

	type: (value: InterviewType[]) =>
		dispatch({ type: InterviewTabFiltersNames.TYPE, payload: value }),

	date: (value: FromToProps<Date>) =>
		dispatch({ type: InterviewTabFiltersNames.DATE, payload: value }),

	time: (value: FromToProps<string>) =>
		dispatch({ type: InterviewTabFiltersNames.TIME, payload: value }),

	status: (value: InterviewStatus[]) =>
		dispatch({ type: InterviewTabFiltersNames.STATUS, payload: value }),

	saleManager: (value: string) =>
		dispatch({ type: InterviewTabFiltersNames.SALE_MANAGER, payload: value }),

	jiraTask: (value: string) =>
		dispatch({ type: InterviewTabFiltersNames.JIRA_TASK, payload: value }),

	reasonForRefusal: (value: string) =>
		dispatch({
			type: InterviewTabFiltersNames.REASON_FOR_REFUSAL,
			payload: value,
		}),

	note: (value: string) =>
		dispatch({ type: InterviewTabFiltersNames.NOTE, payload: value }),

	set: (payload: InterviewTabFilters) => {
		dispatch({
			type: InterviewTabFiltersNames.SET,
			payload,
		});
	},

	reset: () => {
		dispatch({
			type: InterviewTabFiltersNames.RESET,
		});
	},
});

export const interviewFiltersComparator = (
	{
		id,
		client,
		jiraTask,
		note,
		reasonForRefusal,
		saleManager,
		specialization,
		status,
		type,
		date,
		time,
	}: InterviewTabFilters,
	initialState: InterviewTabFilters
) => ({
	id: id === initialState.id,
	specialization: specialization === initialState.specialization,
	client: client === initialState.client,
	type: areArraysEqualByPrimitiveItems(type, initialState.type),
	status: areArraysEqualByPrimitiveItems(status, initialState.status),
	saleManager: saleManager === initialState.saleManager,
	jiraTask: jiraTask === initialState.jiraTask,
	reasonForRefusal: reasonForRefusal === initialState.reasonForRefusal,
	note: note === initialState.note,
	date: areFromToCompatibleObjectsEqual(initialState.date, date),
	time: areFromToCompatibleObjectsEqual(initialState.time, time),
});

export const buildInterviewOrderBy = (sortColumns: SortColumn[]) =>
	sortColumns
		.map((col) => {
			const mapper = interviewSortMapper[col.columnKey];

			if (mapper) {
				return mapper(col.direction as SortDirection);
			}

			return `${col.columnKey} ${col.direction}`;
		})
		.join(',');
