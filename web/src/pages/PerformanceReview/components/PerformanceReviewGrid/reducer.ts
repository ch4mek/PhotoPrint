import { Dispatch } from 'react';

import { defaultPerformanceReviewFilterValues } from '@/pages/PerformanceReview/components/PerformanceReviewGrid/constants';
import {
	PerformanceReviewAction,
	PerformanceReviewsFilterSetter,
	PerformanceReviewTableFilters,
	PerformanceReviewTableFiltersKeys,
} from '@/pages/PerformanceReview/components/PerformanceReviewGrid/types';

export const performanceReviewFiltersReducer = (
	state: PerformanceReviewTableFilters,
	{ type, payload }: PerformanceReviewAction
): PerformanceReviewTableFilters => {
	switch (type) {
		case PerformanceReviewTableFiltersKeys.id: {
			return { ...state, id: payload as string };
		}

		case PerformanceReviewTableFiltersKeys.fullName: {
			return { ...state, fullName: payload as string };
		}

		case PerformanceReviewTableFiltersKeys.departments: {
			return { ...state, departments: payload as string[] };
		}

		case PerformanceReviewTableFiltersKeys.set: {
			return payload as PerformanceReviewTableFilters;
		}

		case PerformanceReviewTableFiltersKeys.reset: {
			return payload as PerformanceReviewTableFilters;
		}

		default: {
			return state;
		}
	}
};

export const generateSetFilters = (
	dispatch: Dispatch<PerformanceReviewAction>
): PerformanceReviewsFilterSetter => ({
	id: (value: string) =>
		dispatch({
			type: PerformanceReviewTableFiltersKeys.id,
			payload: value,
		}),
	fullName: (value: string) =>
		dispatch({
			type: PerformanceReviewTableFiltersKeys.fullName,
			payload: value,
		}),
	departments: (value: string[]) =>
		dispatch({
			type: PerformanceReviewTableFiltersKeys.departments,
			payload: value,
		}),
	set: (value: PerformanceReviewTableFilters) =>
		dispatch({ type: PerformanceReviewTableFiltersKeys.set, payload: value }),
	reset: () =>
		dispatch({
			type: PerformanceReviewTableFiltersKeys.reset,
			payload: defaultPerformanceReviewFilterValues,
		}),
});
