import { fromToDateObject } from '@/constants/defaultValues';
import { SortDirection } from '@/constants/sort';
import { TRACKED_TIME_DEFAULT_VALUE } from '@/pages/constants';
import { getOrderBySaleManager } from '@/pages/Staffing/components/InterviewTab/utils/getOrderBySaleManager';
import { GridColumnWidth } from '@/types/grid';

import { InterviewTabFilters, SortMapper } from './types';

export const ADD_BUTTON_TITLE = 'Add interview';

export const INITIAL_ORDER_BY_VALUE = 'date';

export const DEFAULT_INTERVIEW_FILTERS: InterviewTabFilters = {
	id: '',
	specialization: '',
	client: '',
	type: [],
	date: fromToDateObject,
	time: TRACKED_TIME_DEFAULT_VALUE,
	status: [],
	saleManager: '',
	jiraTask: '',
	reasonForRefusal: '',
	note: '',
};

export const columnsWidths: Record<string, GridColumnWidth> = {
	id: 67,
	specialization: 130,
	client: {
		maxWidth: 213,
		width: 186,
	},
	type: 87,
	date: 100,
	time: 80,
	status: 147,
	saleManager: 213,
	jiraTask: 100,
	reasonForRefusal: 200,
	note: 260,
};

export const interviewSortMapper: Record<string, SortMapper> = {
	date: (direction) =>
		`userInterviewDate ${direction}, userInterviewFromUtc ${SortDirection.ASC}`,

	time: (direction) => `userInterviewFromUtc ${direction}`,
	type: (direction) => `interviewTypeOrderWeight ${direction}`,
	statusText: (direction) => `statusOrderWeight ${direction}`,
	saleManager: (direction) => getOrderBySaleManager(direction),
};
