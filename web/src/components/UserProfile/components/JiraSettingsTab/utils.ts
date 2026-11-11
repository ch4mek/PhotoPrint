import { Dispatch } from 'react';
import { SortColumn } from 'react-data-grid';

import { areObjectsShallowlyEqual } from '@/helpers/areValuesEqual';
import { JiraSettingsFilters, JiraSettingsGridRow } from '@/types/jira';

import { defaultJiraSettingsFilters } from './constants';
import {
	JiraSettingsAction,
	JiraSettingsFiltersNames,
	JiraSettingsSetFilters,
} from './types';

export const getRowKey = (row: JiraSettingsGridRow) => row.id;

export const jiraSettingsFiltersReducer = (
	state: JiraSettingsFilters,
	action: JiraSettingsAction
): JiraSettingsFilters => {
	switch (action.type) {
		case JiraSettingsFiltersNames.URL:
			return { ...state, url: action.payload };

		case JiraSettingsFiltersNames.EMAIL:
			return { ...state, email: action.payload };

		case JiraSettingsFiltersNames.PROJECT_NAME:
			return { ...state, projectName: action.payload };

		case JiraSettingsFiltersNames.PROJECT_TAG:
			return { ...state, projectTag: action.payload };

		case JiraSettingsFiltersNames.SET: {
			return { ...action.payload };
		}

		case JiraSettingsFiltersNames.RESET:
			return defaultJiraSettingsFilters;

		default:
			return state;
	}
};

export const generateJiraSettingsSetFilters = (
	dispatch: Dispatch<JiraSettingsAction>
): JiraSettingsSetFilters => ({
	url: (value: string) =>
		dispatch({ type: JiraSettingsFiltersNames.URL, payload: value }),

	email: (value: string) =>
		dispatch({ type: JiraSettingsFiltersNames.EMAIL, payload: value }),

	projectName: (value: string) =>
		dispatch({ type: JiraSettingsFiltersNames.PROJECT_NAME, payload: value }),

	projectTag: (value: string) =>
		dispatch({ type: JiraSettingsFiltersNames.PROJECT_TAG, payload: value }),

	set: (payload: JiraSettingsFilters) => {
		dispatch({
			type: JiraSettingsFiltersNames.SET,
			payload,
		});
	},

	reset: () => {
		dispatch({
			type: JiraSettingsFiltersNames.RESET,
		});
	},
});

export const jiraSettingsFiltersComparator = (
	filters: JiraSettingsFilters,
	initialState: JiraSettingsFilters
) => areObjectsShallowlyEqual(filters, initialState);

const jiraSettingsOrderByFieldMap: Record<string, string> = {
	projectTag: 'jiraProjectTag',
};

export const buildJiraSettingsOrderBy = (sortColumns: SortColumn[]) =>
	sortColumns
		.map(({ columnKey, direction }) => {
			const field = jiraSettingsOrderByFieldMap[columnKey] ?? columnKey;

			return `${field} ${direction}`;
		})
		.join(',');
