import { Dispatch } from 'react';

import { FromToProps } from '@/components/Grid';
import { SetFilters } from '@/types/helpers';

import { initialLabboardFilters } from './constants';
import {
	LabboardFilters,
	LabboardFiltersAction,
	LabboardFiltersActionType,
} from './types';

export const labboardFiltersReducer = (
	state: LabboardFilters,
	action: LabboardFiltersAction
): LabboardFilters => {
	switch (action.type) {
		case LabboardFiltersActionType.SetId:
			return { ...state, id: action.payload };

		case LabboardFiltersActionType.SetUser:
			return { ...state, user: action.payload };

		case LabboardFiltersActionType.SetInternshipSpecialization:
			return { ...state, internshipSpecialization: action.payload };

		case LabboardFiltersActionType.SetGroup:
			return { ...state, group: action.payload };

		case LabboardFiltersActionType.SetInternshipType:
			return { ...state, internshipType: action.payload };

		case LabboardFiltersActionType.SetInternshipEngineerStage:
			return { ...state, internshipEngineerStage: action.payload };

		case LabboardFiltersActionType.SetEnglishLevel:
			return { ...state, englishLevel: action.payload };

		case LabboardFiltersActionType.SetInternshipApprovedPoints:
			return { ...state, internshipApprovedPoints: action.payload };

		case LabboardFiltersActionType.SetInternshipCompletedPoints:
			return { ...state, internshipCompletedPoints: action.payload };

		case LabboardFiltersActionType.SetCurrentLocation:
			return { ...state, currentLocation: action.payload };

		case LabboardFiltersActionType.SetInternshipStartDate:
			return { ...state, internshipStartDate: action.payload };

		case LabboardFiltersActionType.SetInternshipPlannedEndDate:
			return { ...state, internshipPlannedEndDate: action.payload };

		case LabboardFiltersActionType.SetStatus:
			return { ...state, status: action.payload };

		case LabboardFiltersActionType.SetInternshipFinalResult:
			return { ...state, internshipFinalResult: action.payload };

		case LabboardFiltersActionType.SetComments:
			return { ...state, comments: action.payload };

		case LabboardFiltersActionType.Set:
			return { ...action.payload };

		case LabboardFiltersActionType.Reset:
			return initialLabboardFilters;

		default:
			return state;
	}
};

export const createSetFilters = (
	dispatch: Dispatch<LabboardFiltersAction>
): SetFilters<LabboardFilters> => ({
	id: (payload: string) =>
		dispatch({ type: LabboardFiltersActionType.SetId, payload }),

	user: (payload: string) =>
		dispatch({ type: LabboardFiltersActionType.SetUser, payload }),

	internshipSpecialization: (payload: string) =>
		dispatch({
			type: LabboardFiltersActionType.SetInternshipSpecialization,
			payload,
		}),

	group: (payload: string) =>
		dispatch({ type: LabboardFiltersActionType.SetGroup, payload }),

	internshipType: (payload: string[]) =>
		dispatch({ type: LabboardFiltersActionType.SetInternshipType, payload }),

	internshipEngineerStage: (payload: string) =>
		dispatch({
			type: LabboardFiltersActionType.SetInternshipEngineerStage,
			payload,
		}),

	englishLevel: (payload: string[]) =>
		dispatch({ type: LabboardFiltersActionType.SetEnglishLevel, payload }),

	internshipApprovedPoints: (payload: FromToProps<string>) =>
		dispatch({
			type: LabboardFiltersActionType.SetInternshipApprovedPoints,
			payload,
		}),

	internshipCompletedPoints: (payload: FromToProps<string>) =>
		dispatch({
			type: LabboardFiltersActionType.SetInternshipCompletedPoints,
			payload,
		}),

	currentLocation: (payload: string) =>
		dispatch({ type: LabboardFiltersActionType.SetCurrentLocation, payload }),

	internshipStartDate: (payload: FromToProps<Date>) =>
		dispatch({
			type: LabboardFiltersActionType.SetInternshipStartDate,
			payload,
		}),

	internshipPlannedEndDate: (payload: FromToProps<Date>) =>
		dispatch({
			type: LabboardFiltersActionType.SetInternshipPlannedEndDate,
			payload,
		}),

	status: (payload: string[]) =>
		dispatch({ type: LabboardFiltersActionType.SetStatus, payload }),

	internshipFinalResult: (payload: string[]) =>
		dispatch({
			type: LabboardFiltersActionType.SetInternshipFinalResult,
			payload,
		}),

	comments: (payload: string) =>
		dispatch({ type: LabboardFiltersActionType.SetComments, payload }),

	set: (payload: LabboardFilters) =>
		dispatch({ type: LabboardFiltersActionType.Set, payload }),

	reset: () => dispatch({ type: LabboardFiltersActionType.Reset }),
});
