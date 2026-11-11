import { Dispatch } from 'react';

import {
	PerformanceReviewFilter,
	PerformanceReviewFilterSetter,
	PerformanceReviewTableFiltersKeys,
} from '../../types';

import { FromToProps } from '@/components/Grid';
import { PerformanceReviewAction } from '@/pages/TeamBoardUser/components/PerformanceReviewTab/types';

export const performanceFiltersReducer = (
	state: PerformanceReviewFilter,
	{ type, payload }: PerformanceReviewAction
): PerformanceReviewFilter => {
	switch (type) {
		case PerformanceReviewTableFiltersKeys.performanceReviewDate: {
			return { ...state, performanceReviewDate: payload };
		}

		case PerformanceReviewTableFiltersKeys.agreements: {
			return { ...state, agreements: payload };
		}

		case PerformanceReviewTableFiltersKeys.employeeTheses: {
			return { ...state, employeeTheses: payload };
		}

		case PerformanceReviewTableFiltersKeys.theses: {
			return { ...state, theses: payload };
		}

		case PerformanceReviewTableFiltersKeys.set: {
			return payload;
		}

		default: {
			return state;
		}
	}
};

export const generateSetFilters = (
	dispatch: Dispatch<PerformanceReviewAction>
): PerformanceReviewFilterSetter => ({
	performanceReviewDate: (performanceReviewDate: FromToProps<Date>) =>
		dispatch({
			type: PerformanceReviewTableFiltersKeys.performanceReviewDate,
			payload: performanceReviewDate,
		}),
	agreements: (agreements: string) =>
		dispatch({
			type: PerformanceReviewTableFiltersKeys.agreements,
			payload: agreements,
		}),
	employeeTheses: (employeeTheses: string) =>
		dispatch({
			type: PerformanceReviewTableFiltersKeys.employeeTheses,
			payload: employeeTheses,
		}),
	theses: (theses: string) =>
		dispatch({
			type: PerformanceReviewTableFiltersKeys.theses,
			payload: theses,
		}),
	set: (newFilters: PerformanceReviewFilter) =>
		dispatch({
			type: PerformanceReviewTableFiltersKeys.set,
			payload: newFilters,
		}),
});
