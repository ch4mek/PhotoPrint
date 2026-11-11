import { Dispatch } from 'react';

import { FromToProps } from '@/components/Grid';
import { TeamboardGridRow } from '@/types/teamboard';

import {
	TeamboardFiltersNames,
	TeamboardAction,
	TeamboardFilterType,
} from './types';

export const teamboardFiltersReducer = (
	state: TeamboardFilterType,
	{ type, payload }: TeamboardAction
): TeamboardFilterType => {
	switch (type) {
		case TeamboardFiltersNames.USER_ID: {
			return { ...state, userId: payload as string };
		}

		case TeamboardFiltersNames.USER: {
			return { ...state, user: payload as string };
		}

		case TeamboardFiltersNames.DEPARTMENT: {
			return { ...state, departments: payload as string[] };
		}

		case TeamboardFiltersNames.GROUP: {
			return { ...state, groups: payload as string };
		}

		case TeamboardFiltersNames.PROFESSIONAL_EXPERIENCE: {
			return {
				...state,
				professionalExperience: payload as FromToProps<string>,
			};
		}

		case TeamboardFiltersNames.ENGLISH_LEVEL: {
			return { ...state, englishLevel: payload as string[] };
		}

		case TeamboardFiltersNames.HARDSKILLS: {
			return { ...state, hardSkills: payload as string[] };
		}

		case TeamboardFiltersNames.SPECIALIZATION: {
			return { ...state, specialization: payload as string };
		}

		case TeamboardFiltersNames.CURRENT_LOCATION: {
			return { ...state, currentLocation: payload as string };
		}

		case TeamboardFiltersNames.POSITION: {
			return { ...state, position: payload as string[] };
		}

		case TeamboardFiltersNames.LOCATION: {
			return { ...state, location: payload as string[] };
		}

		case TeamboardFiltersNames.COMMENT: {
			return { ...state, comment: payload as string };
		}
		case TeamboardFiltersNames.RESET: {
			return { ...(payload as TeamboardFilterType) };
		}
		case TeamboardFiltersNames.REGISTERED_LOCATION: {
			return { ...state, registeredLocation: payload as string[] };
		}
		default: {
			return state;
		}
	}
};

export const generateSetFilters = (dispatch: Dispatch<TeamboardAction>) => ({
	userId: (value: string) =>
		dispatch({ type: TeamboardFiltersNames.USER_ID, payload: value }),
	user: (value: string) =>
		dispatch({ type: TeamboardFiltersNames.USER, payload: value }),
	department: (value: string[]) =>
		dispatch({ type: TeamboardFiltersNames.DEPARTMENT, payload: value }),
	group: (value: string) =>
		dispatch({ type: TeamboardFiltersNames.GROUP, payload: value }),
	professionalExperience: (value: FromToProps<string>) =>
		dispatch({
			type: TeamboardFiltersNames.PROFESSIONAL_EXPERIENCE,
			payload: value,
		}),
	englishLevel: (value: string[]) =>
		dispatch({ type: TeamboardFiltersNames.ENGLISH_LEVEL, payload: value }),
	hardSkills: (value: string[]) =>
		dispatch({ type: TeamboardFiltersNames.HARDSKILLS, payload: value }),
	specialization: (value: string) =>
		dispatch({ type: TeamboardFiltersNames.SPECIALIZATION, payload: value }),
	currentLocation: (value: string) =>
		dispatch({ type: TeamboardFiltersNames.CURRENT_LOCATION, payload: value }),
	position: (value: string[]) =>
		dispatch({ type: TeamboardFiltersNames.POSITION, payload: value }),
	location: (value: string[]) =>
		dispatch({ type: TeamboardFiltersNames.LOCATION, payload: value }),
	registeredLocation: (value: string[]) =>
		dispatch({
			type: TeamboardFiltersNames.REGISTERED_LOCATION,
			payload: value,
		}),
	comment: (value: string) =>
		dispatch({ type: TeamboardFiltersNames.COMMENT, payload: value }),
	reset: (state: TeamboardFilterType) =>
		dispatch({ type: TeamboardFiltersNames.RESET, payload: state }),
});

export const getRowKey = (row: TeamboardGridRow) => row.id;
